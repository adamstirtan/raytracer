# JSON scene format (v1)

Scenes can be written as JSON files instead of C# classes. The loader is
`RayTracer.Core/Scenes/JsonSceneLoader.cs`; bundled scenes live in
`RayTracer.Core/Scenes/Json/` and are copied to the CLI output directory
(`Scenes/Json/`) at build time.

Render one:

```sh
# A bundled scene, by file:
dotnet RayTracer.Cli.dll --scene-file RayTracer.Core/Scenes/Json/sphere.json --width 800 --height 600 --out render.png
# A bundled scene, by name (json: prefix), from the CLI output directory:
dotnet RayTracer.Cli.dll --scene json:sphere --width 800 --height 600 --out render.png
# A scene with no code-scene namesake (crystal-orchard, reflective-garden,
# textured-sphere) also works with a plain --scene <name>.
```

`--cam-pos` / `--cam-target` still override the JSON camera, and `--depth`,
`--spp`, `--width`, `--height` and `--diptych` work as with code scenes.
JSON scenes are static: `--frames > 1` is rejected (use the code scenes for
animated sequences).

## Top level

```json
{
  "format": "raytracer-scene",
  "version": 1,
  "name": "my-scene",
  "camera": { "position": [0, 0, -5], "target": [0, 1.2, 0] },
  "materials": { "red": { "color": [0.9, 0.08, 0.05], "diffuse": 0.8, "reflection": 0.55, "specular": 0.9 } },
  "environment": { "type": "solid", "color": [0.02, 0.02, 0.05] },
  "objects": [ ... ]
}
```

- `format` / `version`: accepted but not enforced beyond being present in
  bundled files; the loader does not reject other values.
- `camera.position`: required for a useful render (defaults to the origin).
  `camera.target`: optional; when absent the engine looks at the origin.
- `materials`: optional named materials, referenced by name from objects.
- `environment`: optional. See below. When absent, the background is black.
- `objects`: required, in render order. Unknown object types are errors —
  nothing is silently skipped.

Vectors are `[x, y, z]` arrays of numbers. A material `color` may also be a
single number, meaning a grey `(v, v, v)`.

## Materials

```json
{ "color": [0.9, 0.08, 0.05], "diffuse": 0.8, "reflection": 0.55, "specular": 0.9, "shininess": 80, "emission": 0.07, "ior": 1.5, "transmission": 1 }
```

All fields except `color` are optional and default to 0; `shininess` defaults
to "unspecified" (the engine then falls back to `specular` as the highlight
exponent, exactly as the `Material` class does). `ior` defaults to 1 (air)
and `transmission` to 0 (opaque); a material with `transmission > 0` refracts
light by Snell's law with that index of refraction, blending reflection and
refraction by Schlick's Fresnel approximation, and it does not cast a shadow
(true caustics are not modelled). An object may give its
material inline, or as a string naming an entry in `materials`.

## Textures

A texture is a path string, or an object for mesh triplanar mapping:

```json
"texture": "Textures/Billiards/felt.png"
"texture": { "path": "Textures/Skull/bone-albedo.png", "triplanarScale": 0.125, "origin": [18.7, 0.4, 6] }
```

Paths are resolved in order: relative to the JSON file's directory, then to
the application directory (where the build copies `Textures/` and `assets/`),
then to the current directory. A missing file is a hard error naming every
location tried. Textures are loaded with ImageSharp, exactly as in code.

## Objects

Every object has `type`, `material`, and optionally `texture` (and `uv`,
below). Types and their extra fields:

| type | fields |
|---|---|
| `sphere` | `center`, `radius` |
| `box` | `min`, `max` |
| `cylinder` | `center`, `radius`, `height` |
| `disk` | `center`, `normal`, `radius` |
| `plane` | `normal`, `d` (plane equation `dot(normal, p) + d = 0`) |
| `torus` | `center` (default origin), `major` (default 2), `minor` (default 0.5) |
| `triangle` | `a`, `b`, `c` |
| `mesh` | see below |
| `light` | `center`, `radius` (default: a point light), `visible` (default true) |
| `rotated` | `primitive` (a nested object), `position`, and `quaternion` `[x, y, z, w]` or `yawPitchRoll` `[yaw, pitch, roll]` in radians |

Any non-`rotated` object may also carry `position` plus `quaternion` /
`yawPitchRoll` to wrap it in a rotation, as a shorthand for `rotated`.

Lights: `visible: true` (the default) is `AddLight` — the light is also a
visible sphere in the scene. `visible: false` is `AddLighting` — it lights
the scene but has no visible geometry. This distinction matters: Dex's two
lights and all skull-studio lights are lighting-only.

### Meshes

```json
{ "type": "mesh", "obj": "assets/skull.obj", "normalize": false, "material": "bone", "smooth": true, "accelerate": true }
```

- `obj`: an OBJ file, loaded exactly like `Mesh.FromObj` (`normalize`
  defaults to true, which centres/scales/rotates the model the way that
  method does). `smooth` / `accelerate` call `BuildAcceleration`
  (smooth shading on / off).
- Inline geometry instead of a file:

```json
{ "type": "mesh", "vertices": [[0, 0, 0], [1, 0, 0], [0, 1, 0]],
  "triangles": [[0, 1, 2]], "vertexNormals": [[0, 0, -1], [0, 0, -1], [0, 0, -1]],
  "material": "rock", "smooth": true, "accelerate": true }
```

