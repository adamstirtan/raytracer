"""Compare simulation instants using the same two portrait crops and physical gap."""
from pathlib import Path
import json
from PIL import Image, ImageDraw

root = Path("art/impact/studies")
folders = sorted(path for path in root.iterdir() if (path / "layout.json").exists())
tile_w, tile_h = 750, 465
sheet = Image.new("RGB", (tile_w * 2, tile_h * ((len(folders) + 1) // 2)), (218, 215, 208))
draw = ImageDraw.Draw(sheet)
for index, folder in enumerate(folders):
    x, y = index % 2 * tile_w, index // 2 * tile_h
    layout = json.loads((folder / "layout.json").read_text())
    gap = round(310 * layout["VisibleImageGapCm"] / layout["PanelWidthCm"])
    view = Image.new("RGB", (620 + gap, 410), (195, 192, 186))
    for name, offset in [("left", 0), ("right", 310 + gap)]:
        with Image.open(folder / f"{name}.png") as image:
            view.paste(image.convert("RGB").resize((310, 410)), (offset, 0))
    view.thumbnail((tile_w - 40, tile_h - 55), Image.Resampling.LANCZOS)
    sheet.paste(view, (x + (tile_w - view.width) // 2, y + 40))
    draw.text((x + 20, y + 14), f"{layout['SimulationTimeSeconds']:.2f} seconds", fill=(25, 25, 30))
sheet.save("art/impact/contact-sheet.jpg", quality=95)
