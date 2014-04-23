using UnityEngine;
using System.Collections;

public class InfiniteTerrainModifier : ATerrainModifier {

	FiniteTerrainModifier finiteTerrainModifier;

	public InfiniteTerrainModifier(ATerrainGenerator tg) : base(tg) {
		finiteTerrainModifier = new FiniteTerrainModifier(tg);
	}

	public override void generate (ErosionOptions? erosionOptions, float waterAmount) {
		int padding = 5;
		finiteTerrainModifier.setSize(width + padding * 2, height + padding * 2);

		finiteTerrainModifier.setOffset(terrainGenerator.getOffsetX() - padding, 
		                                terrainGenerator.getOffsetY() - padding);
		finiteTerrainModifier.generate(erosionOptions, waterAmount);

		terrainHeightmap 	= finiteTerrainModifier.getTerrainHeightmap().crop(padding, padding, width, height);
		waterHeightmap   	= finiteTerrainModifier.getWaterHeightmap().crop(padding, padding, width, height);
		waterflowMap 		= finiteTerrainModifier.getWaterflowMap().crop(padding, padding, width, height);
		erosionMap 			= finiteTerrainModifier.getErosionMap().crop(padding, padding, width, height);
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
