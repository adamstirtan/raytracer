# Order / Escape

A tribute to the sphere geometry of the original university ray-tracing assignment. A rigid 4 × 5 × 3 lattice of equally sized spheres departs into a ribbon and an expanding two-turn helix, with three breakaway beads. Cyan, magenta, violet, blue, and peach repeat through both arrangements. Chrome spheres and a violet mirror floor reveal the other panel through recursive reflections.

The two visible prints measure **31 × 41 cm** each, separated by **21.59 cm (8.5 inches) between their visible image openings**. The hidden strip includes the mats and wall gap. One camera views the entire scene; two crops omit that strip while keeping all geometry available for shadows and reflections.

Reproduce the approximately 300 DPI print files from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene order-escape --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 9 --depth 6 --out art/order-escape/print
python3 art/conversation/proof.py art/order-escape/print --print-dpi
```

Print `print/left.png` and `print/right.png` at 31 × 41 cm, without automatic cropping, borders, or colour enhancement. Each file contains an sRGB profile and 300 DPI metadata. `print/wall-proof.jpg` illustrates the framed pair at the measured spacing; it is not a print file. The proof assumes 41 × 51 cm outer frames, as shown on the supplied frame label.

The lower-resolution `preview` folder uses the same scene, camera, gap, nine subpixel samples, and six-level reflection limit. Each folder includes a `layout.json` with the reproducible camera and crop settings.

The command temporarily disables NuGet auditing to work around the repository's existing ImageSharp 3.1.12 vulnerability warnings; it does not change dependency settings.
