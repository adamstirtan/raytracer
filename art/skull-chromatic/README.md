# Chromatic Dialogue

A color variation of *Dialogue*: copper orange and cobalt blue skulls face inward in the same warm, neutral studio. The large sculptural forms and measured lighting carry forward the Irving Penn still-life inspiration; the contrasting colors make the two sculptures and their exchanged reflections easy to distinguish.

Each skull has a polished silver brow facet angled toward the other face. These are actual reshaped regions of the skull mesh, not images pasted onto the surface. The tracer follows reflected rays to the neighbouring skull, including its color, facial details, and further reflections. The rest of each skull retains its curved chrome reflections.

Only two skull sculptures occupy the scene. Each is rendered as a colored body mesh and a silver brow mesh so they can have different materials. Their triangles partition one continuous surface without overlays. Three local subdivision passes refine the brow boundaries; acceleration structures are rebuilt after the geometry changes. There are no mirrors standing beside the skulls, floors, plinths, or props. Lighting and the studio environment add no geometry.

The two prints are **31 × 41 cm** each, with **21.59 cm (8.5 inches) between the visible image openings**. Both are crops of one camera view, retaining the unseen neighbouring sculpture in reflections.

Reproduce from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene skull-chromatic --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 9 --depth 6 --out art/skull-chromatic/print
python3 art/conversation/proof.py art/skull-chromatic/print --print-dpi
```

Print `print/left.png` and `print/right.png` at 31 × 41 cm without automatic cropping, borders, or color enhancement. They are 3661 × 4843 pixels with sRGB profiles and 300 DPI metadata. `print/wall-proof.jpg` is a framing preview. The shared camera and measured crop layout are recorded in `layout.json`.

Tests check that the scene contains only the four material parts of the two skulls, with no overlapping body/brow triangles. Changing the opposite skull leaves direct-only renders unchanged but changes a substantial area of the reflected image at high contrast in both panels.
