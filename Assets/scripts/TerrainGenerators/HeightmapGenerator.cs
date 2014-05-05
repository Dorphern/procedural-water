using UnityEngine;
using System.Collections;

using LibNoise.Unity.Generator;
using LibNoise.Unity;


public class HeightmapGenerator : ATerrainGenerator
{

	Texture2D heightmap;

	public HeightmapGenerator(int seed) : base(seed) {
		heightmap = Resources.Load("heightmap-2", typeof(Texture2D)) as Texture2D;
		setupGenerator();
	}
	
	protected override float TerrainValue (float x, float y) {
		if (x < 0 || y < 0 || x > heightmap.width || y > heightmap.height) return 0;

		return heightmap.GetPixel((int) x, (int) y).r;
	}
}

