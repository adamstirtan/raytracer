using RayTracer.Core.Math;
using RayTracer.Core.Primitives;
using RayTracer.Core.Scenes;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace RayTracer.Core;

public class Engine
{
    private readonly Scene _scene;
    private readonly RenderOptions _options;

    public event EventHandler? RenderStarted;
    public event EventHandler<TimeSpan>? RenderCompleted;

    public Engine(Scene scene, RenderOptions options)
    {
        _scene = scene ?? throw new ArgumentNullException(nameof(scene));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        // Initialize camera position from options once
        _scene.Camera.Position = _options.CameraPosition;
    }

    protected void OnRenderStarted()
    {
        RenderStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void OnRenderCompleted(TimeSpan time)
    {
        RenderCompleted?.Invoke(this, time);
    }

    // Camera control API
    public void SetCameraPosition(Vector3 position) => _scene.Camera.Position = position;

    public void MoveCamera(Vector3 delta) => _scene.Camera.Position += delta;

    public void MoveCameraForward(float amount) => MoveCamera(new Vector3(0, 0, amount));
    public void MoveCameraRight(float amount) => MoveCamera(new Vector3(amount, 0, 0));
    public void MoveCameraUp(float amount) => MoveCamera(new Vector3(0, amount, 0));

    public Image<Rgba32> Render()
    {
        OnRenderStarted();

        Stopwatch stopwatch = Stopwatch.StartNew();

        // Use the current camera position (allow runtime updates)
        // _scene.Camera.Position = _options.CameraPosition;

        // Camera FOV and basis
        float verticalFOV = MathF.PI / 4; // 45 degrees
        int fullWidth = _options.FullWidth ?? _options.Width;
        if (fullWidth < _options.Width || _options.HorizontalOffset < 0 ||
            _options.HorizontalOffset > fullWidth - _options.Width)
            throw new ArgumentOutOfRangeException(nameof(_options), "Crop must fit within the full camera view.");
        float aspectRatio = (float)fullWidth / _options.Height;
        float viewPlaneHeight = 2 * MathF.Tan(verticalFOV / 2);
        float viewPlaneWidth = viewPlaneHeight * aspectRatio;

        // Camera basis using CameraPosition and CameraTarget/Scene Camera Target
        Vector3 camPos = _scene.Camera.Position;
        Vector3 camTarget = _options.CameraTarget ?? _scene.Camera.Target ?? Vector3.Zero;
        Vector3 toTarget = camTarget - camPos;
        // A coincident target has no direction; use +Z before normalization can produce NaNs.
        Vector3 camForward = toTarget == Vector3.Zero ? Vector3.UnitZ : Vector3.Normalize(toTarget);
        Vector3 worldUp = Vector3.UnitY;
        // Handle the case where camera forward is parallel to world up (overhead camera)
        if (System.MathF.Abs(Vector3.Dot(camForward, worldUp)) > 0.999f)
        {
            // choose a different up to avoid degenerate cross product
            worldUp = Vector3.UnitZ;
        }

        Vector3 camRight = Vector3.Normalize(Vector3.Cross(camForward, worldUp));
        Vector3 camUp = Vector3.Normalize(Vector3.Cross(camRight, camForward));

        // Top-left corner of the view plane in camera space
        Vector3 viewCenter = camPos + camForward; // distance 1
        Vector3 topLeft3D = viewCenter - (camRight * (viewPlaneWidth / 2)) + (camUp * (viewPlaneHeight / 2));

        float deltaX = viewPlaneWidth / fullWidth;
        float deltaY = viewPlaneHeight / _options.Height;

        Image<Rgba32> render = new(_options.Width, _options.Height);

        Parallel.For(0, _options.Height, y =>
        {
            Vector3 pixelStart = topLeft3D - camUp * (y * deltaY);

            for (int x = 0; x < _options.Width; x++)
            {
                Vector3 color = Vector3.Zero;

                // Supersampling / stratified jittered sampling
                int spp = System.Math.Max(1, _options.SamplesPerPixel);
                int side = (int)System.MathF.Round(System.MathF.Sqrt(spp));
                if (side * side != spp) side = 1; // fallback to 1 if not perfect square

                Vector3 accum = Vector3.Zero;

                // Stratified random sampling within each pixel. The old
                // code used the centre of every subpixel cell (0.5), a
                // fixed grid, so a hard edge (silhouette, shadow
                // terminator, reflection horizon) landed on the same
                // subpixel positions in every pixel and came out as a
                // sharp, stair-stepped, speckled line. A random offset
                // inside each cell, deterministic per pixel so renders
                // are reproducible and thread-safe with no shared RNG,
                // turns those edges into a soft 1-2 pixel gradient:
                // pixels straddling an edge get a mix of both sides.
                for (int sy = 0; sy < side; sy++)
                {
                    for (int sx = 0; sx < side; sx++)
                    {
                        float jitterX = (sx + Hash01(x, y, sx, sy, 0)) / side;
                        float jitterY = (sy + Hash01(x, y, sx, sy, 1)) / side;

                        Vector3 pixelPosSS = pixelStart + camRight * ((x + _options.HorizontalOffset + jitterX) * deltaX) - camUp * (jitterY * deltaY);
                        Vector3 dirSS = Vector3.Normalize(pixelPosSS - camPos);

                        Ray ssRay = new(camPos, dirSS);
                        float ssDistance = float.MaxValue;
                        Vector3 ssColor = Vector3.Zero;

                        Raytrace(_scene, _options, ssRay, ref ssColor, 1, ref ssDistance);

                        accum += ssColor;
                    }
                }

                color = accum / (side * side);
                color = Vector3.Clamp(color, Vector3.Zero, Vector3.One);

                render[x, y] = new Rgba32(color.X, color.Y, color.Z);
            }
        });

        stopwatch.Stop();

        OnRenderCompleted(stopwatch.Elapsed);

        return render;
    }

    // Deterministic hash in [0, 1) from pixel / subpixel coordinates.
    // SplitMix-style mixing; no shared state, so Parallel.For is safe
    // and the same scene renders identically every run.
    private static float Hash01(int x, int y, int sx, int sy, int channel)
    {
        uint h = (uint)x * 374761393u + (uint)y * 668265263u + (uint)sx * 974634211u + (uint)sy * 1274126177u + (uint)channel * 2654435761u;
        h ^= h >> 16;
        h *= 2246822519u;
        h ^= h >> 13;
        h *= 3266489917u;
        h ^= h >> 16;
        return (h & 0xFFFFFFu) / 16777216f;
    }

    private static Primitive? Raytrace(Scene scene, RenderOptions options, Ray ray, ref Vector3 color, int depth, ref float distance)
    {
        if (depth > options.TraceDepth)
        {
            return null;
        }

        float minDistance = float.MaxValue;
        Primitive? closest = null;
        Vector3 closestNormal = Vector3.Zero;
        bool closestInside = false;

        foreach (Primitive primitive in scene)
        {
            IntersectionResult result = primitive.Intersect(ray, distance);

            if (result.RayIntersection != RayIntersection.Miss && result.Distance < minDistance)
            {
                minDistance = result.Distance;
                closest = primitive;
                closestInside = result.RayIntersection == RayIntersection.Inside;
                // Exit intersections shade the interior-facing side of the surface.
                closestNormal = closestInside
                    ? -result.Normal
                    : result.Normal;
            }
        }

        if (closest == null)
        {
            color = scene.Environment?.Invoke(ray.Direction) ?? Vector3.Zero;
            return null;
        }

        distance = minDistance;

        if (closest is Light)
        {
            color = closest.Material.Color;
        }
        else
        {
            Vector3 intersection = Vector3.Add(ray.Origin, Vector3.Multiply(distance, ray.Direction));
            Vector3 normal = Vector3.Normalize(closestNormal);

            Vector3 baseColor = closest.SampleSurfaceColor(intersection, normal);

            color += baseColor * closest.Material.Emission;

            foreach (Light light in scene.Lighting)
            {
                Vector3 toLight = light.Center - intersection;
                float lightDistance = toLight.Length();
                if (lightDistance <= 0.001f) continue;
                Vector3 lightDirection = toLight / lightDistance;
                // Pass the geometric (outward) normal: closestNormal is
                // flipped for Inside hits, and the shadow bias must push
                // the ray out of the surface, not further into it.
                Vector3 outwardNormal = closestInside ? -normal : normal;
                if (IsOccluded(scene, intersection, outwardNormal, lightDirection, lightDistance)) continue;
                float dot = Vector3.Dot(normal, lightDirection);

                if (!options.DisableDiffuse && dot > 0)
                {
                    // Diffuse reflection
                    color += dot * closest.Material.Diffuse * baseColor * light.Material.Color;
                }

                // Speculation reflection
                if (!options.DisableSpeculation && dot > 0)
                {
                    Vector3 reflectionDirection = Vector3.Normalize(Vector3.Reflect(-lightDirection, normal));
                    float alignment = System.Math.Clamp(Vector3.Dot(reflectionDirection, -Vector3.Normalize(ray.Direction)), 0f, 1f);
                    float specularFactor = MathF.Pow(alignment, closest.Material.Shininess ?? closest.Material.Specular);
                    color += specularFactor * closest.Material.Specular * light.Material.Color;
                }
            }

            // Add base color contribution if diffuse is disabled
            if (options.DisableDiffuse)
            {
                color += baseColor * 0.08f; // Adjust the factor as needed
            }

            if (!options.DisableReflections && depth < options.TraceDepth)
            {
                float transmission = System.Math.Clamp(closest.Material.Transmission, 0f, 1f);
                if (transmission > 0 && closest.Material.Ior > 0)
                {
                    // Refraction (Snell's law) with Schlick's Fresnel approximation.
                    // closestNormal already faces the incoming ray, so cosI is positive.
                    // Inside the material, the ray is exiting into air (n = 1).
                    float n1 = closestInside ? closest.Material.Ior : 1f;
                    float n2 = closestInside ? 1f : closest.Material.Ior;
                    float eta = n1 / n2;
                    float cosI = System.Math.Clamp(-Vector3.Dot(ray.Direction, normal), 0f, 1f);
                    float sin2T = eta * eta * (1f - cosI * cosI);
                    // Schlick's approximation to the Fresnel reflectance.
                    float r0 = (n1 - n2) / (n1 + n2);
                    r0 *= r0;
                    float fresnel = r0 + (1f - r0) * MathF.Pow(1f - cosI, 5f);

                    Vector3 reflectionDirection = Vector3.Normalize(Vector3.Reflect(ray.Direction, normal));
                    Ray reflectedRay = new(intersection + reflectionDirection * 0.001f, reflectionDirection);
                    Vector3 reflectedColor = Vector3.Zero;
                    float reflectedDistance = float.MaxValue;
                    Raytrace(scene, options, reflectedRay, ref reflectedColor, depth + 1, ref reflectedDistance);
                    color += fresnel * reflectedColor;

                    if (sin2T <= 1f)
                    {
                        // Total internal reflection (sin2T > 1) reflects everything,
                        // which the Fresnel term above already approximates at 1
                        // for grazing exits; here there is a real refracted ray.
                        float cosT = MathF.Sqrt(1f - sin2T);
                        Vector3 refractedDirection = Vector3.Normalize(
                            eta * ray.Direction + (eta * cosI - cosT) * normal);
                        Ray refractedRay = new(intersection + refractedDirection * 0.001f, refractedDirection);
                        Vector3 refractedColor = Vector3.Zero;
                        float refractedDistance = float.MaxValue;
                        Raytrace(scene, options, refractedRay, ref refractedColor, depth + 1, ref refractedDistance);
                        color += (1f - fresnel) * transmission * refractedColor;
                    }
                    else
                    {
                        // No refracted ray exists: everything reflects internally.
                        // The Fresnel weight above is < 1 at non-grazing angles,
                        // so top it up to a full reflection.
                        color += (1f - fresnel) * reflectedColor;
                    }
                }
                else if (closest.Material.Reflection > 0)
                {
                    Vector3 reflectionDirection = Vector3.Normalize(Vector3.Reflect(ray.Direction, normal));
                    Ray reflectedRay = new(intersection + reflectionDirection * 0.001f, reflectionDirection);
                    Vector3 reflectedColor = Vector3.Zero;
                    float reflectedDistance = float.MaxValue;

                    Raytrace(scene, options, reflectedRay, ref reflectedColor, depth + 1, ref reflectedDistance);

                    color += closest.Material.Reflection * reflectedColor;
                }
            }
        }

        return closest;
    }

    private static bool IsOccluded(Scene scene, Vector3 position, Vector3 outwardNormal, Vector3 lightDirection, float lightDistance)
    {
        // Offset along the surface normal, not just toward the light.
        // The old light-direction-only bias (0.001) left a shadow ray
        // starting essentially on the surface whenever the light was
        // near the terminator (light direction almost tangent), so the
        // torus scan re-detected its own surface and the shadow
        // terminator came out as a speckled line at the tube equator.
        // A 0.01 normal offset puts the ray clearly outside the surface
        // regardless of light angle, while genuine occluders are much
        // farther away than that.
        const float bias = 0.001f;
        Ray shadowRay = new(position + outwardNormal * 0.01f + lightDirection * bias, lightDirection);
        float maxDistance = lightDistance - bias;

        foreach (Primitive primitive in scene)
        {
            // Lights are emitters, including the non-geometric point lights used by scenes.
            if (primitive is Light) continue;
            // Transmissive materials let light through: a glass object must not
            // cast a solid shadow. (True caustics are out of scope for v1.)
            if (primitive.Material.Transmission > 0) continue;
            float distance = maxDistance;
            RayIntersection result = primitive.Intersects(shadowRay, ref distance);
            // Ignore hits within 0.05 of the shaded point: those are the
            // point's own surface, re-detected by a curved primitive
            // (notably the torus, whose scan can report its own surface
            // for a shadow ray leaving at a grazing angle). Without this,
            // the shadow boundary on a torus came out as single-pixel
            // speckle instead of a clean terminator. Genuine occluders
            // are farther away than 0.05 in any scene at this scale.
            if (result != RayIntersection.Miss && distance >= 0.05f && distance < maxDistance)
                return true;
        }

        return false;
    }
}
