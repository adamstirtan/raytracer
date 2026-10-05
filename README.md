<div align="center">

# Ray Tracer

**From a university assignment to a C# renderer, one ray at a time.**

[![Build and tests](https://github.com/adamstirtan/raytracer/actions/workflows/build.yml/badge.svg)](https://github.com/adamstirtan/raytracer/actions/workflows/build.yml)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)
![CPU rendering](https://img.shields.io/badge/rendering-CPU-222222)

<img src="assets/synthwave.png" alt="Synthwave landscape with a luminous blue grid following jagged valley walls toward a giant pink sun beneath a purple star-filled sky" width="1280" />

*Synthwave — a neon grid valley, jagged terrain, and a giant pink sun beneath a purple star field. Rendered at 1920 × 1110 with 16 samples per pixel.*

<a href="assets/synthwave_driving_loop.mp4"><img src="assets/synthwave_driving_loop.gif" alt="Animated synthwave drive with a rushing blue foreground grid, rigid distant mountains, a stationary pink planet, and drifting stars" width="1280" /></a>

*Synthwave Drive — a seamless ten-second loop with fast foreground motion, huge distant mountains, and drifting stars. [Download the 1080p, 30 fps MP4](assets/synthwave_driving_loop.mp4).*

<img src="assets/future_city_textured.png" alt="Textured futuristic spaceport with metal-clad towers, patterned windows, a suspended reactor halo, segmented glass domes, and reflections on a tiled plaza beneath a dusk sky" width="1280" />

*Future City — metal cladding, patterned windows, ceramic panels, and segmented glass domes around a suspended reactor halo, reflected in a tiled plaza beneath a dusk sky.*

<img src="assets/orbital_textured.png" alt="Textured orbital station and docking ship beside a gas giant with swirling cloud bands and a copper storm, a cratered moon, and an asteroid belt against a faint nebula" width="1280" />

*Orbital — detailed hull panels and solar cells beside a swirling gas giant, a cratered moon, and 180 textured asteroids beneath a subtle nebula.*

Future City and Orbital are rendered at 1600 × 1000 with nine samples per pixel, cast shadows, and recursive reflections. Future City uses reflection depth four; Orbital uses depth three.

<img src="assets/observatory.png" alt="Alien observatory with tilted golden orbital rings around a blue orb, terraced platforms, angular mountains, and a planet reflected in a mirror lake" width="1280" />

*Observatory — intersecting golden rings above a mirror lake, framed by mountain silhouettes and a blue planet. Rendered at 1280 × 900 with nine samples per pixel and reflection depth four.*

<img src="assets/chromatic_spheres.png" alt="Garden of 360 separated polished spheres in teal, gold, orange, blue, magenta, and chrome, with suspended spirals and reflections in a dark mirror floor" width="1280" />

*Chromatic — 360 non-overlapping spheres, suspended spirals, and a chrome centerpiece on a mirror floor. Rendered at 1920 × 1080 with nine samples per pixel and reflection depth five.*

<a href="assets/falling_spheres_physics.mp4"><img src="assets/falling_spheres_physics.gif" alt="Ray-traced animation of colorful reflective spheres falling from above, bouncing on a mirror floor, colliding, and scattering" width="1280" /></a>

*Falling Spheres — 240 polished balls under Earth gravity, with collisions, friction, and diminishing bounces. Every frame is ray traced. [Download the ten-second 1080p, 30 fps MP4](assets/falling_spheres_physics.mp4).*

<img src="assets/billiards_textured.png" alt="Pool table with green felt, walnut rails, six pockets, a cue ball, and a rack of numbered billiards balls" width="1280" />

*Billiards — woven felt, walnut grain, and glossy numbered solids and stripes on a six-pocket table.*

<img src="assets/billiards_detail.png" alt="Close-up of the billiards rack showing felt weave, colored stripes, number medallions, glossy highlights, and cast shadows" width="1280" />

*A closer look at the UV textures and polished ball finishes. Both views use nine samples per pixel and reflection depth three.*

[How it works](#how-it-works) · [The rendering loop](#the-rendering-loop) · [Inside the project](#inside-the-project)

</div>

This project began as a university assignment and has grown through years of returning to the same question: **how do geometry and light become an image?** Today it is a CPU ray tracer written in C# on .NET 10, with imported meshes, textured materials, cast shadows, and recursive reflections.

## Light, surfaces, and a little recursion

<table>
<tr>
<td colspan="2"><img src="assets/skull_face.png" alt="Ray-traced skull mesh with warm lighting, detailed teeth, and dark eye sockets" width="960" /></td>
</tr>
<tr>
<td colspan="2"><strong>Imported geometry</strong><br />An OBJ skull, rendered by tracing rays through its triangles and shading the closest visible surface.</td>
</tr>
<tr>
<td width="50%"><img src="assets/sphere_shadows.png" alt="Red and blue spheres casting overlapping shadows on a reflective floor" width="480" /></td>
<td width="50%"><img src="assets/box_lighting.png" alt="Blue box with distinct lighting on each visible face and a floor reflection" width="480" /></td>
</tr>
<tr>
<td><strong>Shadows & reflections</strong><br />Visibility rays determine which lights reach a surface. Reflected rays reveal the surrounding scene.</td>
<td><strong>Surface normals</strong><br />Each face's orientation controls how much light it receives. The same geometry appears in the reflective floor.</td>
</tr>
</table>

| Part of the renderer | What it contributes |
| :--- | :--- |
| **Geometry** | Spheres, planes, triangles, boxes, capped cylinders, disks, and ray-marched tori |
| **OBJ meshes** | Polygon triangulation and the nearest triangle intersection for each ray |
| **Materials** | Surface color, UV textures, diffuse lighting, specular highlights, and reflection strength |
| **Camera** | Perspective projection, look-at targets, and runtime position controls |
| **Sampling** | A regular subpixel grid—1, 4, 9, or more square-count samples—to smooth edges |
| **Execution** | Parallel image rows and immutable intersection results carrying each hit's normal |

## How it works

A ray starts at the camera and passes through a sample within a pixel. The renderer finds the nearest surface along that ray, calculates its lighting, and follows additional rays where needed.

```mermaid
flowchart LR
    A[Camera ray] --> B{Nearest surface?}
    B -->|Miss| C[Black background]
    B -->|Hit| D[Color or UV texture]
    D --> E[Shadow rays toward lights]
    E --> F[Visible diffuse and specular light]
    F --> G{Reflective surface?}
    G -->|Yes| H[Trace reflected ray]
    H --> I[Combine contributions]
    G -->|No| I
    I --> J[Average pixel samples]
```

Three ray types do the work:

- **Camera rays** answer “what surface is visible at this pixel?”
- **Shadow rays** answer “can this surface see the light?” A blocker counts only when it lies before the light.
- **Reflection rays** answer “what is visible in the mirror direction?” They repeat the same tracing process up to a depth limit.

## The rendering loop

The following pseudocode summarizes the renderer. Small offsets keep secondary rays from immediately intersecting the surface they just left.

```text
function render(scene, camera, samplesPerPixel, maxDepth):
    basis = lookAt(camera.position, camera.target)
    # If position equals target, use the +Z direction.

    parallel for each image row:
        for each pixel in row:
            colors = []

            for each sample in a regular grid within pixel:
                ray = perspectiveRay(camera.position, basis, sample)
                colors.append(trace(scene, ray, depth = 1, maxDepth))

            image[pixel] = clamp(average(colors), 0, 1)

    return image
```

The tracing function turns one intersection into a color:

```text
function trace(scene, ray, depth, maxDepth):
    if depth > maxDepth:
        return BLACK

    hit = nearestIntersection(scene, ray)
    if hit does not exist:
        return BLACK
    if hit.object is a light:
        return hit.object.color

    normal = hit.normal
    if ray started inside the sphere:
        normal = -normal            # Shade the interior-facing surface.

    baseColor = sampleTexture(hit.uv) if textured else hit.material.color
    color = BLACK

    for each light in scene:
        direction = normalize(light.position - hit.position)
        shadowRay = rayFrom(hit.position + direction * epsilon, direction)
        blocked = any non-light object intersects shadowRay before light

        if not blocked:
            color += diffuse(normal, direction, baseColor, light)
            color += specular(normal, direction, ray, light)

    if hit.material.reflection > 0 and depth < maxDepth:
        direction = reflect(ray.direction, normal)
        reflectedRay = rayFrom(hit.position + direction * epsilon, direction)
        color += hit.material.reflection * trace(scene, reflectedRay, depth + 1, maxDepth)

    return color
```

The closest-hit rule is essential for meshes: a triangle encountered first in the OBJ file may sit behind another triangle. Each ray searches the mesh for the smallest valid positive intersection distance and keeps that triangle's normal with the result.

## Inside the project

| Project | Responsibility |
| :--- | :--- |
| [RayTracer.Core](RayTracer.Core) | Geometry, materials, cameras, scenes, and the rendering engine |
| [RayTracer.Cli](RayTracer.Cli) | Scene selection, camera and sampling options, and PNG output |
| [RayTracer.Core.Tests](RayTracer.Core.Tests) | Regression tests for intersections, normals, shadows, camera targets, and small renders |
| [assets](assets) | OBJ models and example renders |

The showcases use the CLI's `synthwave`, `future-city`, `orbital`, `observatory`, `chromatic`, and `billiards` scenes, defined in [SynthwaveScene.cs](RayTracer.Core/Scenes/SynthwaveScene.cs), [FutureCityScene.cs](RayTracer.Core/Scenes/FutureCityScene.cs), [OrbitalScene.cs](RayTracer.Core/Scenes/OrbitalScene.cs), [ObservatoryScene.cs](RayTracer.Core/Scenes/ObservatoryScene.cs), [ChromaticScene.cs](RayTracer.Core/Scenes/ChromaticScene.cs), and [BilliardsScene.cs](RayTracer.Core/Scenes/BilliardsScene.cs). The skull uses `mesh`. Other scenes include `sphere`, `triangle`, `box`, `cylinder`, `disk`, `hand`, `torus`, and `reflective`.

The animated showcase uses `synthwave-distant`. Render its 300-frame sequence with `--width 1920 --height 1080 --spp 9 --depth 1 --frames 300 --fps 30 --out frames`, then encode it with [encode_synthwave_loop.py](assets/texture-tools/encode_synthwave_loop.py). The encoder uses FFmpeg or the `imageio-ffmpeg` Python package. Terrain and sky return to their initial state after each cycle, with no fade or duplicate endpoint frame.

The physics animation uses `falling-spheres`, defined in [FallingSpheresScene.cs](RayTracer.Core/Scenes/FallingSpheresScene.cs). [SphereDynamics.cs](RayTracer.Core/Physics/SphereDynamics.cs) advances the simulation at 240 steps per second with gravity of 9.80665 m/s², mass based on sphere volume, collision impulses, friction, and restitution. Its 300 frames use four samples per pixel and reflection depth three; the same encoder assembles them into a ten-second video.

This is a classic direct-light ray tracer with recursive mirror reflections. Mesh intersections currently check every triangle; a spatial acceleration structure is a natural next step for larger models.
