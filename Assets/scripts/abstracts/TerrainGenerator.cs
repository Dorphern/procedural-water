using System.Collections;

public abstract class TerrainGenerator {

	protected int offsetX;
	protected int offsetY;
	protected int width;
	protected int height;
	protected int seed;

	public TerrainGenerator(int seed) {
		this.seed = seed;
	}
		
	public void setSize(int width, int height) {
		this.width 	= width;
		this.height = height;
	}

	public void setOffset(int x, int y) {
		this.offsetX = x;
		this.offsetY = y;
	}

	public abstract Heightmap generateWater();

	public abstract Heightmap generateTerrain();
}
