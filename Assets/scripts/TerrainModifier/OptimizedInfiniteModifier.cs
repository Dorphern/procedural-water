﻿using UnityEngine;
using System.Collections;

public class OptimizedInfiniteModifier : ATerrainModifier {

	int[] tileNeighbours = { 
		0,1,  0,-1,  1,0,  -1,0,  
		1,1,  1,-1,  -1,1,  -1,-1
	};

	private int workingZoom = 0;
	private int mapPadding = 0;


	public OptimizedInfiniteModifier(ATerrainGenerator tg) : base(tg) {  }


	public override void generate (ErosionOptions? erosionOptions, int time, float waterAmount) {
		mapPadding = (int)Mathf.Pow (2f, time);
		totalSize = width + mapPadding * 2;

		terrainHeightmap = new Heightmap(totalSize);
		waterflowMap = new Heightmap(totalSize, 0f);
		erosionMap = new Heightmap(totalSize, 0f);
		
		for (int x = 0; x < totalSize; x++) {
			for (int y = 0; y < totalSize; y++) {
				terrainHeightmap.setHeight(x, y, this.terrainGenerator.GetHeight(x - mapPadding, y - mapPadding));
			}
		}

		createAccumulatedMap();
		applyWaterEffects(time, waterAmount);
		createFinalHeightmaps();
	}

	// Create waterHeightmap for export
	private void createFinalHeightmaps() {
		terrainHeightmap = terrainHeightmap.crop(mapPadding, mapPadding, width, height);
		//waterflowMap = waterflowMap.crop (0,0, width, height);
		waterflowMap = waterflowMap.crop (mapPadding, mapPadding, width, height);
		erosionMap = erosionMap.crop (mapPadding, mapPadding, width, height);

		waterHeightmap = new Heightmap(width, height);

		for (int x = 0; x < this.width; x++) {
			for (int y = 0; y < this.width; y++) {
				// Set terrain heightmap
				//float th = terrainHeightmap.getHeight(x, y) + erosionMap.getHeight(x, y);
				//terrainHeightmap.setHeight(x, y, th);
				
				// Set water heightmap
				float wh = terrainHeightmap.getHeight(x, y) + waterflowMap.getHeight(x, y);
				waterHeightmap.setHeight(x, y, wh - 0.01f);
			}
		}
	}

	private void applyWaterEffects (int time, float waterAmount) {
		waterflowMap = new Heightmap(totalSize, waterAmount);

		//moveWaterOnZoom(time);

		for (int i = time; i > 0; i--) {
			moveWaterOnZoom(i);
		}
	}

	private void moveWaterOnZoom (int zoom) {
		workingZoom = zoom;
		int s = getZoomSize();

		int steps = Mathf.FloorToInt(totalSize / s);
		//Debug.Log ("Steps: " + steps + " - " + s);
		//Debug.Log ("s");

		for (int x = 0; x < steps; x++) {
			for (int y = 0; y < steps; y++) {
				int dirIndex = waterDirection(x, y);

				if (dirIndex == -1) continue;

				int tx = x + tileNeighbours[dirIndex],
					ty = y + tileNeighbours[dirIndex + 1];
				float fromAmount = getZoomWaterHeight(x, y),
					  toAmount   = getZoomWaterHeight(tx, ty);

				float diff = getZoomTerrainHeight(x, y) - getZoomTerrainHeight(tx, ty);
				float totalWater = fromAmount + toAmount;
				
				float ww = fromAmount + toAmount;

				if (totalWater <= diff) {
					// All water goes to new tile
					fromAmount = 0f;
					toAmount = totalWater;
				} else {
					// All water is split amongst the tiles
					float moving = ((fromAmount - toAmount) + diff) / 2f;
					fromAmount -= moving;
					toAmount += moving;
				}



				setZoomWaterHeight(x, y, fromAmount);
				setZoomWaterHeight(tx, ty, toAmount);
			}
		}
	}

	private float getZoomWaterHeight (int x, int y) {
		int realX = zoomToRealCoord(x);
		int realY = zoomToRealCoord(y);
		return waterflowMap.getHeight(realX, realY);
	}

	private void setZoomWaterHeight (int x, int y, float h) {
		int x0 = zoomToRealCoord(x);
		int y0 = zoomToRealCoord(y);

		for (int i = x0; i < x0 + getZoomSize(); i++) {
			for (int j = y0; j < y0 + getZoomSize(); j++) {
				waterflowMap.setHeight(i, j, h);
			}
		}
	}

	private float getZoomTerrainHeight (int x, int y) {
		if (workingZoom == 1)
			return terrainGenerator.GetHeight(x, y);
		
		return GetAverageAreaHeight(zoomToRealCoord(x), 
	    	                        zoomToRealCoord(y),
	        	                    getZoomSize(),
	            	                getZoomSize());
	}

	private float getZoomHeight (int x, int y) {
		float terrainH = getZoomTerrainHeight(x, y);
		float waterflowH = getZoomWaterHeight(x, y);
		return terrainH + waterflowH;
	}

	private int zoomToRealCoord (int z) {
		return z * getZoomSize();
	}

	private int getZoomSize () {
		return Mathf.FloorToInt(Mathf.Pow(2f, (float)workingZoom - 1));
	}


	/**
	 * Find the direction of the water on the current tile,
	 * taking into account the total height (water and terrain) on every tile
	 */
	private int waterDirection (int x, int y) {
		float currHeight = getZoomHeight(x, y);
		int neighbourIndex = -1;
		for (int i = 0; i < tileNeighbours.Length; i += 2) {
			int dx = x + tileNeighbours[i];
			int dy = y + tileNeighbours[i + 1];

			int max = totalSize / getZoomSize();
			if (dx < 0 || dy < 0 || dx >= max || dy >= max) continue;
			
			float h = getZoomHeight(dx, dy);

			if (h < currHeight) {
				currHeight = h;
				neighbourIndex = i;
			}
		}
		return neighbourIndex;
	}
}
