//Ken Perlin's Improved Noise Algorithm
//See http://mrl.nyu.edu/~perlin/paper445.pdf

// converted to Unity : http://unitycoder.com/blog/2012/04/12/improved-perlin-noise-algorithm/
// ** remember to donate : ) **

using UnityEngine;
using System.Collections;

public class NewPerlin4 : MonoBehaviour
{
	private float timeScale= 1f;
	[SerializeField] private double frequency = 0.1d;
	[SerializeField] private double lacunarity = 2.5d;
	[SerializeField] private double noiseScale = 0.2d;
	[SerializeField] private int octaves = 3;
//	[SerializeField] private int seed = 123452315;
	[SerializeField] private Color lowColor = new Color(0f, 0f, 0f, 1f);
	[SerializeField] private Color highColor = new Color(1f, 1f, 1f, 1f);
	private int width = 1024;
	private int height = 1024;
	private Texture2D texture;
	private const int MAXOCTAVES = 32;
	private double[] weights = new double[MAXOCTAVES];
	
	static int[,] primes;
	Color32[] cols;
	
	void Start () 
	{
		texture = new Texture2D(width, height);
		renderer.material.mainTexture = texture;
		cols = new Color32[width*height];
		setupPrimes();


		for (double y = 0; y<height; y++)
		{
			for (double x = 0; x<width; x++)
			{
				x *= this.frequency;
				y *= this.frequency;

				double signal = 0.0;
				double weight = 1.0;
				double offset = 1.0;
				double gain = 2.0;
				double r = 0;
				for (int i = 0; i < octaves && i < MAXOCTAVES; i++)
				{
					double nx = MakeInt32Range(x) / noiseScale;
					double ny = MakeInt32Range(y) / noiseScale;
//					long seed = (this.seed + i) & 0x7fffffff;
					signal = SmoothedNoise((int)nx, (int)ny, i);
					signal = Mathf.Abs((float)signal);
					signal = offset - signal;
					signal *= signal;
					signal *= weight;
					weight = signal * gain;
					if (weight > 1.0) { weight = 1.0; }
					if (weight < 0.0) { weight = 0.0; }
					r += (signal * this.weights[i]);
					x *= this.lacunarity;
					y *= this.lacunarity;
				}
				r = r * 1.25d - 1.0d;
				cols[(int)x+(int)y*width] = (float)r*highColor - (1.0f-(float)r) * lowColor;		
			}
		}
		
		texture.SetPixels32(cols);
		texture.Apply(false);

	}
	
	void Update () 
	{

	}
	void OnGUI()
	{
		string fps = "" + 1/Time.deltaTime;
		GUI.TextArea(new Rect(0, 0, 100, 20), fps);
	}
	
	private int IntNoise(int x, int y, int i)
	{
		int n = x + 57 * y;// + (int)(131 * z);
		n = (n<<13) ^ n;	
		float res = (1 - ((n * (n * n * primes[i,0] + primes[i,1]) + primes[i,2]) & 0x7fffffff) / 1073741824);
		return (int)res;  
	}

	private float SmoothedNoise(int x, int y, int i)
	{	
		float corners = (IntNoise(x-1, y-1, i) + IntNoise(x+1, y-1, i) + IntNoise(x-1, y+1, i) + IntNoise(x+1, y+1, i)) / 16f;
		float sides   = (IntNoise(x-1, y,   i) + IntNoise(x+1, y,   i) + IntNoise(x,   y-1, i) + IntNoise(x,   y+1, i)) /  8f;
		float center  =  IntNoise(x,   y,	i) / 4f;
		return center + sides + corners;
	}
	private float Cosine_Interpolate(float x, float y, float t)
	{
		float ft = t * Mathf.PI;
		float f = (1 - Mathf.Cos(ft)) * 0.5f;
			
		return  x*(1-f) + y*f;
	}
	private float InterpolatedNoise(float x, float y, int i)
	{	
		int integer_X    = Mathf.FloorToInt(x);
		float fractional_X = x - integer_X;

		int integer_Y    = Mathf.FloorToInt(y);
		float fractional_Y = y - integer_Y;
				
		float v1 = SmoothedNoise(integer_X,     integer_Y, 		i);
		float v2 = SmoothedNoise(integer_X + 1, integer_Y, 		i);
		float v3 = SmoothedNoise(integer_X,     integer_Y + 1, 	i);
		float v4 = SmoothedNoise(integer_X + 1, integer_Y + 1,	i);
				
		float i1 = Cosine_Interpolate(v1, v2, fractional_X);
		float i2 = Cosine_Interpolate(v3, v4, fractional_X);
		return Cosine_Interpolate(i1, i2 , fractional_Y);
	}

	private void setupPrimes()
	{
		primes = new int[,] {{ 15731,  	 789221, 1376312589},
							 {241511,  49979687,  920419813},
							 { 11171, 104395301,  941083987},
							 {  9677, 275604541,  961748941},
							 {140167,  32452843,  941083981},
							 { 15199, 141650939,  982451653},
							 {340447, 236887691,  961748927},
							 {300007, 198491317,  920419823}};		
	}
	private double MakeInt32Range(double value)
	{
		if (value >= 1073741824.0) { return (2.0 * IEEERemainder(value, 1073741824.0)) - 1073741824.0; }
		else if (value <= -1073741824.0) { return (2.0 * IEEERemainder(value, 1073741824.0)) + 1073741824.0; }
		else { return value; }
	}

	private void UpdateWeights()
	{
		double f = 1.0f;
		for (int i = 0; i < MAXOCTAVES; i++)
		{
			this.weights[i] = (double)Mathf.Pow((float)f, -1.0f);
			f *= this.lacunarity;
		}
	}
	private double IEEERemainder (double x, double y)
	{
		double num = x % y;
		if (double.IsNaN (num))
		{
			return double.NaN;
		}
		if (num == 0.0 && x < 0)
		{
			return 0.0d;
		}
		double num2 = num - (double)Mathf.Abs ((float)y) * (x / (double)Mathf.Abs((float)x));
		if (Mathf.Abs ((float)num2) == Mathf.Abs ((float)num))
		{
			double num3 = x / y;
			double value = (double)Mathf.Round((float)num3);
			if (Mathf.Abs ((float)value) > Mathf.Abs ((float)num3))
			{
				return num2;
			}
			return num;
		}
		else
		{
			if (Mathf.Abs ((float)num2) < Mathf.Abs ((float)num))
			{
				return num2;
			}
			return num;
		}
	}
}
