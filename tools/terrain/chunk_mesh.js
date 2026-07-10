import { getChunk, biomeColor, CHUNK, MAX_H } from './height_field.js';

const RES = 16;
export { CHUNK, MAX_H };

export function buildChunkMesh(coordX, coordZ) {
  const field = getChunk(coordX, coordZ);
  const ox = coordX * CHUNK, oz = coordZ * CHUNK;
  const verts = [], colors = [], tris = [];

  function addTri(wx0, wz0, wx1, wz1, wx2, wz2) {
    const y0 = bilinear(field, wx0, wz0);
    const y1 = bilinear(field, wx1, wz1);
    const y2 = bilinear(field, wx2, wz2);
    const ab = [wx1 - wx0, y1 - y0, wz1 - wz0], ac = [wx2 - wx0, y2 - y0, wz2 - wz0];
    const nx = ab[1]*ac[2]-ab[2]*ac[1], ny = ab[2]*ac[0]-ab[0]*ac[2], nz = ab[0]*ac[1]-ab[1]*ac[0];
    const slope = 1 - ny / (Math.hypot(nx, ny, nz) || 1);
    const cx = (wx0+wx1+wx2)/3, cz = (wz0+wz1+wz2)/3, cy = (y0+y1+y2)/3;
    const flow = bilinearFlow(field, cx, cz);
    const col = biomeColor(cx, cz, cy, slope, flow);
    const i = verts.length / 3;
    verts.push(wx0,y0,wz0, wx1,y1,wz1, wx2,y2,wz2);
    colors.push(...col, ...col, ...col);
    tris.push(i, i+1, i+2);
  }

  for (let z = 0; z < RES - 1; z++)
    for (let x = 0; x < RES - 1; x++) {
      const wx0 = ox + x * CHUNK / (RES - 1), wz0 = oz + z * CHUNK / (RES - 1);
      const wx1 = ox + (x + 1) * CHUNK / (RES - 1), wz1 = oz + (z + 1) * CHUNK / (RES - 1);
      addTri(wx0, wz0, wx0, wz1, wx1, wz0);
      addTri(wx1, wz0, wx0, wz1, wx1, wz1);
    }
  return { verts, tris, colors };
}

function bilinear(field, wx, wz) {
  let lx = (wx - field.ox) / field.cell, lz = (wz - field.oz) / field.cell;
  const gs = field.h.length;
  lx = Math.max(0, Math.min(gs - 1.001, lx)); lz = Math.max(0, Math.min(gs - 1.001, lz));
  const x0 = Math.floor(lx), z0 = Math.floor(lz), tx = lx - x0, tz = lz - z0;
  const a = field.h[x0][z0], b = field.h[Math.min(x0+1,gs-1)][z0];
  const c = field.h[x0][Math.min(z0+1,gs-1)], d = field.h[Math.min(x0+1,gs-1)][Math.min(z0+1,gs-1)];
  return (1-tx)*(1-tz)*a + tx*(1-tz)*b + (1-tx)*tz*c + tx*tz*d;
}

function bilinearFlow(field, wx, wz) {
  let lx = (wx - field.ox) / field.cell, lz = (wz - field.oz) / field.cell;
  const gs = field.flow.length;
  lx = Math.max(0, Math.min(gs - 1.001, lx)); lz = Math.max(0, Math.min(gs - 1.001, lz));
  const x0 = Math.floor(lx), z0 = Math.floor(lz), tx = lx - x0, tz = lz - z0;
  const a = field.flow[x0][z0], b = field.flow[Math.min(x0+1,gs-1)][z0];
  const c = field.flow[x0][Math.min(z0+1,gs-1)], d = field.flow[Math.min(x0+1,gs-1)][Math.min(z0+1,gs-1)];
  return (1-tx)*(1-tz)*a + tx*(1-tz)*b + (1-tx)*tz*c + tx*tz*d;
}

export function chunkCoords(camX, camZ, radius = 2) {
  const cx = Math.floor(camX / CHUNK), cz = Math.floor(camZ / CHUNK);
  const out = [];
  for (let dz = -radius; dz <= radius; dz++)
    for (let dx = -radius; dx <= radius; dx++) out.push([cx + dx, cz + dz]);
  return out;
}
