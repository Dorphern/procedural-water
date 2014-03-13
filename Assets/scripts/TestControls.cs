using UnityEngine;
using System.Collections;

public class TestControls : MonoBehaviour {
	
	[SerializeField] private float simSizeMin = 32f;
	[SerializeField] private float simSizeMax = 256f;

	private string generationSeed = "";
	private float simulationSize = 1f;

	private TerrainData terrainData;
	private GameObject terrain;

	// Use this for initialization
	void Start () {
		this.simulationSize = this.simSizeMin;
		generateTerrain();
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(0, 0, 200, 500));
		GUILayout.BeginVertical("box");

		GUILayout.Box("walla", GUILayout.Height(200));
		this.generationSeed = GUILayout.TextField(this.generationSeed);
		this.simulationSize = GUILayout.HorizontalSlider(this.simulationSize, this.simSizeMin, this.simSizeMax);
		if (GUILayout.Button("Generate!")) {
			this.generateTerrain();
		}

		this.simulationSize = Mathf.Pow(2, Mathf.Round(Mathf.Log(this.simulationSize, 2)));

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}

	void generateTerrain() {
		if (this.terrain) Destroy(this.terrain);
		this.terrainData = new TerrainData();
		this.terrain = Terrain.CreateTerrainGameObject(terrainData);

		int res = (int)(this.simulationSize + 1f);
		int size = res * 2;

		int height = 15;

		this.terrainData.alphamapResolution = res;
		this.terrainData.heightmapResolution = res;
		this.terrainData.SetDetailResolution(res, 8);

		this.terrainData.size = new Vector3(size, height, size);

		this.terrain.transform.position = new Vector3(-this.terrainData.size.x / 2f, 0f, -this.terrainData.size.z / 2f);

		TerrainGenerator generator = new BasicFiniteTerrain(this.generationSeed);
		generator.setSize(res, res);
		Heightmap heightmap = generator.generateTerrain();

		terrainData.SetHeights(0, 0, heightmap.getHeights());
	}
}
