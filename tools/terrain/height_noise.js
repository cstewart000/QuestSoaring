/** Ridged multifractal — matches QuestSoaring.Terrain.HeightNoise */
export const MAX_H = 480;
export const SCALE = 620;
export const OCTAVES = 5;

export function sampleHeight(x, z, scale = SCALE, octaves = OCTAVES) {
  let h = 0, amp = 1, freq = 1 / scale, ampSum = 0;
  for (let i = 0; i < octaves; i++) {
    let n = perlin(x * freq, z * freq);
    n = 1 - Math.abs(n * 2 - 1);
    h += n * n * amp;
    ampSum += amp;
    amp *= 0.52;
    freq *= 2.1;
  }
  let norm = h / ampSum;
  norm = Math.pow(Math.min(1, norm), 1.28);
  return norm * MAX_H;
}

export function heightToGray(y, slope = 0) {
  const t = Math.pow(Math.min(1, y / MAX_H), 0.88);
  let g = 0.1 + t * 0.62;
  g *= 1 - slope * 0.35;
  return [g, g, g];
}

function perlin(x, y) {
  const xi = Math.floor(x) & 255, yi = Math.floor(y) & 255;
  const xf = x - Math.floor(x), yf = y - Math.floor(y);
  const u = fade(xf), v = fade(yf);
  const aa = grad(hash(xi, yi), xf, yf);
  const ba = grad(hash(xi + 1, yi), xf - 1, yf);
  const ab = grad(hash(xi, yi + 1), xf, yf - 1);
  const bb = grad(hash(xi + 1, yi + 1), xf - 1, yf - 1);
  return lerp(lerp(aa, ba, u), lerp(ab, bb, u), v) * 0.5 + 0.5;
}

const perm = new Uint8Array(512);
for (let i = 0; i < 256; i++) perm[i] = perm[i + 256] = i;
for (let i = 255; i > 0; i--) { const j = Math.floor(Math.random() * (i + 1)); [perm[i], perm[j]] = [perm[j], perm[i]]; }
const fade = t => t * t * t * (t * (t * 6 - 15) + 10);
const lerp = (a, b, t) => a + t * (b - a);
const hash = (x, y) => perm[perm[x & 255] + (y & 255)] & 15;
const grad = (h, x, y) => ((h & 1) ? -x : x) + ((h & 2) ? -y : y);
