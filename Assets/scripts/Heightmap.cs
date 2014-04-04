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

	public Heightmap(int width, int height, float defaultValue) : this(height, width) {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				heights[x, y] = defaultValue;
			}
		}
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
