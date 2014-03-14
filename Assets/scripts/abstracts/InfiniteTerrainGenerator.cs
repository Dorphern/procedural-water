using System.Collections;

public abstract class InfiniteTerrainGenerator : TerrainGenerator {

	/*
	 * This is very much up for descussion! 
	 * I'm not very sure if this is the right way to do it.
	 * */

	public InfiniteTerrainGenerator(int seed) : base(seed) { }

	public abstract float pointWaterHeight(int x, int y);

	public abstract float pointTerrainHeight(int x, int y);


	public override Heightmap generateWater() {
		return new Heightmap(1, 1);
	}

	public override Heightmap generateTerrain() {
		return new Heightmap(1, 1);
	}
}
