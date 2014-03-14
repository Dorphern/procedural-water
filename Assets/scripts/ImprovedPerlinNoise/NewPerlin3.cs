//Ken Perlin's Improved Noise Algorithm
//See http://mrl.nyu.edu/~perlin/paper445.pdf

// converted to Unity : http://unitycoder.com/blog/2012/04/12/improved-perlin-noise-algorithm/
// ** remember to donate : ) **

using UnityEngine;
using System.Collections;

namespace LibNoise.Unity.Generator
{
	public class NewPerlin3 : MonoBehaviour
	{
		[SerializeField] private double frequency = 1d;
		[SerializeField] private double lacunarity = 2.5d;
		[SerializeField] private int octaves = 2;
		[SerializeField] private int seed = 123452315;
		[SerializeField] private QualityMode quality = QualityMode.High;
		[SerializeField] private double scale = 1d;
		[SerializeField] private Color lowColor = new Color(0f, 0f, 0f, 1f);
		[SerializeField] private Color highColor = new Color(1f, 1f, 1f, 1f);
		private int width = 1024;
		private int height = 1024;
		private Texture2D texture;
		Color32[] cols;
		RidgedMultifractal ridgedMultiFractal;

		private float c;

		void Start()
		{
			UpdateTexture();
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				UpdateTexture();
			}
		}

		private void UpdateTexture()
		{
			ridgedMultiFractal = new RidgedMultifractal(frequency, lacunarity, octaves, seed, quality);
			texture = new Texture2D(width, height);
			renderer.material.mainTexture = texture;
			cols = new Color32[width*height];
			
			for (int y = 0; y<height; y++)
			{
				for (int x = 0; x<width; x++)
				{
					c = (float)ridgedMultiFractal.GetValue(x * scale, y * scale, 0);
					cols[x+y*width] = c*highColor + (1-c) * lowColor;		
				}
			}
			texture.SetPixels32(cols);
			texture.Apply(false);
		}

		
		void OnGUI()
		{
			string fps = "" + 1/Time.deltaTime;
			GUI.TextArea(new Rect(0, 0, 100, 20), fps);
		}
	}
}