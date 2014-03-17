﻿using System.Collections;
using LibNoise.Unity.Generator;
using LibNoise.Unity;

public abstract class TerrainGenerator {

	// Terrain generation parameters
	protected double frequency = 0.07d;
	protected double lacunarity = 2.0d;
	protected int octaves = 1;
	protected QualityMode quality = QualityMode.High;
	protected double scale = 1d;
	protected RidgedMultifractal ridgedMultiFractal;


	protected int offsetX;
	protected int offsetY;
	protected int width;
	protected int height;
	protected int seed;


	protected float getBaseTerrainHeight(int x, int y) {
		return (float)ridgedMultiFractal.GetValue(x * scale, y * scale, 0);
	}


	public TerrainGenerator(int seed) {
		this.seed = seed;
		ridgedMultiFractal = new RidgedMultifractal(frequency, lacunarity, octaves, seed, quality);
	}
		
	public void setSize(int width, int height) {
		this.width 	= width;
		this.height = height;
	}

	public void setOffset(int x, int y) {
		this.offsetX = x;
		this.offsetY = y;
	}

	public void setScale(float s) {
		this.scale = s;
	}

	public abstract Heightmap generateWater();

	public abstract Heightmap generateTerrain();
}
