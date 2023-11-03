package com.assambra.game.app.terrain;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class Terrain {

    private int terrainSize;
    private float terrainHeight;
    private int numTiles;
    private int tileSize;
    private int overlap;
    private float[][] heightmap;

    public Terrain(int terrainSize, float terrainHeight, int numTiles, int tileSize, int overlap) {
        this.terrainSize = terrainSize;
        this.terrainHeight = terrainHeight;
        this.numTiles = numTiles;
        this.tileSize = tileSize;
        this.overlap = overlap;

        heightmap = loadHeightmap();
    }

    private float[][] loadHeightmap() {
        float[][] heightmap = new float[terrainSize][terrainSize];
        int tileSize = this.tileSize - overlap;

        for (int tileZ = 0; tileZ < numTiles; tileZ++) {
            for (int tileX = 0; tileX < numTiles; tileX++) {
                String fileName = "Tile " + tileX + "," + tileZ + ".raw";
                float[][] tile = loadTile(fileName);

                int startX = tileX * tileSize - (tileX > 0 ? overlap : 0);
                int startY = tileZ * tileSize - (tileZ > 0 ? overlap : 0);

                for (int z = 0; z < this.tileSize; z++) {
                    for (int x = 0; x < this.tileSize; x++) {
                        heightmap[startX + x][startY + z] = tile[x][z];
                    }
                }
            }
        }

        return heightmap;
    }

    private float[][] loadTile(String fileName) {
        float[][] tile = new float[tileSize][tileSize];
        InputStream inputStream = getClass().getResourceAsStream("/" + fileName);

        if (inputStream != null) {
            try (InputStream is = inputStream) {
                ByteBuffer buffer = ByteBuffer.allocate(2);
                buffer.order(ByteOrder.LITTLE_ENDIAN);

                for (int z = 0; z < tileSize; z++) {
                    for (int x = 0; x < tileSize; x++) {
                        buffer.clear();
                        if (is.read(buffer.array()) != 2) {
                            throw new IOException("Not enough data available.");
                        }
                        int rawHeightValue = buffer.getShort(0) & 0xFFFF;
                        float adjustedHeightValue = (rawHeightValue / 65535.0f) * terrainHeight;
                        tile[x][z] = adjustedHeightValue;
                    }
                }
            } catch (IOException e) {
                e.printStackTrace();
            }
        }

        return tile;
    }

    public float getHeightValue(float x, float z) {
        if (x < 0.0f || x >= terrainSize || z < 0.0f || z >= terrainSize) {
            return -1.0f;
        }
        int x0 = (int) (x / (terrainSize - 1) * (tileSize - 1));
        int x1 = Math.min(x0 + 1, tileSize - 1);
        int z0 = (int) (z / (terrainSize - 1) * (tileSize - 1));
        int z1 = Math.min(z0 + 1, tileSize - 1);
        float tX = (x / (terrainSize - 1) * (tileSize - 1)) - x0;
        float tZ = (z / (terrainSize - 1) * (tileSize - 1)) - z0;
        float height00 = heightmap[x0][z0];
        float height01 = heightmap[x0][z1];
        float height10 = heightmap[x1][z0];
        float height11 = heightmap[x1][z1];
        float heightX0 = height00 * (1 - tZ) + height01 * tZ;
        float heightX1 = height10 * (1 - tZ) + height11 * tZ;
        return heightX0 * (1 - tX) + heightX1 * tX;
    }
}