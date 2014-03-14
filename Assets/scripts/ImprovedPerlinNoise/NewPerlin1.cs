//Ken Perlin's Improved Noise Algorithm
//See http://mrl.nyu.edu/~perlin/paper445.pdf

// converted to Unity : http://unitycoder.com/blog/2012/04/12/improved-perlin-noise-algorithm/
// ** remember to donate : ) **

using UnityEngine;
using System.Collections;

public class NewPerlin1 : MonoBehaviour
{
	private float timeScale= 1f;
	[SerializeField] private float noiseScale = 0.2f;
	[SerializeField] private float octaves = 3;
	[SerializeField] private float persistence = 0.5f;
	[SerializeField] private Color lowColor = new Color(0f, 0f, 0f, 1f);
	[SerializeField] private Color highColor = new Color(1f, 1f, 1f, 1f);
	private int width = 1024;
	private int height = 1024;
	private Texture2D texture;
	private float frequency;
	private float amplitude;
	private float t = 0;
	
	static int[,] primes;
	Color32[] cols;
	
	void Start () 
	{
		texture = new Texture2D(width, height);
		renderer.material.mainTexture = texture;
		cols = new Color32[width*height];
		setupPrimes();
	}
	
	void Update () 
	{
		for (int y = 0; y<height; y++)
		{
			for (int x = 0; x<width; x++)
			{
				float r = 0;
				for (int i = 0; i < octaves; i++)
				{
					frequency = Mathf.Pow(2, i);
					amplitude = Mathf.Pow (persistence, i);
					r += Mathf.Abs(InterpolatedNoise(x * frequency * noiseScale, y * frequency * noiseScale, t, i) * amplitude);
				}
				cols[x+y*width] = r*highColor - (1-r) * lowColor;		
			}
		}
		t += Time.deltaTime * timeScale;
		texture.SetPixels32(cols);
		texture.Apply(false);
	}
	void OnGUI()
	{
		string fps = "" + 1/Time.deltaTime;
		GUI.TextArea(new Rect(0, 0, 100, 20), fps);
	}
	
	private int IntNoise(int x, int y, float z, int i)
	{
		int n = x + 57 * y;// + (int)(131 * z);
		n = (n<<13) ^ n;	
		float res = (1 - ((n * (n * n * primes[i,0] + primes[i,1]) + primes[i,2]) & 0x7fffffff) / 1073741824);
		return (int)res;  
	}

	private float SmoothedNoise(int x, int y, float t, int i)
	{	
		int it = (int)t;
		float corners = (IntNoise(x-1, y-1, it, i) + IntNoise(x+1, y-1,it, i) + IntNoise(x-1, y+1,it, i) + IntNoise(x+1, y+1,it, i)) / 16f;
		float sides   = (IntNoise(x-1, y,   it, i) + IntNoise(x+1, y,  it, i) + IntNoise(x,   y-1,it, i) + IntNoise(x,   y+1,it, i)) /  8f;
		float center  =  IntNoise(x, 	 y,	  it, i) / 4f;
		return center + sides + corners;
	}
	private float Cosine_Interpolate(float x, float y, float t)
	{
		float ft = t * Mathf.PI;
		float f = (1 - Mathf.Cos(ft)) * 0.5f;
			
		return  x*(1-f) + y*f;
	}
	private float InterpolatedNoise(float x, float y, float t, int i)
	{	
		int integer_X    = Mathf.FloorToInt(x);
		float fractional_X = x - integer_X;

		int integer_Y    = Mathf.FloorToInt(y);
		float fractional_Y = y - integer_Y;
				
		float v1 = SmoothedNoise(integer_X,     integer_Y, 		t,	i);
		float v2 = SmoothedNoise(integer_X + 1, integer_Y, 		t,	i);
		float v3 = SmoothedNoise(integer_X,     integer_Y + 1, 	t,	i);
		float v4 = SmoothedNoise(integer_X + 1, integer_Y + 1,	t,  i);
				
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
}
