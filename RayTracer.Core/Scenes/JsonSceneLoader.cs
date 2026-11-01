using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Json;
using RayTracer.Core.Materials;
using RayTracer.Core.Primitives;

namespace RayTracer.Core.Scenes;

/// <summary>
/// A scene built from a JSON file (see docs/scene-format.md). It is a plain
/// <see cref="Scene"/>; the only marker is <see cref="SourcePath"/>, so the
/// CLI can tell a JSON scene apart from a code scene (e.g. it cannot be
/// animated with --frames).
/// </summary>
public sealed class JsonScene : Scene
{
    public string? SourcePath { get; init; }
    public string? SceneName { get; init; }
}

/// <summary>
/// Loads a <see cref="JsonScene"/> from the JSON scene format documented in
/// docs/scene-format.md. Errors name the file and the JSON element they came
/// from; a <see cref="JsonException"/> from the parser also carries its line
/// number. Unknown object types, materials, UV modes and environment types
/// are errors, not silently skipped objects.
/// </summary>
public static class JsonSceneLoader
{
    /// <summary>Directory the CLI copies bundled JSON scenes to at build time.</summary>
    public static string BundledDirectory =>
        Path.Combine(AppContext.BaseDirectory, "Scenes", "Json");

    /// <summary>
    /// Resolves a --scene name to a bundled JSON file, or null if there is
    /// none. A name that is itself an existing file path wins first.
    /// </summary>
    public static string? FindBundled(string name)
    {
        if (File.Exists(name)) return Path.GetFullPath(name);
        string candidate = Path.Combine(BundledDirectory, name.EndsWith(".json", StringComparison.OrdinalIgnoreCase) ? name : name + ".json");
        if (File.Exists(candidate)) return candidate;
        string cwdCandidate = Path.Combine(Directory.GetCurrentDirectory(), "Scenes", "Json", Path.GetFileName(candidate));
        return File.Exists(cwdCandidate) ? cwdCandidate : null;
    }

    public static JsonScene Load(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Scene file not found: {path}", path);
        string json;
        try
        {
            json = File.ReadAllText(path);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            throw new IOException($"Could not read scene file '{path}': {ex.Message}", ex);
        }
        return LoadFromJson(json, Path.GetDirectoryName(Path.GetFullPath(path)) ?? Directory.GetCurrentDirectory(), path);
    }

    public static JsonScene LoadFromJson(string json, string? baseDirectory, string? sourcePath = null)
    {
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException(
                $"Invalid JSON in scene file '{sourcePath ?? "<inline>"}' (line {ex.LineNumber}, byte {ex.BytePositionInLine}): {ex.Message}", ex);
        }

