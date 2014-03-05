using UnityEngine;
using System.Collections;

public class TestControls : MonoBehaviour {

	private string generationSeed = "";
	private float simulationSize = 1f;
	[SerializeField] private float simSizeMin = 5.0f;
	[SerializeField] private float simSizeMax = 100.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI () {
		GUILayout.BeginArea(new Rect(0, 0, 200, 200));
		GUILayout.BeginVertical("box");

		GUILayout.Box("walla");
		this.generationSeed = GUILayout.TextField(this.generationSeed);
		this.simulationSize = GUILayout.HorizontalSlider(this.simulationSize, this.simSizeMin, this.simSizeMax);
		GUILayout.Button("Generate!");

		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
}
