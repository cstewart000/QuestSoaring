/** Continental uplift before erosion — matches NaturalUplift.cs */
export function uplift(x, z) {
  const warpX = x + perlin(x * 0.00028, z * 0.00028) * 420;
  const warpZ = z + perlin(x * 0.00028 + 90, z * 0.00028 + 40) * 420;
  const continent = perlin(warpX * 0.00022, warpZ * 0.00022);
  const ridge = 1 - Math.abs(perlin(warpX * 0.00085, warpZ * 0.00085) * 2 - 1);
  return (continent * 0.38 + ridge * 0.62) * 360 + 50;
}

function perlin(x, y) {
  const xi = Math.floor(x) & 255, yi = Math.floor(y) & 255;
  const xf = x - Math.floor(x), yf = y - Math.floor(y);
  const u = fade(xf), v = fade(yf);
  return lerp(lerp(grad(hash(xi,yi),xf,yf), grad(hash(xi+1,yi),xf-1,yf), u),
    lerp(grad(hash(xi,yi+1),xf,yf-1), grad(hash(xi+1,yi+1),xf-1,yf-1), v), v) * 0.5 + 0.5;
}

const perm = new Uint8Array(512);
for (let i = 0; i < 256; i++) perm[i] = perm[i + 256] = i;
for (let i = 255; i > 0; i--) { const j = (Math.sin(i * 928371 + 12345) * 0.5 + 0.5) * 255 | 0; [perm[i], perm[j]] = [perm[j], perm[i]]; }
const fade = t => t * t * t * (t * (t * 6 - 15) + 10);
const lerp = (a, b, t) => a + t * (b - a);
const hash = (x, y) => perm[perm[x & 255] + (y & 255)] & 15;
const grad = (h, x, y) => ((h & 1) ? -x : x) + ((h & 2) ? -y : y);

export { perlin };
