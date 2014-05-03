using UnityEngine;
using System.Collections;

public class Erosion {

	private int mapWidth;
	private int mapHeight;
	private Heightmap heightMap;
	
	private float[,] waterLevel;
	private float[,] sedimentLevel;

	private float rainAmount;
	private float solubility;
	private float evaporation;
	private float sedimentCapacity;

	public Erosion(Heightmap heightMap, float rainAmount, float solubility, float evaporation, float sedimentCapacity)
	{
		SetHeightMap(heightMap);
		SetParameters(rainAmount, solubility, evaporation, sedimentCapacity);
	}
		

	private void SetHeightMap(Heightmap heightMap)
	{
		this.heightMap = heightMap;
		this.mapWidth  = heightMap.getHeights().GetUpperBound(0);
		this.mapHeight = heightMap.getHeights().GetUpperBound(1);
		this.waterLevel = new float[mapWidth, mapHeight];
		this.sedimentLevel = new float[mapWidth, mapHeight];
	}

	private void SetParameters(float rainAmount, float solubility, float evaporation, float sedimentCapacity)
	{
		this.rainAmount = rainAmount;
		this.solubility = solubility;
		this.evaporation = evaporation;
		this.sedimentCapacity = sedimentCapacity;
	}

	public void ErodeTerrain(int generations, int erosionPerGeneration)
	{
		for(int g = 0; g < generations; g++)
		{
			/* 1: Rainfall */
			ApplyRain();
			/* 2: Erosion */
			for(int e = 0; e < 7; e++)
			{
				ApplyErosion();
			}
			/* 3: Movement */
			ApplyMovement();
			/* 4: Evaporation */
			ApplyEvaporation();
		}
	}

	/*
	 * Increases the amount of water 'globally
	 */ 
	private void ApplyRain()
	{
		for(int i = 0; i < mapWidth; i++)
		{
			for(int j = 0; j < mapHeight; j++)
			{
				waterLevel[i, j] += this.rainAmount;
			}
		}
	}

	/*
	 * Erodes some of the terrain into the water
	 */ 
	private void ApplyErosion()
	{
		for(int i = 0; i < mapWidth; i++)
		{
			for(int j = 0; j < mapHeight; j++)
			{
				float erodeAmount = this.solubility;
				// Disable this part if performance is too limited by it
				if (sedimentLevel[i,j] + this.solubility < waterLevel[i,j] * this.sedimentCapacity)
					erodeAmount = waterLevel[i,j] * this.sedimentCapacity - sedimentLevel[i,j];

				heightMap.setHeight(i, j, heightMap.getHeight(i,j) - erodeAmount);
				sedimentLevel[i, j] += erodeAmount;
			}
		}
	}

	/*
	 * Moves the water downhill, moving the sediments with it.
	 */
	private void ApplyMovement()
	{
		float[,] heights = this.heightMap.getHeights();
		float[,] tempWaterLevel    = (float[,])this.waterLevel.Clone();
		float[,] tempSedimentLevel = (float[,])this.sedimentLevel.Clone();
		int localDistributionIndex;
		int di = 0;
		int dj = 0;
		for(int i = 0; i < mapWidth; i++)
		{
			for(int j = 0; j < mapHeight; j++)
			{
				localDistributionIndex = GetLocalDistribution(heightMap, i, j);
				switch (localDistributionIndex)
				{
				case -1:
					continue;
				case 0:
					di = 1;
					dj = 0;
					break;
				case 1:
					di = 1;
					dj = 1;
					break;
				case 2:
					di = 0;
					dj = 1;
					break;
				case 3:
					di = -1;
					dj = 1;
					break;
				case 4:
					di = -1;
					dj = 0;
					break;
				case 5:
					di = -1;
					dj = -1;
					break;
				case 6:
					di = 0;
					dj = -1;
					break;
				case 7:
					di = 1;
					dj = -1;
					break;
				}
				di += i;
				dj += j;
				if (heights[di, dj] + waterLevel[di, dj] + waterLevel[i, j] < heights[i, j])
				{
					tempWaterLevel[di, dj] += waterLevel[i, j];
					tempWaterLevel[i, j] = 0;
					tempSedimentLevel[di, dj] += sedimentLevel[i, j];
					tempSedimentLevel[i, j] = 0;
				}
				else
				{
					float halfHeightDiff = ((heights[i,j] + waterLevel[i,j]) - (heights[di,dj] + waterLevel[di,dj]))/2f;
					float sedimentTransfered = (halfHeightDiff / waterLevel[i,j]) * sedimentLevel[i, j];
					tempWaterLevel[di, dj] += halfHeightDiff;
					tempWaterLevel[i, j]   -= halfHeightDiff;
					tempSedimentLevel[di, dj] += sedimentTransfered;
					tempSedimentLevel[i, j]   -= sedimentTransfered;
				}
			}
		}
		waterLevel    = tempWaterLevel;
		sedimentLevel = tempSedimentLevel;
	}

