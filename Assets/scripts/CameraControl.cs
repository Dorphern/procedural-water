using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

	[SerializeField] private float speed = 1f; 
	[SerializeField] private float distance = 100f;

	private Vector2 rotation;
	private Vector3 mouseStart;

	void Start () {
		rotation = new Vector2(90, 0);
		SetPosition();
	}

	// Update is called once per frame
	void Update () {

		if (Input.GetMouseButtonDown(0)) {
			mouseStart = Input.mousePosition;
		} else if (Input.GetMouseButton(0)) {
			Vector3 mouseDiff = Input.mousePosition - mouseStart;
			rotation.x += mouseDiff.y * Time.deltaTime * speed;
			rotation.y -= mouseDiff.x * Time.deltaTime * speed;
			rotation.x = Mathf.Clamp(rotation.x, 90f, 170f);
			SetPosition();
		}

	}

	void SetPosition() {
		Vector3 eulerAngles = new Vector3(0f, 0f, 0f);
		Vector3 position = new Vector3(0f, 0f, 0f);

		float rotX = this.rotation.x * Mathf.Deg2Rad;
		float rotY = this.rotation.y * Mathf.Deg2Rad;

		position.y = Mathf.Sin(rotX) * this.distance;
		position.x = Mathf.Cos(rotY) * Mathf.Cos(rotX) * this.distance;
		position.z = Mathf.Sin(rotY) * Mathf.Cos(rotX) * this.distance;

		eulerAngles.x = 180 - rotation.x;
		eulerAngles.y = 90 - rotation.y;

		this.transform.position = position;
		this.transform.eulerAngles = eulerAngles;
	}
}
