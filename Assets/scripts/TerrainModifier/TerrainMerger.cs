using UnityEngine;
using System.Collections;

public class TerrainMerger  {

	private ATerrainModifier modifier;
	private Heightmap terrainHeightmap;
	private Heightmap waterHeightmap;

	private Heightmap waterflow;
	private Heightmap erosion;

	private int size;
	private int splits;


	public TerrainMerger(ATerrainModifier mod, int size, int splits) {
		this.modifier 	= mod;
		this.size 		= size - 1;
		this.splits		= (int)Mathf.Pow(2, (splits - 1)); 
		Debug.Log ("splits: " + splits);
	}

	public void generate(ErosionOptions? erosionOptions, int time, float waterAmount) {
		int chunkSize = size / splits;
		//if (splits != 1) chunkSize /= 2;

		modifier.setSize(chunkSize, chunkSize);

		// Initialize heightmaps
		terrainHeightmap 	= new Heightmap(size + 1);
		waterHeightmap 		= new Heightmap(size + 1);
		waterflow 			= new Heightmap(size + 1);
		erosion 			= new Heightmap(size + 1);

		Heightmap terrainChunkHm, waterChunkHm;

		for (int x = 0; x < size; x += chunkSize) {
			for (int y = 0; y < size; y += chunkSize) {
				modifier.setOffset(x, y);
				modifier.generate(erosionOptions, time, waterAmount);

				terrainHeightmap.addOffset(x, y, modifier.getTerrainHeightmap());
				waterHeightmap.addOffset(x, y, modifier.getWaterHeightmap());
				waterflow.addOffset(x, y, modifier.getWaterflowMap());
				erosion.addOffset(x, y, modifier.getErosionMap());
			}
		}

	}

	public Heightmap WaterHeightmap() {
		return waterHeightmap;
	}

	public Heightmap TerrainHeightmap() {
		return terrainHeightmap;
	}

	public Heightmap WaterflowHeightmap() {
		return waterflow;
	}
	
	public Heightmap ErosionHeightmap() {
		return erosion;
	}
}
