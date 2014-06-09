using UnityEngine;
using System.Collections;

public class KernelFiniteModifier : ATerrainModifier {

	int[] kernelNeighbours = {
		-1, 1,  0, 1,  1, 1,
		-1, 0,  0, 0,  1, 0,
		-1,-1,  0,-1,  1,-1
	};

	// 3x3 matrix
	float[] kernelModifier = {
		2f,  2f,  2f,

		2f,	 1f,  2f,

		2f,  2f,  2f
	};
	
	private int workingZoom = 0;
	private int mapPadding = 0;
	private Heightmap tempWaterFlowMap;
	
	public KernelFiniteModifier(ATerrainGenerator tg) : base(tg) { 

		// Kernel normalize
		float kernsum = 0f;
		for (int i = 0; i < kernelModifier.Length; i++) kernsum += kernelModifier[i];
		for (int i = 0; i < kernelModifier.Length; i++) kernelModifier[i] /= kernsum;

		//for (int i = 0; i < kernelModifier.Length; i++) Debug.Log (kernelModifier[i]);

	}
	
	
	public override void generate (ErosionOptions? erosionOptions, int time, float waterAmount) {
		mapPadding = Mathf.FloorToInt(Mathf.Pow(2f, (float)time));
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
		waterHeightmap = new Heightmap(width, height);
		terrainHeightmap = terrainHeightmap.crop(mapPadding, mapPadding, width, height);
		waterflowMap = waterflowMap.crop(mapPadding, mapPadding, width, height);
		erosionMap = erosionMap.crop(mapPadding, mapPadding, width, height);

		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
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

		for (int i = time; i > 0; i--) {
			moveWaterOnZoom(i);
		}
	}
	
	private void moveWaterOnZoom (int zoom) {
		workingZoom = zoom;
		int s = getZoomSize();

		int steps = Mathf.FloorToInt(totalSize / s);

		tempWaterFlowMap = new Heightmap(totalSize, 0f);

		for (int x = 1; x < steps - 1; x++) {
			for (int y = 1; y < steps - 1; y++) {
				float newValue = calculateHeight(x, y);
				newValue -= getZoomTerrainHeight(x, y);

				setTempZoomWaterHeight(x, y, newValue);
			}
		}

		waterflowMap = tempWaterFlowMap;

	}


	private float calculateHeight (int x, int y) {
		float val = 0f;

		// Get adjecent values
		for (int i = 0; i < kernelModifier.Length; i++) {
			val += kernelModifier[i] * 
				getZoomHeight(
					x + kernelNeighbours[i * 2], 
			        y + kernelNeighbours[i * 2 + 1]);
		}

		return val;
	}
	
	private float getZoomWaterHeight (int x, int y) {
		int realX = zoomToRealCoord(x);
		int realY = zoomToRealCoord(y);
		return waterflowMap.getHeight(realX, realY);
	}
	
	private void setTempZoomWaterHeight (int x, int y, float h) {
		int x0 = zoomToRealCoord(x);
		int y0 = zoomToRealCoord(y);

		for (int i = x0; i < x0 + getZoomSize(); i++) {
			for (int j = y0; j < y0 + getZoomSize(); j++) {
				tempWaterFlowMap.setHeight(i, j, h);
			}
		}
	}

	private float getZoomTerrainHeight (int x, int y) {
		if (workingZoom == 111) {
			return terrainGenerator.GetHeight(x, y);
		} else {
			return GetAverageAreaHeight(zoomToRealCoord(x), 
		    	                        zoomToRealCoord(y),
		        	                    getZoomSize(),
		            	                getZoomSize());
		}
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

}
