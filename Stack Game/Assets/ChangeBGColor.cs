using UnityEngine;
using System.Collections;

public class ChangeBGColor : MonoBehaviour {
	
	public Color[] colors;
	public float duration;
	Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera> ();
		camera.clearFlags = CameraClearFlags.SolidColor;
	
	}
	
	// Update is called once per frame
	void Update () {
		float t = Mathf.PingPong (Time.time, duration)/duration;
		camera.backgroundColor = Color.Lerp(colors[0] , colors[1], t);
	
	}
}
