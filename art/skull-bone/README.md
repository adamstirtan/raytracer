# Bone / Echo

The left skull has a matte ivory bone surface with cream mottling, fine pores, and mineral flecks. The right is dark reflective gray metal. Its original curved surface reflects the bone skull from the neighbouring print. The neutral studio and precise lighting carry forward the Irving Penn still-life inspiration.

The bone texture is generated procedurally, as requested, using seeded periodic noise at several scales. No photograph or image-generation API was used. Its albedo map has no directional illumination baked in. Triplanar projection blends texture samples across the skull’s surface normals, avoiding the need for OBJ texture coordinates. The generated map and the sculpture remain in the scene when tracing reflected rays.

The scene contains exactly two complete skull meshes, using the original OBJ vertices and triangles with only the placement, rotation, and uniform scale changed. Both skulls retain their natural curved foreheads. Reflections follow those curves and appear distorted across the metal surface. There are no props, floors, or standalone mirrors.

Each visible print measures **31 × 41 cm**, with **21.59 cm (8.5 inches) between the visible openings**. The prints share one camera view and omit the physical gap from their crops.

Reproduce the print pair from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene skull-bone --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 9 --depth 6 --out art/skull-bone/print
python3 art/conversation/proof.py art/skull-bone/print --print-dpi
```

Print `print/left.png` and `print/right.png` at 31 × 41 cm without automatic cropping, borders, or color enhancement. Both are 3661 × 4843 pixels, tagged sRGB and 300 DPI. `print/wall-proof.jpg` previews the pair in the supplied frames. `layout.json` records the measured camera layout.

## Texture source

The 2048 × 2048 generated map is `RayTracer.Core/Textures/Skull/bone-albedo.png`. Regenerate it with NumPy and Pillow installed:

```sh
python3 assets/texture-tools/generate_bone.py
```

The fixed seed is 1984. Periodic correlated noise produces ivory mottling, fine grain, pores, and sparse flecks. The scene repeats the map every eight world units, blends three projections with fourth-power normal weights, and uses bilinear filtering across the repeating edges. These projection settings are opt-in on `Mesh`; existing texture coordinates and texture sampling retain their previous behavior.

Tests cover projection axes, blending, texture origin/scale, wrapping, the original full skull topology, the two-sculpture composition, and the bone skull’s contribution to natural reflections in the metal. The full renderer suite also checks existing scene behavior.
