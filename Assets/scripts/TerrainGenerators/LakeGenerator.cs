using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class LakeGenerator : ATerrainGenerator
{
	public LakeGenerator(int seed) : base(seed) {
		this.frequency = 0.01d;
		this.persistence = 0.5d;
		this.lacunarity = 1.4d;
		this.octaves = 2;
		this.scale = 1d;
		this.heightScale = 0.5f;
		
		this.setupGenerator();
	}
	
	protected override float TerrainValue (float x, float y)
	{
		float tVal = Mathf.Abs(0.5f - base.TerrainValue(x, y));
		float vVal = base.TerrainValue(x * 0.4f, y * 0.4f);
		return Mathf.Clamp(vVal - tVal, 0f, 1f);
	}
}

