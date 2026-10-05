"""Generate reproducible, seamless surface maps for the Orbital showcase."""
from pathlib import Path
import numpy as np
from PIL import Image, ImageDraw

output = Path(__file__).resolve().parents[2] / 'RayTracer.Core' / 'Textures' / 'Orbital'
output.mkdir(parents=True, exist_ok=True)
rng = np.random.default_rng(73)
width, height = 2048, 1024
u, v = np.meshgrid(np.arange(width) / width, np.arange(height) / height)
tau = 2 * np.pi

def save(name, pixels):
    Image.fromarray(np.uint8(np.clip(pixels, 0, 255))).save(output / name)

def noise(octaves=6):
    result = np.zeros_like(u)
    for i in range(octaves):
        frequency = 2 ** i
        for _ in range(3):
            result += np.sin(tau * (rng.integers(1, 5) * frequency * u
                                   + rng.integers(1, 5) * frequency * v)
                             + rng.uniform(0, tau)) / (2 ** (i * .7) * 3)
    return result

# Flowing cloud bands with fine turbulence, cream zones and copper belts.
n = noise()
flow = v + .004 * np.sin(tau * u * 9 + v * 23) + .009 * n
bands = np.clip(.5 + .3 * np.sin(flow * tau * 13)
                + .18 * np.sin(flow * tau * 29) + .1 * np.sin(flow * tau * 53), 0, 1)
clouds = np.array([132, 82, 56]) + bands[..., None] * np.array([98, 121, 119])
wisps = noise(8)
clouds += wisps[..., None] * np.array([19, 16, 12])
# An elliptical vortex with nested, turbulent cloud walls.
dx = ((u - .32 + .5) % 1 - .5) / .055
dy = (v - .48) / .026
radius = np.sqrt(dx * dx + dy * dy)
angle = np.arctan2(dy, dx)
vortex = .5 + .5 * np.sin(radius * 23 + angle * 2 + wisps * 7)
storm = np.array([145, 43, 23]) + vortex[..., None] * np.array([73, 68, 41])
blend = np.clip((1.25 - radius) * 5, 0, 1)
clouds = clouds * (1 - blend[..., None]) + storm * blend[..., None]
save('gas-giant.png', clouds)

# Crater rims and dark floors baked into albedo, with varied mineral grain.
for name, base, count in [('moon.png', [150, 157, 165], 140), ('rock.png', [111, 94, 79], 90)]:
    grain = noise() * 16 + rng.normal(0, 2, u.shape)
    for _ in range(count):
        cx, cy = rng.random(), rng.uniform(.04, .96)
        r = rng.uniform(.006, .045)
        distance = np.sqrt((((u - cx + .5) % 1 - .5) * np.sin(cy * np.pi)) ** 2 + (v - cy) ** 2) / r
        grain += 26 * np.exp(-((distance - 1) / .13) ** 2) - 24 * np.exp(-(distance / .72) ** 4)
    save(name, np.array(base) + grain[..., None])

# Panel seams, recessed windows, fasteners and service markings.
for name, base in [('hull.png', (176, 187, 199)), ('blue-hull.png', (32, 67, 112)),
                   ('dark-hull.png', (42, 49, 62)), ('gold-hull.png', (174, 118, 47))]:
    image = Image.new('RGB', (1024, 512), base)
    draw = ImageDraw.Draw(image)
    for y in range(0, 512, 64):
        for x in range(0, 1024, 64):
            shade = int(rng.integers(-14, 15))
            color = tuple(max(0, min(255, c + shade)) for c in base)
            draw.rectangle((x+2, y+2, x+61, y+61), fill=color, outline=(65, 76, 88), width=2)
            for rx in (x+7, x+56):
                draw.ellipse((rx, y+7, rx+2, y+9), fill=(223, 225, 222))
            if (x//64 + y//64) % 3 == 0:
                draw.rectangle((x+12, y+25, x+51, y+38), fill=(9, 22, 35))
                for wx in range(x+15, x+50, 9):
                    draw.rectangle((wx, y+28, wx+4, y+34), fill=(90, 187, 213))
            if (x//64 + y//64) % 7 == 0:
                draw.line((x+12, y+48, x+42, y+48), fill=(228, 152, 54), width=3)
    image.save(output / name)

image = Image.new('RGB', (1024, 1024), (7, 13, 33))
draw = ImageDraw.Draw(image)
for y in range(0, 1024, 128):
    for x in range(0, 1024, 64):
        draw.rounded_rectangle((x+3, y+3, x+60, y+124), radius=6,
                               fill=(18, 41 + int(rng.integers(0, 14)), 98 + int(rng.integers(0, 24))),
                               outline=(70, 93, 144), width=2)
        for offset in range(12, 124, 12):
            draw.line((x+6, y+offset, x+57, y+offset), fill=(45, 70, 122))
        draw.line((x+31, y+4, x+31, y+123), fill=(156, 171, 185), width=1)
image.save(output / 'solar-cells.png')

# A restrained dust-cloud backdrop; stars remain the brightest background features.
nebula = np.exp(-((v - .43 - .11*np.sin(tau*u)) / .13) ** 2)
structure = np.clip(.6 + noise()*.35, 0, 1)
sky = np.array([1, 2, 5]) + (nebula * structure)[..., None] * np.array([19, 16, 38])
save('starfield.png', sky)
image = Image.open(output / 'starfield.png')
draw = ImageDraw.Draw(image)
for _ in range(2200):
    x, y = int(rng.integers(width)), int(rng.integers(height))
    brightness = int(rng.integers(85, 240))
    draw.point((x, y), fill=(brightness, brightness, min(255, brightness+12)))
for _ in range(35):
    x, y = int(rng.integers(width)), int(rng.integers(height))
    draw.ellipse((x-1,y-1,x+1,y+1), fill=(206,220,248))
image.save(output / 'starfield.png')
print(output)
