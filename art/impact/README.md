# Impact

A frozen frame of the existing 240-body falling-sphere simulation, captured **3.15 seconds** after release. Sphere positions, radii, velocities, gravity, collisions, friction, and restitution are unchanged from `FallingSpheresScene`. Only the materials, studio lighting, and camera differ. Cyan, magenta, violet, peach, blue, and chrome give the snapshot its synthwave palette.

The left and right crops share one camera and one simulation instant. Each visible print measures **31 × 41 cm**, separated by **21.59 cm (8.5 inches) between visible image openings**. The omitted strip already includes the mats and wall space. All 240 bodies remain available for shadows and reflections, including those outside the printed crops.

Reproduce the approximately 300 DPI prints from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene impact --time 3.15 --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 9 --depth 6 --out art/impact/print
python3 art/conversation/proof.py art/impact/print --print-dpi
```

Print `print/left.png` and `print/right.png` at 31 × 41 cm, without automatic cropping, borders, or colour enhancement. Both PNGs include an sRGB profile and 300 DPI metadata. `print/wall-proof.jpg` is a framed placement illustration using the supplied 41 × 51 cm frame dimensions, not a print file.

The `studies` folder and `contact-sheet.jpg` compare 2.55, 2.85, 3.15, and 3.45 seconds with the initial camera. The selected `preview` and `print` use a slightly wider camera view. Their `layout.json` files record the exact simulation time and camera settings.

`--time` also supports single-frame `falling-spheres` renders. The NuGet audit option only works around existing ImageSharp dependency advisories during restore; no dependency configuration is changed.