`vertexNormals` is optional; when present it must have one entry per vertex.

- `fit`: the skull-placement transform used by the bundled skull scenes,
  reproducing `SkullEchoScene.Skull` for an OBJ loaded with
  `normalize: false`:

```json
{ "type": "mesh", "obj": "assets/skull.obj", "normalize": false,
  "fit": { "height": 20.5, "yawDegrees": 65, "x": 18.7, "z": 6, "centerY": 0.4 },
  "material": "pearl", "smooth": true, "accelerate": true }
```

It rotates (yaw + 180° about Y, then −90° about X), scales the rotated
mesh so its height equals `height`, centres it, and places it at
`(x, centerY ?? height/2 + 1.1, z)`.

### UV overrides

Some code scenes override a primitive's UV mapping. A JSON object can carry
a `uv` object to reproduce those, implemented by `UvMappedPrimitive`
(which delegates intersection/normals and only replaces `GetUV`):

| mode | fields | reproduces |
|---|---|---|
| `xzScale` | `scaleX`, `scaleZ` | Billiards felt bed (`1.4, 1.4`); FutureCity ground (`0.125, 0.125`, i.e. ÷8) |
| `facade` | `repeat` (default 4) | FutureCity facade boxes |
| `planeMapped` | `offsetX`, `width`, `offsetY`, `height`, `wrap` (default false; false clamps v to [.001, .999]) | FutureCity sky plane; Orbital star backdrop (wrapped) |
| `spherical` | `center`, `size` | Orbital asteroids |

```json
{ "type": "box", "min": [-3, -0.2, -6], "max": [3, 0, 6], "material": "cloth",
  "texture": "Textures/Billiards/felt.png",
  "uv": { "mode": "xzScale", "scaleX": 1.4, "scaleZ": 1.4 } }
```

## Environments

```json
{ "type": "solid", "color": [0.02, 0.02, 0.05] }
```

```json
{ "type": "studio", "dark": [0.105, 0.11, 0.115], "light": [0.47, 0.455, 0.425],
  "yFactor": 0.045,
  "apertures": [{ "center": [0, 1.1], "size": [1.35, 0.5], "color": [0.8, 0.79, 0.76] }] }
```

A studio environment is the skull scenes' backdrop: a vertical gradient
from `dark` to `light` by `clamp((d.z + 0.35) / 1.2)`, scaled by
`1 + yFactor * d.y`, plus, for directions with `d.z < -0.08`, one soft
aperture per entry: `exp(-|u/size.x|^8 - |v/size.y|^8)` with
`u = d.x / -d.z`, `v = d.y / -d.z`, lerped toward the aperture colour.
The three bundled studio variants (bone, chromatic, dialogue) are exactly
this function with different parameters — dialogue's two apertures are
just two entries.

```json
"environment": "nightSky"
```

Dex's night sky, with overridable parameters via the object form
(`dark`, `light`, `haloDirection`, `haloColor`, `haloPower`,
`tealDirection`, `tealColor`, `tealPower`; defaults are Dex's values).

## Complete example

```json
{
  "format": "raytracer-scene",
  "version": 1,
  "name": "sphere",
  "camera": { "position": [0, 0, -5], "target": [0, 1.2, 0] },
  "objects": [
    { "type": "plane", "normal": [0, 1, 0], "d": 0,
      "material": { "color": [0.15, 0.15, 0.15], "diffuse": 0.85, "reflection": 0.25, "specular": 0.15 } },
    { "type": "sphere", "center": [0, 1.5, 0], "radius": 1.5,
      "material": { "color": [0.9, 0.08, 0.05], "diffuse": 0.8, "reflection": 0.55, "specular": 0.9 } },
    { "type": "light", "center": [5, 8, -3],
      "material": { "color": [1, 0.95, 0.88] } }
  ]
}
```

## What JSON cannot express

- **Animation and simulation.** Synthwave's animated valley/sky and the
  falling-spheres physics are code. `falling-spheres.json` and
  `impact.json` are frozen snapshots: falling-spheres at time 0 (the
  spheres in their starting grid, before any physics step) and impact at
  its default 3.15 s. Rendering them with `--frames` is rejected; use the
  code scenes (and `--time` for impact) for anything else.
- **Custom primitives.** Synthwave's valley is a bespoke primitive with
  its own ray traversal, so `synthwave` / `synthwave-distant` have no
  JSON file at all and stay code-only.
- **Custom mesh surgery.** SkullChromatic's polished brow facets are made
  by subdividing and re-partitioning the skull mesh in code
  (`PolishBrow`); that algorithm is not a JSON construct, so
  `skull-chromatic` stays code-only. The other skull scenes use plain
  `obj` + `fit` and are JSON.
- **MeshScene's fallback.** The code scene loads `skull.obj` if present,
  else `teapot.obj`. `mesh.json` pins `skull.obj`; if it is missing the
  loader errors instead of silently switching models.
- **Procedural code in general.** A JSON scene is a fixed list of objects.
  Anything computed at render time (a seeded random garden is fine — it was
  expanded to explicit objects when the JSON was generated) must be
  expanded first. The exporter that produced the bundled files lives in
  `~/workspace/raytracer-json-work/exporter/` (not in this repo).
