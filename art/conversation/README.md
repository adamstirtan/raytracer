# A Conversation Across the Gap

One ray-traced sculpture studio, seen through two portrait windows. The copper ring and amber beads occupy the left opening; the blue sculpture and chrome sphere occupy the right. The angled mirror on the left reflects the blue sculpture and chrome sphere. The chrome sphere reflects the copper ring. Floor reflections share the same camera and horizon.

Each print is 31 × 41 cm. The hidden strip between the visible image openings is **21.59 cm (8.5 inches)**. This already includes the mats and wall space; do not add them again. The full virtual view spans 83.59 × 41 cm. The wall proof assumes 41 × 51 cm outer frames, based on the supplied frame label.

Render the print files from the repository root:

```sh
dotnet run --project RayTracer.Cli -c Release -p:NuGetAudit=false -- --scene conversation --diptych --gap-cm 21.59 --width 3661 --height 4843 --spp 16 --depth 6 --out art/conversation/print
python3 art/conversation/proof.py art/conversation/print --print-dpi
```

`--width` and `--height` describe each panel. `--gap-cm` is the distance between visible image openings. `--panel-width-cm` defaults to 31. The renderer skips primary camera rays in the hidden strip, while retaining every object for shadows and recursive reflections. `layout.json` records the camera, crop, and rendering settings.

Print `left.png` and `right.png` at 31 × 41 cm, approximately 300 DPI, without automatic cropping, borders, or photo enhancement. The wall proof is a placement illustration, not a print file. Render PNGs use the renderer's existing RGB output without additional exposure or colour grading.

The command disables the NuGet vulnerability audit only for this invocation because the repository's existing ImageSharp 3.1.12 dependency has advisories that otherwise fail restore under warnings-as-errors. No dependency settings have been changed.
