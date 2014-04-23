using UnityEngine;
using System.Collections;

public class Heightmap {
	
	private int height = 0;
	private int width  = 0;

	private float[,] heights;

	public Heightmap(int width, int height) {
		this.height = height;
		this.width = width;
		heights = new float[width, height];
	}

	public Heightmap(int size) : this(size, size) { }

	public Heightmap(int width, int height, float defaultValue) : this(height, width) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				heights[x, y] = defaultValue;
			}
		}
	}

	public void addOffset(int offX, int offY, Heightmap hm) {
		for (int x = 0; x < hm.width; x++) {
			for (int y = 0; y < hm.height; y++) {
				heights[x + offX, y + offY] += hm.getHeight(x, y);
			}
		}
	}

	public Heightmap crop(int offX, int offY, int w, int h) {
		Heightmap heightmap = new Heightmap(w, h);
		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				heightmap.setHeight(x, y, getHeight(offX + x, offY + y));
			}
		}
		return heightmap;
	}

	public void setHeight(int x, int y, float value) {
		heights[x, y] = value;
	}

	public float getHeight(int x, int y) {
		return heights[x, y];
	}

	public float[,] getHeights() {
		return heights;
	}

	public int getSizeWidth () {
		return width;
	}

	public int getSizeHeight () {
		return height;
	}

}
