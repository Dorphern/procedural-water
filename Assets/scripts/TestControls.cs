using UnityEngine;
using System.Collections;
using System.Text;

public class TestControls : MonoBehaviour {

	private TerrainMerger terrainMerger;
	private Heightmap groundHeightmap;
	private Heightmap waterHeightmap;
	private Heightmap erosionMap;
	private Heightmap waterflowMap;

	private string generationSeed = "";
	private float simulationSize = 1f;
	private int terrainGenerator = 0;
	private int heatmap = 0; //0 - none, 1 - height, 2 - waterflow, 3 - errosion
	private bool useInfiniteModifier = false;
	private bool visualizeHeight = true;
	private bool visualizeErosion = false;
	private int mapSplits = 1;
	[SerializeField] private float simSizeMin = 5f;
	[SerializeField] private float simSizeMax = 10f;
	[SerializeField] private Terrain groundTerrain;
	[SerializeField] private Terrain waterTerrain;

	[SerializeField] private float rainAmount = 0.1f;
	[SerializeField] private float solubility = 0.01f;
	[SerializeField] private float evaporation = 0.1f;
	[SerializeField] private float sedimentCapacity = 0.03f;
	[SerializeField] private int generations = 7;
	[SerializeField] private int erosionsPerGeneration = 7;

	[SerializeField] private float waterAmount = 0.1f;
	[SerializeField] private int time = 5;


	// Use this for initialization
	void Start () {
		this.simulationSize = this.simSizeMin;
		generateTerrain();
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(0, 0, 200, 500));
		GUILayout.BeginVertical("box");

