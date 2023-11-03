package com.assambra.game.app.terrain;

import com.tvd12.ezyfox.util.EzyLoggable;

import java.io.BufferedInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;

public class Terrain extends EzyLoggable {

    private int terrainSize;
    private float terrainHeight;
    private int numTiles;
    private int tileSize;
    private float[][] heightmap;
    private static final int BUFFER_SIZE = 4096;

    public Terrain(int terrainSize, float terrainHeight, int numTiles, int tileSize) {
        this.terrainSize = terrainSize;
        this.terrainHeight = terrainHeight;
        this.numTiles = numTiles;
        this.tileSize = tileSize;

        this.heightmap = new float[terrainSize][terrainSize];
        loadHeightmap();
        scaleHeightmap();
    }

    private void loadHeightmap() {
        for (int tileZ = 0; tileZ < numTiles; tileZ++) {
            for (int tileX = 0; tileX < numTiles; tileX++) {
                String fileName = "Tile " + tileX + "," + tileZ + ".raw";
                float[][] tile = loadTile(fileName);

                int startX = tileX * tileSize;
                int startY = tileZ * tileSize;

                for (int z = 0; z < tileSize; z++) {
                    for (int x = 0; x < tileSize; x++) {
                        heightmap[startX + x][startY + z] = tile[x][z];
                    }
                }
            }
        }
    }

    private float[][] loadTile(String fileName) {
        float[][] tile = new float[tileSize][tileSize];
        InputStream inputStream = getClass().getResourceAsStream("/" + fileName);

        if (inputStream != null) {
            try (BufferedInputStream bis = new BufferedInputStream(inputStream, BUFFER_SIZE)) {
                ByteBuffer buffer = ByteBuffer.allocate(2);
                buffer.order(ByteOrder.LITTLE_ENDIAN);

                for (int z = 0; z < tileSize; z++) {
                    for (int x = 0; x < tileSize; x++) {
                        buffer.clear();
                        if (bis.read(buffer.array()) != 2) {
                            throw new IOException("Not enough data available.");
                        }
                        int rawHeightValue = buffer.getShort(0) & 0xFFFF;
                        float adjustedHeightValue = (rawHeightValue / 65535.0f) * terrainHeight;
                        tile[x][z] = adjustedHeightValue;
                    }
                }
            } catch (IOException e) {
                logger.error("Failed to load heightmap: ", e.getMessage());
            }
        } else {
            logger.error("Input stream for heightmap is null. Check if the file {} exists and is accessible.", fileName);
        }
        return tile;
    }

    private void scaleHeightmap() {
        float scaleX = (float) terrainSize / (tileSize * numTiles);
        float scaleY = scaleX; // Assuming uniform scaling

        float[][] newHeightmap = new float[terrainSize][terrainSize];

        for (int z = 0; z < terrainSize; z++) {
            for (int x = 0; x < terrainSize; x++) {
                // Convert the scaled indices back to the original heightmap's space
                float xOrig = x / scaleX;
                float zOrig = z / scaleY;

                // Calculate the indices of the top-left corner
                int xFloor = (int) Math.floor(xOrig);
                int zFloor = (int) Math.floor(zOrig);

                // Calculate the fractional part to determine the weights for interpolation
                float xFrac = xOrig - xFloor;
                float zFrac = zOrig - zFloor;

                // Ensure we don't go out of bounds
                int xCeil = xFloor + (xFloor < tileSize * numTiles - 1 ? 1 : 0);
                int zCeil = zFloor + (zFloor < tileSize * numTiles - 1 ? 1 : 0);

                // Perform bilinear interpolation
                float top = (1 - xFrac) * heightmap[xFloor][zFloor] + xFrac * heightmap[xCeil][zFloor];
                float bottom = (1 - xFrac) * heightmap[xFloor][zCeil] + xFrac * heightmap[xCeil][zCeil];
                newHeightmap[x][z] = (1 - zFrac) * top + zFrac * bottom;
            }
        }

        heightmap = newHeightmap;
    }



    public float getHeightValue(float worldX, float worldZ) {
        // Scale from world coordinates to heightmap coordinates
        float xCoord = (worldX / terrainSize) * (heightmap.length - 1);
        float zCoord = (worldZ / terrainSize) * (heightmap[0].length - 1);

        // Calculate the indices of the bottom left corner
        int xFloor = (int) Math.floor(xCoord);
        int zFloor = (int) Math.floor(zCoord);

        // Calculate the fractional part to determine the weights for interpolation
        float xFrac = xCoord - xFloor;
        float zFrac = zCoord - zFloor;

        // Ensure we don't go out of bounds
        int xCeil = xFloor + (xFloor < heightmap.length - 1 ? 1 : 0);
        int zCeil = zFloor + (zFloor < heightmap[0].length - 1 ? 1 : 0);

        // Perform bilinear interpolation
        float top = (1 - xFrac) * heightmap[xFloor][zFloor] + xFrac * heightmap[xCeil][zFloor];
        float bottom = (1 - xFrac) * heightmap[xFloor][zCeil] + xFrac * heightmap[xCeil][zCeil];
        return (1 - zFrac) * top + zFrac * bottom;
    }
}