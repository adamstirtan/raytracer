"""Generate a reproducible, seamless ivory-bone albedo map (NumPy + Pillow)."""
from pathlib import Path
import numpy as np
from PIL import Image

output = Path(__file__).resolve().parents[2] / 'RayTracer.Core' / 'Textures' / 'Skull'
output.mkdir(parents=True, exist_ok=True)
size = 2048
rng = np.random.default_rng(1984)
frequency = np.fft.fftfreq(size)
fx, fy = np.meshgrid(frequency, frequency)
radius2 = fx * fx + fy * fy


def field(wavelength):
    """Periodic correlated grain, with no directional lighting baked in."""
    source = rng.standard_normal((size, size))
    spectrum = np.fft.fft2(source)
    result = np.fft.ifft2(spectrum * np.exp(-radius2 * wavelength**2 * 2)).real
    return result / result.std()


clouds = field(155)
mineral = field(38)
grain = field(5)
fine = field(1.4)
# Delicate cream mottling and mineral flecks rather than stone-like fissures.
stain = np.maximum(0, clouds + .45 * mineral - .35)
pores = np.clip((grain - 1.15) / 1.7, 0, 1)**2
speckles = np.clip((fine - 2) / 1.1, 0, 1)
variation = clouds * 2.5 + mineral * 2 + grain * 1.4 + fine * .8
pixels = np.array([232., 222., 202.]) + variation[..., None]
pixels -= stain[..., None] * np.array([7., 10., 14.])
pixels -= pores[..., None] * np.array([33., 35., 34.])
pixels -= speckles[..., None] * np.array([17., 19., 20.])
Image.fromarray(np.uint8(np.clip(pixels, 0, 255))).save(output / 'bone-albedo.png')
print(output / 'bone-albedo.png')
