# Dialogue

Two large skull meshes face inward in a shared studio space. Irving Penn’s sculptural still-life photography is the artistic reference: restrained pearl and platinum tones, precise lighting, and strong forms against a warm paper-gray background. This is an original ray-traced composition.

The scene contains exactly two objects. There are no floors, mirrors, plinths, or props. The pearl and polished-silver materials reflect the other skull and the analytic studio environment. Each skull turns inward by 65 degrees in world space; the camera sees a three-quarter view. Their height is 20.5 scene units, compared with 12 in the previous skull scene.

Each visible print measures **31 × 41 cm**, with **21.59 cm (8.5 inches) between the visible image openings**. Both images share a camera and retain the neighbouring skull in reflected rays, even though the physical gap is omitted from the print crops.

Reproduce from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene skull-dialogue --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 9 --depth 6 --out art/skull-dialogue/print
python3 art/conversation/proof.py art/skull-dialogue/print --print-dpi
```

Print `print/left.png` and `print/right.png` at 31 × 41 cm without automatic cropping, borders, or colour enhancement. The files are 3661 × 4843 pixels, with sRGB profiles and 300 DPI metadata. `print/wall-proof.jpg` shows the pair in the supplied frames and is for preview only. `layout.json` records the shared camera, physical gap, and crop settings.

`Scene.Environment` supplies background and reflected light when a ray misses all geometry. `Scene.AddLighting()` adds illumination without visible geometry. The skulls use the mesh acceleration and smooth shading introduced for `skull-echo`. Render tests verify that changing the opposite skull changes reflected pixels in both panels while leaving direct-only renders unchanged. Separate tests cover studio environment reflections and illumination without geometry.
