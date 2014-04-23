using UnityEngine;
using System.Collections;

public class InfiniteTerrainModifier : ATerrainModifier {

	public InfiniteTerrainModifier(ATerrainGenerator tg) : base(tg) {
		
	}

	public override void generate (ErosionOptions? erosionOptions, float waterAmount) {
		initializeHeightmaps();
		terrainHeightmap = new Heightmap(width, height);
		
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				terrainHeightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x, y));
			}
		}
		
		//applyWaterEffects(waterAmount);
	}
	
	
	private void initializeHeightmaps() {
		terrainHeightmap 	= new Heightmap(width, height, 0.2f);
		waterHeightmap 		= new Heightmap(width, height, 0.2f);
		waterflowMap 		= new Heightmap(width, height, 0.2f);
		erosionMap 			= new Heightmap(width, height, 0.2f);
	}


	/*
	public override Heightmap modifiedWater() {
		Heightmap heightmap = new Heightmap(this.width, this.height);
		
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				heightmap.setHeight(x, y, 0.1f);
			}
		}
		
		return heightmap;
	}
	
	public override Heightmap modifiedTerrain(bool erode) {
		Heightmap heightmap = new Heightmap(this.width, this.height);
		
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				heightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x, y));
			}
		}
		this.heightmap = heightmap;

		return heightmap;
	}*/
}
