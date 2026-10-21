"""Make a scale-consistent wall proof; optionally tag print PNGs at 300 DPI."""
import json
import sys
from pathlib import Path
from PIL import Image, ImageDraw, ImageCms

folder = Path(sys.argv[1] if len(sys.argv) > 1 else "art/conversation/preview")
layout = json.loads((folder / "layout.json").read_text())
scale = 14
panel_w = round(layout["PanelWidthCm"] * scale)
panel_h = round(layout["PanelHeightCm"] * scale)
gap = round(layout["VisibleImageGapCm"] * scale)
border = round(5 * scale)  # 41 x 51 frame around a 31 x 41 opening.
margin = 65
wall = Image.new("RGB", (2 * panel_w + gap + 2 * border + 2 * margin,
                         panel_h + 2 * border + 2 * margin), (191, 190, 184))
draw = ImageDraw.Draw(wall)
for name, x in [("left", margin + border), ("right", margin + border + panel_w + gap)]:
    y = margin + border
    outer = (x - border, y - border, x + panel_w + border, y + panel_h + border)
    draw.rectangle(tuple(v + 7 for v in outer), fill=(146, 146, 140))
    draw.rectangle(outer, fill=(24, 26, 27))
    frame = 10
    draw.rectangle((outer[0] + frame, outer[1] + frame, outer[2] - frame, outer[3] - frame),
                   fill=(240, 238, 229))
    with Image.open(folder / f"{name}.png") as panel:
        wall.paste(panel.convert("RGB").resize((panel_w, panel_h), Image.Resampling.LANCZOS), (x, y))
        if "--print-dpi" in sys.argv:
            profile = ImageCms.ImageCmsProfile(ImageCms.createProfile("sRGB")).tobytes()
            panel.save(folder / f"{name}.png", dpi=(300, 300), icc_profile=profile)
wall.save(folder / "wall-proof.jpg", quality=95)
print(folder / "wall-proof.jpg")
