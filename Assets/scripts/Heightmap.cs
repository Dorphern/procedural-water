using System.Collections;

public class Heightmap {
	
	private int height = 0;
	private int width  = 0;

	private float[,] heights;

	public Heightmap(int width, int height) {
		heights = new float[width, height];
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

}
