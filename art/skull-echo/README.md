# Memento / Echo

A dark skull still life made from the repository's detailed `assets/skull.obj` mesh. Warm ivory and smoked metal sit on slate plinths above an obsidian mirror floor. A muted brass-framed mirror in the left panel reveals the metal skull from the right and a profile of the ivory skull. The metal skull also reflects the ivory skull across the gap. Lighting uses restrained warm and cool neutral tones.

The two visible prints measure **31 × 41 cm** each, with **21.59 cm (8.5 inches) between their visible image openings**. One camera views the entire scene; the two portrait crops omit the physical gap but retain all geometry for shadows and reflections. The gap already includes the mats and wall space.

Reproduce the approximately 300 DPI prints from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene skull-echo --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 9 --depth 6 --out art/skull-echo/print
python3 art/conversation/proof.py art/skull-echo/print --print-dpi
```

Print `print/left.png` and `print/right.png` at 31 × 41 cm, without automatic cropping, borders, or colour enhancement. Both include an sRGB profile and 300 DPI metadata. `print/wall-proof.jpg` illustrates the pair in the supplied 41 × 51 cm outer frames; it is not a print file. `layout.json` records the camera and crop settings.

## Mesh rendering

This scene loads the OBJ with `normalize: false`, then transforms its vertices and normals into the intended pose. `Mesh.BuildAcceleration(smoothShading: true)` snapshots the finished geometry into an immutable, median-split bounding-volume hierarchy. Rays visit only relevant triangle groups, and barycentric vertex-normal interpolation smooths the skull surfaces. Rebuild the acceleration structure after changing vertices, faces, or normals.

Existing OBJ loading defaults and older mesh scenes retain their previous transforms and flat shading. Tests compare accelerated intersections with brute-force results under concurrent rays, different ray directions, distance limits, and edited geometry. Separate render checks verify that each opposite skull appears through reflected rays.

The NuGet audit option only works around existing ImageSharp dependency advisories during restore; no dependency configuration has changed.
