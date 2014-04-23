using UnityEngine;
using System.Collections;

public class FiniteTerrainModifier : ATerrainModifier {

	int[] tileNeighbours = { 0,1,  0,-1,  1,1,  1,-1,  1,0,  -1,1,  -1,-1,  -1,0 };

	public FiniteTerrainModifier(ATerrainGenerator tg) : base(tg) { }

	public override void generate (ErosionOptions? erosionOptions, float waterAmount) {
		terrainHeightmap = new Heightmap(width, height);
		erosionMap = new Heightmap(width, height, 0.2f);

		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				terrainHeightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x, y));
			}
		}

		applyWaterEffects(waterAmount);
	}
	

	private void applyWaterEffects (float water) {
		// Apply rain (water) to every tile
		waterflowMap = new Heightmap(width, height, water);

		// Move the water around
		for (int i = 0; i < 5; i++) {
			for (int x = 0; x < this.width; x++) {
				for (int y = 0; y < this.width; y++) {
					int dirIndex = waterDirection(x, y);
					if (dirIndex == -1) continue;

					int tx = x + tileNeighbours[dirIndex],
						ty = y + tileNeighbours[dirIndex + 1];
					float fromAmount = waterflowMap.getHeight(x, y),
						  toAmount   = waterflowMap.getHeight(tx, ty);

					float diff = terrainHeightmap.getHeight(x, y) - terrainHeightmap.getHeight(tx, ty);
					float totalWater = fromAmount + toAmount;

					float ww = fromAmount + toAmount;
					
					if (totalWater <= diff) {
						// All water goes to new tile
						fromAmount = 0f;
						toAmount = totalWater;
					} else {
						// All water is split amongst the tiles
						totalWater -= diff;
						toAmount = diff + totalWater / 2f;
						fromAmount = totalWater / 2f;
					}
				   	
					waterflowMap.setHeight(x, y, fromAmount);
					waterflowMap.setHeight(tx, ty, toAmount);
				}
			}
		}


		waterHeightmap = new Heightmap(width, height);
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				float h = terrainHeightmap.getHeight(x, y) + waterflowMap.getHeight(x, y);
				waterHeightmap.setHeight(x, y, h - 0.1f);
			}
		}
	}

	/**
	 * Find the direction of the water on the current tile,
	 * taking into account the total height (water and terrain) on every tile
	 */
	private int waterDirection (int x, int y) {
		float currHeight = terrainHeightmap.getHeight(x, y) + waterflowMap.getHeight(x, y);
		int neighbourIndex = -1;
		for (int i = 0; i < tileNeighbours.Length; i += 2) {
			int dx = x + tileNeighbours[i],
				dy = y + tileNeighbours[i + 1];

			if (dx < 0 || dy < 0 || dx >= terrainHeightmap.getSizeWidth() 
			    || dy >= terrainHeightmap.getSizeHeight()) continue;

			float height = terrainHeightmap.getHeight(dx, dy) + waterflowMap.getHeight(dx, dy);
			if (height < currHeight) {
				currHeight = height;
				neighbourIndex = i;
			}
		}
		return neighbourIndex;
	}
}
