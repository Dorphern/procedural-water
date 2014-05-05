using UnityEngine;
using System.Collections;

public class OptimizedInfiniteModifier : ATerrainModifier {

	int[] tileNeighbours = { 0,1,  0,-1,  1,0,  -1,0,  1,1,  1,-1,  -1,1,  -1,-1};

	public OptimizedInfiniteModifier(ATerrainGenerator tg) : base(tg) { }


	public override void generate (ErosionOptions? erosionOptions, int time, float waterAmount) {
		terrainHeightmap = new Heightmap(width, height);
		waterflowMap = new Heightmap(width, height, 0);
		erosionMap = new Heightmap(width, height, 0);
		
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				terrainHeightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x, y));
			}
		}

		applyWaterEffects(time, waterAmount);
		createFinalHeightmaps();
	}

	private void applyWaterEffects (int time, float waterAmount) {
		waterflowMap = new Heightmap(width, height, 0.1f);
	}


	// Create waterHeightmap for export
	private void createFinalHeightmaps() {
		waterHeightmap = new Heightmap(width, height);
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				// Set terrain heightmap
				//float th = terrainHeightmap.getHeight(x, y) + erosionMap.getHeight(x, y);
				//terrainHeightmap.setHeight(x, y, th);
				
				// Set water heightmap
				float wh = terrainHeightmap.getHeight(x, y) + waterflowMap.getHeight(x, y);
				waterHeightmap.setHeight(x, y, wh);
			}
		}
	}
}