	/*
	 * Removes some of the water, evenly across the map.
	 */ 
	private void ApplyEvaporation()
	{
		float[,] heights = heightMap.getHeights();
		for(int i = 0; i < mapWidth; i++)
		{
			for(int j = 0; j < mapHeight; j++)
			{
				waterLevel[i,j] -= evaporation;
				if (waterLevel[i,j] < 0)
					waterLevel[i,j] = 0;

				float sedimentOverhead = sedimentLevel[i,j] - waterLevel[i,j] * sedimentCapacity;
				if (sedimentOverhead > 0)
				{
					heightMap.setHeight(i, j, heights[i,j] + sedimentOverhead);
					sedimentLevel[i,j] -= sedimentOverhead;
				}
			}
		}
	}

	/*
	 * Returns an array of 8 floats, resembling which direction the water should go.
	 */
	private int GetLocalDistribution(Heightmap heightMap, int i, int j)
	{
		/* 
		 * Currently simple implementation.
		 * Distributes water to lowest neighbour.
		 */
		float[] localHeights = GetLocalHeights(heightMap, i, j);
		float lowestHeight = float.MaxValue;
		int lowestLocalIndex = -1;
		for(int n = 0; n < localHeights.Length; n++)
		{
			if (localHeights[n] < lowestHeight)
			{
				lowestHeight = localHeights[n];
				lowestLocalIndex = n;
			}
		}
		return lowestLocalIndex;
	}

	private float[] GetLocalHeights(Heightmap heightMap, int i, int j)
	{
		float[,] heights = heightMap.getHeights();
		float[] localHeights = new float[8];
		bool ig0 = i > 0;
		bool ilm = i < heights.GetUpperBound(0) - 1;
		bool jg0 = j > 0;
		bool jlm = j < heights.GetUpperBound(1) - 1;

		localHeights[0] = (ilm 			? heights[i+1, j  ] : float.MaxValue);
		localHeights[1] = (ilm && jlm 	? heights[i+1, j+1] : float.MaxValue);
		localHeights[2] = (jlm 			? heights[i  , j+1] : float.MaxValue);
		localHeights[3] = (ig0 && jlm 	? heights[i-1, j+1] : float.MaxValue);
		localHeights[4] = (ig0 			? heights[i-1, j  ] : float.MaxValue);
		localHeights[5] = (ig0 && jg0	? heights[i-1, j-1] : float.MaxValue);
		localHeights[6] = (jg0 			? heights[i  , j-1] : float.MaxValue);
		localHeights[7] = (ilm && jg0 	? heights[i+1, j-1] : float.MaxValue);

		return localHeights;
	}

	private float[] GetLocalSlopes(Heightmap heightMap, int i, int j)
	{
		float[] slopes = new float[8];
		float[,] heights = heightMap.getHeights();
		
		bool ig0 = i > 0;
		bool ilm = i < heights.GetUpperBound(0) - 1;
		bool jg0 = j > 0;
		bool jlm = j < heights.GetUpperBound(1) - 1;
		
		slopes[0] = (ilm 		? heights[i,j] - heights[i+1, j  ] : float.MaxValue);
		slopes[1] = (ilm && jlm ? heights[i,j] - heights[i+1, j+1] : float.MaxValue);
		slopes[2] = (jlm 		? heights[i,j] - heights[i  , j+1] : float.MaxValue);
		slopes[3] = (ig0 && jlm ? heights[i,j] - heights[i-1, j+1] : float.MaxValue);
		slopes[4] = (ig0 		? heights[i,j] - heights[i-1, j  ] : float.MaxValue);
		slopes[5] = (ig0 && jg0	? heights[i,j] - heights[i-1, j-1] : float.MaxValue);
		slopes[6] = (jg0 		? heights[i,j] - heights[i  , j-1] : float.MaxValue);
		slopes[7] = (ilm && jg0 ? heights[i,j] - heights[i+1, j-1] : float.MaxValue);
		
		return slopes;
	}
}