		//GUILayout.Box("walla", GUILayout.Height(200));
		this.generationSeed = GUILayout.TextField(this.generationSeed);

		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUILayout.Width(80));
				GUILayout.Label("Size: ");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				this.simulationSize = Mathf.Round(GUILayout.HorizontalSlider(this.simulationSize, this.simSizeMin, this.simSizeMax));
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUILayout.Width(80));
				GUILayout.Label("Map Splits: ");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				this.mapSplits = (int)Mathf.Round(GUILayout.HorizontalSlider(this.mapSplits, 1f, 4f));
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		
		this.useInfiniteModifier = GUILayout.Toggle(this.useInfiniteModifier, " Infinite terrain");

		bool oldVisHeight = this.visualizeHeight;
		this.visualizeHeight = GUILayout.Toggle(this.visualizeHeight, " Visualize height");

		bool oldVisErosion = this.visualizeErosion;
		this.visualizeErosion = GUILayout.Toggle(this.visualizeErosion, " Visualize erosion");

		if (oldVisHeight != this.visualizeHeight
		    || oldVisErosion != this.visualizeErosion) { this.updateTerrainVisualization(); }

		string type = "";
		switch (this.heatmap) {
			case 0: type = "none"; break; 
			case 1: type = "height"; break; 
			case 2: type = "waterflow"; break; 
			case 3: type = "errosion"; break; 
		}
		GUILayout.Label("Heatmap: " + type);
		int oldHeatmap = this.heatmap;
		this.heatmap = (int)Mathf.Round(GUILayout.HorizontalSlider(this.heatmap, 0.0f, 3.0f));
		if (oldHeatmap != this.heatmap) { this.updateTerrainVisualization(); }

		GUILayout.BeginHorizontal();
			GUILayout.BeginVertical(GUILayout.Width(80));
				GUILayout.Label("Generator: ");
			GUILayout.EndVertical();
			GUILayout.BeginVertical();
				this.terrainGenerator = (int)Mathf.Round(GUILayout.HorizontalSlider((float)this.terrainGenerator, 0, 2));
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();

		if (GUILayout.Button("Generate!")) { this.generateTerrain(); }

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	int numberFromString(string str) {
		int d = 0, i = 0;
		foreach (char c in str)
		{
			d += ((int) Mathf.Pow(10, i)) * (int) c;
			i++;
		}
		return d;
	}

	void setTerrainObjects() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		int res = (int)(Mathf.Pow(2, this.simulationSize) + 1f);
		int size = res * 2;
		
		int height = 30;
		
		groundTerrainData.alphamapResolution 	= waterTerrainData.alphamapResolution 	= res;
		groundTerrainData.heightmapResolution 	= waterTerrainData.heightmapResolution 	= res;
		groundTerrainData.SetDetailResolution(res, 8);
		waterTerrainData.SetDetailResolution(res, 8);
		
		groundTerrainData.size = waterTerrainData.size = new Vector3(size, height, size);
		
		this.groundTerrain.gameObject.transform.position = 
			this.waterTerrain.gameObject.transform.position =
				new Vector3(-groundTerrainData.size.x / 2f, 0f, -groundTerrainData.size.z / 2f);
	}

	ATerrainGenerator getTerrainGenerator(int num, int seed) {
		switch (num) {
			case 0: return new MountainGenerator(seed);
			case 1: return new CanyonGenerator(seed);
			case 2: return new PillarGenerator(seed);
		}
		return null;
	}

	Vector4 heatmapVector(float w) { 
		Vector4 weights = new Vector4(1, 0, 0, 0);

		float b = Mathf.Clamp((w) * 3f, 0f, 1f);
		float g = Mathf.Clamp((w - 0.33f) * 3f, 0f, 1f);
		float r = Mathf.Clamp((w - 0.66f) * 3f, 0f, 1f);

		weights = Vector4.Lerp(weights, new Vector4(0, 0, 0, 1), b);
		weights = Vector4.Lerp(weights, new Vector4(0, 0, 1, 0), g);
		weights = Vector4.Lerp(weights, new Vector4(0, 1, 0, 0), r);

		return weights;
	}
	
	void generateTerrain() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		setTerrainObjects();

		int res = (int)(Mathf.Pow(2, this.simulationSize) + 1f);
		int size = res * 2;

		ATerrainGenerator generator = this.getTerrainGenerator(this.terrainGenerator, 
		                                                       numberFromString(this.generationSeed));
		ATerrainModifier modifier;

		if (this.useInfiniteModifier) {
			modifier = new InfiniteTerrainModifier(generator);
		} else {
			modifier = new FiniteTerrainModifier(generator);
		}

		ErosionOptions? erosionOptions = null;
		if (this.visualizeErosion) {
			erosionOptions = new ErosionOptions {
				rainAmount 				= rainAmount,
				solubility 				= solubility,
				evaporation 			= evaporation,
				sedimentCapacity 		= sedimentCapacity,
				generations 			= generations,
				erosionsPerGeneration	= erosionsPerGeneration
			};
		}

		TerrainMerger terrainMerger = new TerrainMerger(modifier, res, mapSplits);
		terrainMerger.generate(erosionOptions, time, waterAmount);


		//modifier.setSize(res, res);
		this.groundHeightmap = terrainMerger.TerrainHeightmap();
		this.waterHeightmap = terrainMerger.WaterHeightmap();


		//Erosion erosionController = new Erosion(this.groundHeightmap, this.rainAmount, this.solubility, this.evaporation, this.sedimentCapacity);
		//erosionController.ErodeTerrain(generations, erosionsPerGeneration);


		this.erosionMap = terrainMerger.ErosionHeightmap();
		this.waterflowMap = terrainMerger.WaterflowHeightmap();

		this.updateTerrainVisualization();
	}

	void updateTerrainVisualization() {
		TerrainData groundTerrainData 	= this.groundTerrain.terrainData;
		TerrainData waterTerrainData 	= this.waterTerrain.terrainData;

		Heightmap groundHm, waterHm;

		if (this.visualizeHeight) {
			groundHm = this.groundHeightmap;
			waterHm = this.waterHeightmap;
		} else {
			int size = this.groundHeightmap.getSizeHeight();
			groundHm = new Heightmap(size, size, 0.2f);
			waterHm = new Heightmap(size, size, 0.1f);
		}

		float[,,] splatmapData = new float[groundTerrainData.alphamapWidth, 
		                                   groundTerrainData.alphamapHeight, 
		                                   groundTerrainData.alphamapLayers];


		Heightmap heightmapViz = this.groundHeightmap;
		switch(this.heatmap) {
			case 1: heightmapViz = this.groundHeightmap; break;
			case 2: heightmapViz = this.waterflowMap; break;
			case 3: heightmapViz = this.erosionMap; break;
		}

		for (int x = 0; x < groundTerrainData.alphamapWidth; x++) {
			for (int y = 0; y < groundTerrainData.alphamapHeight; y++) {

				Vector4 splat = Vector4.zero;
				if (this.heatmap != 0) {
					splat = this.heatmapVector(heightmapViz.getHeight(x, y));
				}

				splatmapData[x, y, 0] = ( this.heatmap == 0 ? 1f : 0f);
				splat.Normalize();
				splatmapData[x, y, 1] = splat.x;
				splatmapData[x, y, 2] = splat.y;
				splatmapData[x, y, 3] = splat.z;
				splatmapData[x, y, 4] = splat.w;
			}
		}

		groundTerrainData.SetAlphamaps(0, 0, splatmapData);

		groundTerrainData.SetHeights(0, 0, groundHm.getHeights());
		waterTerrainData.SetHeights(0, 0, waterHm.getHeights());
	}
}