        using (doc)
        {
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                throw new InvalidDataException($"Scene file '{sourcePath ?? "<inline>"}': top level must be a JSON object.");
            try
            {
                return ParseScene(root, baseDirectory ?? Directory.GetCurrentDirectory(), sourcePath);
            }
            catch (JsonException ex)
            {
                throw new InvalidDataException(
                    $"Invalid JSON in scene file '{sourcePath ?? "<inline>"}' (line {ex.LineNumber}, byte {ex.BytePositionInLine}): {ex.Message}", ex);
            }
        }
    }

    // ---------- scene ----------

    private static JsonScene ParseScene(JsonElement root, string baseDirectory, string? sourcePath)
    {
        var scene = new JsonScene
        {
            SourcePath = sourcePath,
            SceneName = OptString(root, "name"),
        };

        if (root.TryGetProperty("camera", out JsonElement camera) && camera.ValueKind == JsonValueKind.Object)
        {
            scene.Camera.Position = OptVec3(camera, "position") ?? Vector3.Zero;
            scene.Camera.Target = OptVec3(camera, "target");
        }

        var materials = new Dictionary<string, Material>(StringComparer.OrdinalIgnoreCase);
        if (root.TryGetProperty("materials", out JsonElement mats) && mats.ValueKind == JsonValueKind.Object)
        {
            foreach (JsonProperty prop in mats.EnumerateObject())
                materials[prop.Name] = ParseMaterial(prop.Value, $"materials.{prop.Name}");
        }

        if (root.TryGetProperty("environment", out JsonElement env))
            scene.Environment = ParseEnvironment(env, "environment");

        if (!root.TryGetProperty("objects", out JsonElement objects) || objects.ValueKind != JsonValueKind.Array)
            throw new InvalidDataException("Scene must contain an 'objects' array.");
        int index = 0;
        foreach (JsonElement obj in objects.EnumerateArray())
        {
            ParseObject(obj, $"objects[{index}]", materials, baseDirectory, scene);
            index++;
        }
        return scene;
    }

    private static void ParseObject(JsonElement el, string ctx, Dictionary<string, Material> materials,
        string baseDirectory, Scene scene)
    {
        if (el.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException($"{ctx}: each object must be a JSON object.");
        string type = ReqString(el, "type", ctx).ToLowerInvariant();
        Material material;
        Texture? texture = null;
        Primitive primitive;
        float? triplanarScale = null;
        Vector3? triplanarOrigin = null;

        switch (type)
        {
            case "light":
            {
                material = ParseMaterialField(el, ctx, materials);
                var center = ReqVec3(el, "center", ctx);
                float radius = OptFloat(el, "radius", float.MinValue);
                var light = new Light(center, radius, material);
                if (OptBool(el, "visible", true)) scene.AddLight(light);
                else scene.AddLighting(light);
                return;
            }
            case "rotated":
            {
                if (!el.TryGetProperty("primitive", out JsonElement inner) || inner.ValueKind != JsonValueKind.Object)
                    throw new InvalidDataException($"{ctx}: a 'rotated' object needs a 'primitive' object.");
                // A uv mapping on the rotated wrapper applies to the inner primitive,
                // because RotatedPrimitive transforms positions into local space first.
                Primitive innerPrimitive = ParsePrimitive(inner, $"{ctx}.primitive", materials, baseDirectory);
                Quaternion rotation = ParseRotation(el, ctx);
                Vector3 position = ReqVec3(el, "position", ctx);
                scene.AddObject(new RotatedPrimitive(innerPrimitive, rotation, position));
                return;
            }
            default:
                material = ParseMaterialField(el, ctx, materials);
                texture = ParseTextureField(el, ctx, baseDirectory, out triplanarScale, out triplanarOrigin);
                primitive = ParsePrimitive(el, ctx, materials, baseDirectory, material, texture);
                break;
        }

        // A plain object may also carry a rotation/position wrapper.
        if (el.TryGetProperty("rotation", out _) || el.TryGetProperty("position", out _))
        {
            Quaternion rotation = ParseRotation(el, ctx);
            Vector3 position = ReqVec3(el, "position", ctx);
            primitive = new RotatedPrimitive(primitive, rotation, position);
        }
        if (triplanarScale.HasValue && primitive is Mesh mesh)
        {
            mesh.TriplanarTextureScale = triplanarScale;
            mesh.TextureOrigin = triplanarOrigin ?? Vector3.Zero;
        }
        scene.AddObject(primitive);
    }

    private static Primitive ParsePrimitive(JsonElement el, string ctx, Dictionary<string, Material> materials,
        string baseDirectory, Material? givenMaterial = null, Texture? givenTexture = null)
    {
        string type = ReqString(el, "type", ctx).ToLowerInvariant();
        Material material = givenMaterial ?? ParseMaterialField(el, ctx, materials);
        // When called for a rotated inner primitive, material/texture come from the inner object itself.
        Texture? texture = givenTexture ?? ParseTextureField(el, ctx, baseDirectory, out _, out _);
        Primitive primitive = type switch
        {
            "sphere" => new Sphere(ReqVec3(el, "center", ctx), ReqFloat(el, "radius", ctx), material, texture),
            "box" => new Box(ReqVec3(el, "min", ctx), ReqVec3(el, "max", ctx), material, texture),
            "cylinder" => new Cylinder(ReqVec3(el, "center", ctx), ReqFloat(el, "radius", ctx), ReqFloat(el, "height", ctx), material, texture),
            "disk" => new Disk(ReqVec3(el, "center", ctx), ReqVec3(el, "normal", ctx), ReqFloat(el, "radius", ctx), material, texture),
            "plane" => new Primitives.Plane(ReqVec3(el, "normal", ctx), ReqFloat(el, "d", ctx), material, texture),
            "torus" => new Torus(material, texture, OptFloat(el, "major", 2f), OptFloat(el, "minor", 0.5f), OptVec3(el, "center") ?? Vector3.Zero),
            "triangle" => new Triangle(ReqVec3(el, "a", ctx), ReqVec3(el, "b", ctx), ReqVec3(el, "c", ctx), material, texture),
            "mesh" => ParseMesh(el, ctx, material, texture, baseDirectory),
            _ => throw new InvalidDataException($"{ctx}: unknown object type '{type}'."),
        };

        // UV mapping override, if any.
        JsonElement? uvElement = null;
        if (el.TryGetProperty("uv", out JsonElement uv) && uv.ValueKind == JsonValueKind.Object)
            uvElement = uv;
        if (primitive is Mesh mesh2 && texture is not null
            && el.TryGetProperty("texture", out JsonElement texEl) && texEl.ValueKind == JsonValueKind.Object
            && texEl.TryGetProperty("triplanarScale", out JsonElement ts) && ts.ValueKind == JsonValueKind.Number)
        {
            // Triplanar settings live on the texture object in JSON.
            mesh2.TriplanarTextureScale = ts.GetSingle();
            mesh2.TextureOrigin = OptVec3(texEl, "origin") ?? Vector3.Zero;
        }
        if (uvElement is JsonElement uvObj)
            primitive = ApplyUv(primitive, uvObj, ctx);
        return primitive;
    }

    private static Mesh ParseMesh(JsonElement el, string ctx, Material material, Texture? texture, string baseDirectory)
    {
        Mesh mesh;
        if (el.TryGetProperty("obj", out JsonElement objEl) && objEl.ValueKind == JsonValueKind.String)
        {
            string objPath = ResolveAsset(objEl.GetString()!, ctx, baseDirectory);
            bool normalize = OptBool(el, "normalize", true);
            mesh = Mesh.FromObj(objPath, material, normalize);
            // FromObj never attaches a texture; the JSON texture applies to the mesh.
            mesh.Texture = texture;
        }
        else if (el.TryGetProperty("vertices", out JsonElement verts) && verts.ValueKind == JsonValueKind.Array
            && el.TryGetProperty("triangles", out JsonElement tris) && tris.ValueKind == JsonValueKind.Array)
        {
            mesh = new Mesh(material, texture);
            foreach (JsonElement v in verts.EnumerateArray())
                mesh.Vertices.Add(ParseVec3(v, $"{ctx}.vertices[]"));
            foreach (JsonElement t in tris.EnumerateArray())
            {
                if (t.ValueKind != JsonValueKind.Array || t.GetArrayLength() != 3)
                    throw new InvalidDataException($"{ctx}.triangles[]: each triangle must be [a, b, c] vertex indices.");
                mesh.Triangles.Add((t[0].GetInt32(), t[1].GetInt32(), t[2].GetInt32()));
            }
            if (el.TryGetProperty("vertexNormals", out JsonElement normals) && normals.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement n in normals.EnumerateArray())
                    mesh.VertexNormals.Add(ParseVec3(n, $"{ctx}.vertexNormals[]"));
                if (mesh.VertexNormals.Count != mesh.Vertices.Count)
                    throw new InvalidDataException($"{ctx}: vertexNormals must have one entry per vertex.");
            }
        }
        else
        {
            throw new InvalidDataException($"{ctx}: a 'mesh' object needs either 'obj' (a path) or 'vertices' + 'triangles'.");
        }
        if (el.TryGetProperty("fit", out JsonElement fit) && fit.ValueKind == JsonValueKind.Object)
            ApplySkullFit(mesh, fit, ctx);
        if (OptBool(el, "smooth", false) || OptBool(el, "accelerate", false))
            mesh.BuildAcceleration(smoothShading: OptBool(el, "smooth", false));
        return mesh;
    }

    /// <summary>
    /// Reproduces SkullEchoScene.Skull's placement transform for a skull OBJ:
    /// rotate (yaw + 180 degrees about Y, then -90 degrees about X), scale so
    /// the rotated height equals <c>height</c>, centre it, and place it at
    /// (x, centerY ?? height/2 + 1.1, z). Normals are rotated, not scaled.
    /// </summary>
    private static void ApplySkullFit(Mesh mesh, JsonElement fit, string ctx)
    {
        float height = ReqFloat(fit, "height", ctx + ".fit");
        float yaw = OptFloat(fit, "yawDegrees", 0f) * MathF.PI / 180f;
        float x = ReqFloat(fit, "x", ctx + ".fit");
        float z = ReqFloat(fit, "z", ctx + ".fit");
        float centerY = OptFloat(fit, "centerY", float.NaN);
        Quaternion rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, yaw + MathF.PI) *
            Quaternion.CreateFromAxisAngle(Vector3.UnitX, -MathF.PI / 2);
        Vector3 min = new(float.MaxValue), max = new(float.MinValue);
        for (int i = 0; i < mesh.Vertices.Count; i++)
        {
            mesh.Vertices[i] = Vector3.Transform(mesh.Vertices[i], rotation);
            min = Vector3.Min(min, mesh.Vertices[i]);
            max = Vector3.Max(max, mesh.Vertices[i]);
        }
        if (max.Y - min.Y <= 0)
            throw new InvalidDataException($"{ctx}.fit: mesh has no height to fit.");
        float scale = height / (max.Y - min.Y);
        Vector3 center = (min + max) / 2;
        Vector3 placement = new(x, float.IsNaN(centerY) ? height / 2 + 1.1f : centerY, z);
        for (int i = 0; i < mesh.Vertices.Count; i++)
            mesh.Vertices[i] = (mesh.Vertices[i] - center) * scale + placement;
        for (int i = 0; i < mesh.VertexNormals.Count; i++)
        {
            Vector3 n = Vector3.Transform(mesh.VertexNormals[i], rotation);
            mesh.VertexNormals[i] = n.LengthSquared() > 0 ? Vector3.Normalize(n) : Vector3.Zero;
        }
    }

    private static Primitive ApplyUv(Primitive primitive, JsonElement uv, string ctx)
    {
        string mode = ReqString(uv, "mode", ctx + ".uv").ToLowerInvariant();
        return mode switch
        {
            "xzscale" => new UvMappedPrimitive(primitive, "xzScale",
                scaleX: OptFloat(uv, "scaleX", 1f), scaleZ: OptFloat(uv, "scaleZ", 1f)),
            "facade" => new UvMappedPrimitive(primitive, "facade",
                repeat: OptFloat(uv, "repeat", 4f)),
            "planemapped" => new UvMappedPrimitive(primitive, "planeMapped",
                offsetX: OptFloat(uv, "offsetX", 0f), width: ReqFloat(uv, "width", ctx + ".uv"),
                offsetY: OptFloat(uv, "offsetY", 0f), height: ReqFloat(uv, "height", ctx + ".uv"),
                wrap: OptBool(uv, "wrap", false)),
            "spherical" => new UvMappedPrimitive(primitive, "spherical",
                center: ReqVec3(uv, "center", ctx + ".uv"), size: ReqFloat(uv, "size", ctx + ".uv")),
            _ => throw new InvalidDataException($"{ctx}.uv: unknown uv mode '{mode}'."),
        };
    }

    private static Quaternion ParseRotation(JsonElement el, string ctx)
    {
        if (el.TryGetProperty("quaternion", out JsonElement q))
        {
            if (q.ValueKind != JsonValueKind.Array || q.GetArrayLength() != 4)
                throw new InvalidDataException($"{ctx}.quaternion: expected [x, y, z, w].");
            var quat = new Quaternion(ReqFloat(q, 0, ctx + ".quaternion"), ReqFloat(q, 1, ctx + ".quaternion"),
                ReqFloat(q, 2, ctx + ".quaternion"), ReqFloat(q, 3, ctx + ".quaternion"));
            if (quat.LengthSquared() == 0)
                throw new InvalidDataException($"{ctx}.quaternion: rotation must be nonzero.");
            return Quaternion.Normalize(quat);
        }
        if (el.TryGetProperty("yawPitchRoll", out JsonElement ypr))
        {
            Vector3 v = ParseVec3(ypr, ctx + ".yawPitchRoll");
            return Quaternion.CreateFromYawPitchRoll(v.X, v.Y, v.Z);
        }
        throw new InvalidDataException($"{ctx}: a rotated object needs 'quaternion' [x, y, z, w] or 'yawPitchRoll' [yaw, pitch, roll] (radians).");
    }

    // ---------- materials / textures ----------

    private static Material ParseMaterialField(JsonElement el, string ctx, Dictionary<string, Material> materials)
    {
        if (!el.TryGetProperty("material", out JsonElement m))
            throw new InvalidDataException($"{ctx}: missing 'material' (an object or a name from 'materials').");
        return ParseMaterial(m, ctx + ".material", materials);
    }

    private static Material ParseMaterial(JsonElement el, string ctx, Dictionary<string, Material>? materials = null)
    {
        if (el.ValueKind == JsonValueKind.String)
        {
            string name = el.GetString()!;
            if (materials is not null && materials.TryGetValue(name, out Material? found)) return found;
            throw new InvalidDataException($"{ctx}: unknown material '{name}' (not in 'materials').");
        }
        if (el.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException($"{ctx}: material must be an object or a material name.");
        Vector3 color = el.TryGetProperty("color", out JsonElement c) ? ParseVec3(c, ctx + ".color", allowScalar: true) : Vector3.One;
        var material = new Material(color,
            OptFloat(el, "diffuse", 0f), OptFloat(el, "reflection", 0f), OptFloat(el, "specular", 0f));
        if (el.TryGetProperty("shininess", out JsonElement s) && s.ValueKind == JsonValueKind.Number)
            material.Shininess = s.GetSingle();
        material.Emission = OptFloat(el, "emission", 0f);
        material.Ior = OptFloat(el, "ior", 1f);
        material.Transmission = OptFloat(el, "transmission", 0f);
        return material;
    }

    private static Texture? ParseTextureField(JsonElement el, string ctx, string baseDirectory,
        out float? triplanarScale, out Vector3? triplanarOrigin)
    {
        triplanarScale = null;
        triplanarOrigin = null;
        if (!el.TryGetProperty("texture", out JsonElement t) || t.ValueKind == JsonValueKind.Null)
            return null;
        string path;
        if (t.ValueKind == JsonValueKind.String)
        {
            path = t.GetString()!;
        }
        else if (t.ValueKind == JsonValueKind.Object)
        {
            path = ReqString(t, "path", ctx + ".texture");
            if (t.TryGetProperty("triplanarScale", out JsonElement ts) && ts.ValueKind == JsonValueKind.Number)
            {
                triplanarScale = ts.GetSingle();
                triplanarOrigin = OptVec3(t, "origin") ?? Vector3.Zero;
            }
        }
        else
        {
            throw new InvalidDataException($"{ctx}.texture: expected a path string or an object with 'path'.");
        }
        return new Texture(ResolveAsset(path, ctx + ".texture", baseDirectory));
    }

    private static string ResolveAsset(string path, string ctx, string baseDirectory)
    {
        var tried = new List<string>();
        if (Path.IsPathRooted(path))
        {
            tried.Add(path);
        }
        else
        {
            tried.Add(Path.GetFullPath(Path.Combine(baseDirectory, path)));
            tried.Add(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path)));
            tried.Add(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), path)));
        }
        foreach (string candidate in tried)
            if (File.Exists(candidate)) return candidate;
        throw new FileNotFoundException(
            $"{ctx}: asset not found: '{path}' (looked in: {string.Join("; ", tried)})", path);
    }

    // ---------- environments ----------

    private static Func<Vector3, Vector3> ParseEnvironment(JsonElement el, string ctx)
    {
        if (el.ValueKind == JsonValueKind.String)
        {
            return el.GetString()!.ToLowerInvariant() switch
            {
                "nightsky" => NightSkyEnvironment(null, null, null, null, null, null, null, null),
                _ => throw new InvalidDataException($"{ctx}: unknown environment '{el.GetString()}' (use an object with 'type', or 'nightSky')."),
            };
        }
        if (el.ValueKind != JsonValueKind.Object)
            throw new InvalidDataException($"{ctx}: environment must be an object with 'type', or the string 'nightSky'.");
        string type = ReqString(el, "type", ctx).ToLowerInvariant();
        if (type == "solid")
        {
            // Parse eagerly: the returned lambda runs after the JsonDocument
            // is disposed, so it must not capture the JsonElement itself.
            Vector3 solidColor = ReqVec3(el, "color", ctx);
            return _ => solidColor;
        }
        return type switch
        {
            "studio" => StudioEnvironment(
                ReqVec3(el, "dark", ctx), ReqVec3(el, "light", ctx),
                OptFloat(el, "yFactor", 0f), ParseApertures(el, ctx)),
            "nightsky" => NightSkyEnvironment(
                OptVec3(el, "dark"), OptVec3(el, "light"),
                OptVec3(el, "haloDirection"), OptVec3(el, "haloColor"),
                OptFloat(el, "haloPower", 14f),
                OptVec3(el, "tealDirection"), OptVec3(el, "tealColor"),
                OptFloat(el, "tealPower", 10f)),
            _ => throw new InvalidDataException($"{ctx}: unknown environment type '{type}' (solid, studio, nightSky)."),
        };
    }

    private static List<(Vector3 Center, Vector3 Size, Vector3 Color)> ParseApertures(JsonElement el, string ctx)
    {
        var list = new List<(Vector3, Vector3, Vector3)>();
        if (el.TryGetProperty("apertures", out JsonElement arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement a in arr.EnumerateArray())
                list.Add((ReqVec2(a, "center", ctx + ".apertures[]"), ReqVec2(a, "size", ctx + ".apertures[]"),
                    ReqVec3(a, "color", ctx + ".apertures[]")));
        }
        return list;
    }

    /// <summary>Studio backdrop: a vertical gradient plus soft rectangular apertures behind the camera.</summary>
    private static Func<Vector3, Vector3> StudioEnvironment(Vector3 dark, Vector3 light, float yFactor,
        List<(Vector3 Center, Vector3 Size, Vector3 Color)> apertures)
    {
        return direction =>
        {
            Vector3 d = Vector3.Normalize(direction);
            float paper = System.Math.Clamp((d.Z + .35f) / 1.2f, 0, 1);
            Vector3 tone = Vector3.Lerp(dark, light, paper);
            tone *= 1 + yFactor * d.Y;
            if (d.Z < -.08f)
            {
                float u = d.X / -d.Z, v = d.Y / -d.Z;
                foreach (var (center, size, color) in apertures)
                {
                    float aperture = MathF.Exp(-MathF.Pow(MathF.Abs((u - center.X) / size.X), 8)
                        - MathF.Pow(MathF.Abs((v - center.Y) / size.Y), 8));
                    tone = Vector3.Lerp(tone, color, aperture);
                }
            }
            return tone;
        };
    }

    /// <summary>Dex's night sky (defaults): indigo gradient, a warm halo and a faint teal glow.</summary>
    private static Func<Vector3, Vector3> NightSkyEnvironment(
        Vector3? dark, Vector3? light,
        Vector3? haloDirection, Vector3? haloColor, float? haloPower,
        Vector3? tealDirection, Vector3? tealColor, float? tealPower)
    {
        Vector3 darkC = dark ?? new Vector3(0.008f, 0.012f, 0.045f);
        Vector3 lightC = light ?? new Vector3(0.05f, 0.06f, 0.20f);
        Vector3 haloDir = Vector3.Normalize(haloDirection ?? new Vector3(0f, 0.18f, 1f));
        Vector3 haloCol = haloColor ?? new Vector3(0.16f, 0.09f, 0.02f);
        float haloPow = haloPower ?? 14f;
        Vector3 tealDir = Vector3.Normalize(tealDirection ?? new Vector3(0.75f, -0.45f, 1f));
        Vector3 tealCol = tealColor ?? new Vector3(0.01f, 0.07f, 0.07f);
        float tealPow = tealPower ?? 10f;
        return direction =>
        {
            Vector3 d = Vector3.Normalize(direction);
            float t = System.Math.Clamp(d.Y * 0.5f + 0.5f, 0f, 1f);
            Vector3 tone = Vector3.Lerp(darkC, lightC, t);
            if (d.Z > 0f)
            {
                float g = MathF.Pow(MathF.Max(Vector3.Dot(d, haloDir), 0f), haloPow);
                tone += g * haloCol;
                float tg = MathF.Pow(MathF.Max(Vector3.Dot(d, tealDir), 0f), tealPow);
                tone += tg * tealCol;
            }
            return tone;
        };
    }

    // ---------- JSON helpers ----------

    private static string ReqString(JsonElement el, string name, string ctx)
    {
        if (el.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.String)
            return v.GetString()!;
        throw new InvalidDataException($"{ctx}: missing or non-string '{name}'.");
    }

    private static string? OptString(JsonElement el, string name)
        => el.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.String ? v.GetString() : null;

    private static float ReqFloat(JsonElement el, string name, string ctx)
    {
        if (el.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.Number)
            return v.GetSingle();
        throw new InvalidDataException($"{ctx}: missing or non-numeric '{name}'.");
    }

    private static float ReqFloat(JsonElement el, int index, string ctx)
    {
        if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() > index && el[index].ValueKind == JsonValueKind.Number)
            return el[index].GetSingle();
        throw new InvalidDataException($"{ctx}: expected a number at index {index}.");
    }

    private static float OptFloat(JsonElement el, string name, float fallback)
        => el.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.Number ? v.GetSingle() : fallback;

    private static bool OptBool(JsonElement el, string name, bool fallback)
        => el.TryGetProperty(name, out JsonElement v) && (v.ValueKind == JsonValueKind.True || v.ValueKind == JsonValueKind.False)
            ? v.GetBoolean() : fallback;

    /// <summary>Parses a 2-element [x, y] array, stored as a Vector3 with Z = 0.</summary>
    private static Vector3 ReqVec2(JsonElement el, string name, string ctx)
    {
        if (el.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.Array && v.GetArrayLength() == 2)
        {
            float x = ReqFloat(v, 0, ctx + "." + name);
            float y = ReqFloat(v, 1, ctx + "." + name);
            return new Vector3(x, y, 0);
        }
        throw new InvalidDataException($"{ctx}.{name}: expected [x, y].");
    }

    private static Vector3 ReqVec3(JsonElement el, string name, string ctx)
    {
        if (!el.TryGetProperty(name, out JsonElement v))
            throw new InvalidDataException($"{ctx}: missing '{name}' (expected [x, y, z]).");
        return ParseVec3(v, $"{ctx}.{name}");
    }

    private static Vector3? OptVec3(JsonElement el, string name)
        => el.TryGetProperty(name, out JsonElement v) && v.ValueKind == JsonValueKind.Array ? ParseVec3(v, name) : null;

    private static Vector3 ParseVec3(JsonElement el, string ctx, bool allowScalar = false)
    {
        if (allowScalar && el.ValueKind == JsonValueKind.Number)
        {
            float s = el.GetSingle();
            return new Vector3(s);
        }
        if (el.ValueKind != JsonValueKind.Array || el.GetArrayLength() != 3)
            throw new InvalidDataException($"{ctx}: expected [x, y, z].");
        if (el[0].ValueKind != JsonValueKind.Number || el[1].ValueKind != JsonValueKind.Number || el[2].ValueKind != JsonValueKind.Number)
            throw new InvalidDataException($"{ctx}: expected [x, y, z] numbers.");
        return new Vector3(el[0].GetSingle(), el[1].GetSingle(), el[2].GetSingle());
    }
}
