using UnityEngine;
using System.Collections;

public class FiniteTerrainModifier : ATerrainModifier {

	protected Heightmap sedimentLevel;

	bool enableErosion = false;
	ErosionOptions erosionOptions;

	int[] tileNeighbours = { 0,1,  0,-1,  1,0,  -1,0,  1,1,  1,-1,  -1,1,  -1,-1};

	public FiniteTerrainModifier(ATerrainGenerator tg) : base(tg) { }

	public override void generate (ErosionOptions? erosionOptions, int time, float waterAmount) {
		terrainHeightmap = new Heightmap(width, height);

		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				terrainHeightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x, y));
			}
		}

		enableErosion = erosionOptions.HasValue;
		if (enableErosion) this.erosionOptions = erosionOptions.Value;

		applyWaterEffects(time, waterAmount);
		createFinalHeightmaps(waterAmount);
	}


	private void applyWaterEffects (int time, float waterAmount) {
		sedimentLevel = new Heightmap(width, height, 0);
		waterflowMap = new Heightmap(width, height, 0);
		erosionMap = new Heightmap(width, height, 0);

		if (enableErosion) {
			for (int i = 0; i < erosionOptions.generations; i++) 
				applyErosionStep();

			// Add excess sediment back to the terrain
			terrainHeightmap.addOffset(0, 0, sedimentLevel);
		}

		// Place water after erosions
		waterflowMap = new Heightmap(width, height, waterAmount);
		for (int i = 0; i < time; i++) 
			moveWater(false);
	}


	private void applyErosionStep () {
		float erode = 0,
			  maxCapacity = 0,
			  sedimentOverhead = 0;

		// Pick up sediment on every tile
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.height; y++) {

				// Apply rain (water) to every tile
				waterflowMap.addHeight(x, y, erosionOptions.rainAmount);

				erode = erosionOptions.solubility;// * erosionOptions.erosionsPerGeneration;
				maxCapacity = waterflowMap.getHeight(x, y) * erosionOptions.sedimentCapacity;

				// Erode max
				if (sedimentLevel.getHeight(x, y) + erosionOptions.solubility > maxCapacity)
					erode = maxCapacity - sedimentLevel.getHeight(x, y);

				erosionMap.addHeight(x, y, -erode);
				sedimentLevel.addHeight(x, y, erode);
			}
		}

		// Move the water and sediment around
		moveWater(true);

		// Evaporate water
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.height; y++) {
				if (waterflowMap.addHeight(x, y, -erosionOptions.evaporation) < 0) {
					waterflowMap.setHeight(x, y, 0);
				}

				sedimentOverhead = sedimentLevel.getHeight(x, y) 
					- waterflowMap.getHeight(x, y) * erosionOptions.sedimentCapacity;

				if (sedimentOverhead > 0) {
					erosionMap.addHeight(x, y, sedimentOverhead);
					sedimentLevel.addHeight(x, y, -sedimentOverhead);
				}
			}
		}
	}
		
	// Move the water around
	private void moveWater (bool moveSediment) {
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				int dirIndex = waterDirection(x, y);
				if (dirIndex == -1) continue;
				
				int tx = x + tileNeighbours[dirIndex],
				ty = y + tileNeighbours[dirIndex + 1];
				float fromAmount = waterflowMap.getHeight(x, y),
				toAmount   = waterflowMap.getHeight(tx, ty);
				
				float diff = getActualHeight(x, y) - getActualHeight(tx, ty);
				float totalWater = fromAmount + toAmount;
				
				float ww = fromAmount + toAmount;
				
				if (totalWater <= diff) {
					// All water goes to new tile
					fromAmount = 0f;
					toAmount = totalWater;
				} else {
					// All water is split amongst the tiles
					float tw = totalWater - diff;
					toAmount = diff + tw / 2f;
					fromAmount = tw / 2f;
				}

				if (moveSediment) {
					float totalSediment = sedimentLevel.getHeight(x, y) + sedimentLevel.getHeight(tx, ty);
					sedimentLevel.setHeight(x, y, (fromAmount / totalWater) * totalSediment);
					sedimentLevel.setHeight(tx, ty, (toAmount / totalWater) * totalSediment);
				}
				
				waterflowMap.setHeight(x, y, fromAmount);
				waterflowMap.setHeight(tx, ty, toAmount);
			}
		}
	}

	private float getActualHeight(int x, int y) {
		return terrainHeightmap.getHeight(x, y) + erosionMap.getHeight(x, y);
	}
	
	// Create waterHeightmap for export
	private void createFinalHeightmaps(float water) {
		

		waterHeightmap = new Heightmap(width, height);
		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				// Set terrain heightmap
				float th = terrainHeightmap.getHeight(x, y) + erosionMap.getHeight(x, y);
				terrainHeightmap.setHeight(x, y, th);

				// Set water heightmap
				float wh = th + waterflowMap.getHeight(x, y);
				waterHeightmap.setHeight(x, y, wh - 0.1f);
			}
		}
	}

	/**
	 * Find the direction of the water on the current tile,
	 * taking into account the total height (water and terrain) on every tile
	 */
	private int waterDirection (int x, int y) {
		float currHeight = getActualHeight(x, y) + waterflowMap.getHeight(x, y);
		int neighbourIndex = -1;
		for (int i = 0; i < tileNeighbours.Length; i += 2) {
			int dx = x + tileNeighbours[i],
				dy = y + tileNeighbours[i + 1];

			if (dx < 0 || dy < 0 || dx >= terrainHeightmap.getSizeWidth() 
			    || dy >= terrainHeightmap.getSizeHeight()) continue;

			float height = getActualHeight(dx, dy) + waterflowMap.getHeight(dx, dy);
			if (height < currHeight) {
				currHeight = height;
				neighbourIndex = i;
			}
		}
		return neighbourIndex;
	}
}
