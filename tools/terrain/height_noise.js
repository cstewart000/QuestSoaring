/** Height + biomes — matches QuestSoaring.Terrain */
export const MAX_H = 460;
export const SCALE = 680;
export const OCTAVES = 5;

export function sampleHeight(x, z) {
  return applyValleysRivers(baseHeight(x, z), x, z);
}

function baseHeight(x, z) {
  let h = 0, amp = 1, freq = 1 / SCALE, ampSum = 0;
  for (let i = 0; i < OCTAVES; i++) {
    const p = perlin(x * freq, z * freq);
    const ridged = 1 - Math.abs(p * 2 - 1);
    const n = ridged * 0.55 + p * 0.45;
    h += n * amp;
    ampSum += amp;
    amp *= 0.52;
    freq *= 2.05;
  }
  return Math.pow(Math.min(1, h / ampSum), 1.04) * MAX_H;
}

export function valleyMask(x, z) {
  const macro = perlin(x * 0.00085, z * 0.00085);
  return smooth(macro, 0.42, 0.58) * (1 - smooth(macro, 0.68, 0.82));
}

export function riverMask(x, z) {
  const ch = (ox, oz) => {
    const c = perlin(x * 0.0024 + ox, z * 0.0024 + oz);
    return clamp(1 - Math.abs(c - 0.5) * 9);
  };
  const r = Math.max(ch(12, 8), ch(140, 95), ch(260, 180));
  return r * r;
}

function applyValleysRivers(baseH, x, z) {
  const valley = valleyMask(x, z);
  const river = riverMask(x, z);
  const carve = valley * 75 + river * Math.max(60, 130 - baseH * 0.12);
  return Math.max(1.2, baseH - carve);
}

export function biomeAt(x, z, y, slope) {
  if (riverMask(x, z) > 0.35 && y < 140) return 'river';
  if (slope > 0.52) return 'cliff';
  if (y > 300) return 'alpine';
  if (y < 135 && slope < 0.38) return 'valley';
  if (y >= 85 && y <= 295 && slope < 0.42) return 'forest';
  return y > 200 ? 'alpine' : 'valley';
}

export function biomeColor(x, z, y, slope) {
  const b = biomeAt(x, z, y, slope);
  const speck = perlin(x * 0.07, z * 0.07);
  const g = {
    river: 0.03, cliff: 0.06, valley: 0.2,
    forest: 0.05 + speck * 0.1, alpine: 0.86 + speck * 0.14
  }[b] ?? 0.15;
  const v = clamp((g - 0.5) * 1.75 + 0.5);
  return [v, v, v];
}

const clamp = v => Math.max(0, Math.min(1, v));
const smooth = (t, a, b) => clamp((t - a) / (b - a));

function perlin(x, y) {
  const xi = Math.floor(x) & 255, yi = Math.floor(y) & 255;
  const xf = x - Math.floor(x), yf = y - Math.floor(y);
  const u = fade(xf), v = fade(yf);
  return lerp(lerp(grad(hash(xi,yi),xf,yf), grad(hash(xi+1,yi),xf-1,yf), u),
    lerp(grad(hash(xi,yi+1),xf,yf-1), grad(hash(xi+1,yi+1),xf-1,yf-1), v), v) * 0.5 + 0.5;
}

const perm = new Uint8Array(512);
for (let i = 0; i < 256; i++) perm[i] = perm[i + 256] = i;
for (let i = 255; i > 0; i--) { const j = Math.floor(Math.random() * (i + 1)); [perm[i], perm[j]] = [perm[j], perm[i]]; }
const fade = t => t * t * t * (t * (t * 6 - 15) + 10);
const lerp = (a, b, t) => a + t * (b - a);
const hash = (x, y) => perm[perm[x & 255] + (y & 255)] & 15;
const grad = (h, x, y) => ((h & 1) ? -x : x) + ((h & 2) ? -y : y);
