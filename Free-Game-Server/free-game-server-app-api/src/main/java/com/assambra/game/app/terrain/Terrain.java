package com.assambra.game.app.terrain;

import java.io.DataInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteOrder;
import java.nio.ByteBuffer;

public class Terrain {

    private int terrainSize;
    private float terrainHeight;
    private int numTiles;
    private int tileResolution;
    private int overlap;
    private float[][] heightmap;

    public Terrain(int terrainSize, float terrainHeight, int numTiles, int tileResolution, int overlap) {
        this.terrainSize = terrainSize;
        this.terrainHeight = terrainHeight;
        this.numTiles = numTiles;
        this.tileResolution = tileResolution;
        this.overlap = overlap;

        heightmap = loadHeightmap();
    }

    private float[][] loadHeightmap() {
        float[][] heightmap = new float[terrainSize][terrainSize];
        int tileSize = tileResolution - overlap;

        for (int tileZ = 0; tileZ < numTiles; tileZ++) {
            for (int tileX = 0; tileX < numTiles; tileX++) {
                String fileName = "Tile " + tileX + "," + tileZ + ".raw";
                float[][] tile = loadTile(fileName);

                int startX = tileX * tileSize - (tileX > 0 ? overlap : 0);
                int startY = tileZ * tileSize - (tileZ > 0 ? overlap : 0);

                for (int z = 0; z < tileResolution; z++) {
                    for (int x = 0; x < tileResolution; x++) {
                        heightmap[startX + x][startY + z] = tile[x][z];
                    }
                }
            }
        }

        return heightmap;
    }

    private float[][] loadTile(String fileName) {
        float[][] tile = new float[tileResolution][tileResolution];
        InputStream inputStream = getClass().getResourceAsStream("/" + fileName);

        if (inputStream != null) {
            try (DataInputStream dis = new DataInputStream(inputStream)) {
                ByteBuffer buffer = ByteBuffer.allocate(2);
                buffer.order(ByteOrder.LITTLE_ENDIAN); // Set the byte order to IBM PC format

                for (int z = 0; z < tileResolution; z++) {
                    for (int x = 0; x < tileResolution; x++) {
                        dis.read(buffer.array());
                        short rawHeightValue = buffer.getShort(0);
                        float adjustedHeightValue = (rawHeightValue + 32768) * (terrainHeight / 65535.0f);
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
        int x0 = (int) (x / (terrainSize - 1) * (tileResolution - 1));
        int x1 = Math.min(x0 + 1, tileResolution - 1);
        int z0 = (int) (z / (terrainSize - 1) * (tileResolution - 1));
        int z1 = Math.min(z0 + 1, tileResolution - 1);
        float tX = (x / (terrainSize - 1) * (tileResolution - 1)) - x0;
        float tZ = (z / (terrainSize - 1) * (tileResolution - 1)) - z0;
        float height00 = heightmap[x0][z0];
        float height01 = heightmap[x0][z1];
        float height10 = heightmap[x1][z0];
        float height11 = heightmap[x1][z1];
        float heightX0 = height00 * (1 - tZ) + height01 * tZ;
        float heightX1 = height10 * (1 - tZ) + height11 * tZ;
        return heightX0 * (1 - tX) + heightX1 * tX;
    }
}