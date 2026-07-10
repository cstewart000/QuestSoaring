const DROPS = 2500;
const STEPS = 48;

export function erodeGrid(h, w, hgt, seed) {
  let s = seed >>> 0;
  const rng = () => { s = (s * 1664525 + 1013904223) >>> 0; return s / 4294967296; };
  for (let d = 0; d < DROPS; d++) drop(h, w, hgt, rng);
  for (let t = 0; t < 3; t++) thermal(h, w, hgt);
  console.log(`[ErosionSimulator] ${DROPS} drops on ${w}x${hgt}`);
}

function drop(h, w, hgt, rng) {
  let x = 1 + rng() * (w - 3), z = 1 + rng() * (hgt - 3);
  let sediment = 0;
  for (let s = 0; s < STEPS; s++) {
    let ix = clampF(x, 1, w - 2), iz = clampF(z, 1, hgt - 2);
    const gx = h[ix + 1][iz] - h[ix - 1][iz];
    const gz = h[ix][iz + 1] - h[ix][iz - 1];
    const len = Math.hypot(gx, gz) + 0.001;
    x -= (gx / len) * 0.45; z -= (gz / len) * 0.45;
    ix = clampF(x, 1, w - 2); iz = clampF(z, 1, hgt - 2);
    let nx = ix, nz = iz, low = h[ix][iz];
    for (let dz = -1; dz <= 1; dz++)
      for (let dx = -1; dx <= 1; dx++)
        if (h[ix + dx][iz + dz] < low) { low = h[ix + dx][iz + dz]; nx = ix + dx; nz = iz + dz; }
    if (nx === ix && nz === iz) break;
    const cap = Math.min(0.35, (h[ix][iz] - h[nx][nz]) * 0.35);
    h[ix][iz] -= cap; sediment += cap;
    if (sediment > 0.5) { h[nx][nz] += sediment * 0.35; sediment *= 0.65; }
  }
}

function thermal(h, w, hgt) {
  for (let z = 1; z < hgt - 1; z++)
    for (let x = 1; x < w - 1; x++) {
      let nx = x, nz = z, low = h[x][z];
      for (let dz = -1; dz <= 1; dz++)
        for (let dx = -1; dx <= 1; dx++)
          if (h[x + dx][z + dz] < low) { low = h[x + dx][z + dz]; nx = x + dx; nz = z + dz; }
      const diff = h[x][z] - low;
      if (diff > 0.6) { const m = diff * 0.45; h[x][z] -= m; h[nx][nz] += m; }
    }
}

export function flowAccum(h, w, hgt) {
  const flow = Array.from({ length: w }, () => new Float32Array(hgt).fill(1));
  const order = [];
  for (let z = 0; z < hgt; z++) for (let x = 0; x < w; x++) order.push({ x, z, ht: h[x][z] });
  order.sort((a, b) => b.ht - a.ht);
  for (const c of order) {
    let nx = c.x, nz = c.z, low = h[c.x][c.z];
    for (let dz = -1; dz <= 1; dz++)
      for (let dx = -1; dx <= 1; dx++) {
        const px = c.x + dx, pz = c.z + dz;
        if (px < 0 || pz < 0 || px >= w || pz >= hgt) continue;
        if (h[px][pz] < low) { low = h[px][pz]; nx = px; nz = pz; }
      }
    if (nx !== c.x || nz !== c.z) flow[nx][nz] += flow[c.x][c.z];
  }
  return flow;
}

const clampF = (v, a, b) => Math.max(a, Math.min(b, Math.floor(v)));
