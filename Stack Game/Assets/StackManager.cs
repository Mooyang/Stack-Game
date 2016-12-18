﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StackManager : MonoBehaviour {
	private const float BOUND_SIZE = 4.0f;
	private const float STACK_MOVING_SPEED = 5.0f;
	private const float ERROR_MARGIN = 0.25f;
	private const float STACK_BOUNDS_GAIN = 0.25f;
	private const int COMBO_START_GAIN = 4;
	
	private GameObject[] theStack;
	public GameObject Button;
	public Color32[] gameColors = new Color32[4];
	public Material RubbleMat;
	public Text CurrentScore;
	public Text scoreText;
	public Text highScoreText;

	private int scoreCount = 1 ;
	private int highScoreCount = 1;
	private int stackIndex;
	private int combo;

	private float tileTrasition = 0.0f;
	private float tileSpeed = 2.5f; 
	private float secondaryPosition;

	private bool isMovingOnX = true;
	private bool GameOver = false;

	private Vector3 desiredPosition ;
	private Vector3 lastTilePosition;
	private Vector2 stackBounds = new Vector2(BOUND_SIZE, BOUND_SIZE);

	void Awake(){
		Application.targetFrameRate = 30;
	}

	void Start () {
		
		theStack = new GameObject[transform.childCount];
		for(int i = 0; i < transform.childCount; i++){
			theStack [i] = transform.GetChild (i).gameObject;
			ColorMesh (theStack[i].GetComponent<MeshFilter>().mesh);
		}
		stackIndex = transform.childCount - 1;

		Button.SetActive (false);

		if(PlayerPrefs.GetInt("HighScore") != null){
			highScoreCount = PlayerPrefs.GetInt ("HighScore");
		}
	}
	
	void Update () {
		if(!GameOver)
			CurrentScore.text = "score : " + scoreCount;

		if (Input.GetMouseButtonDown (0)) {
			if (PlaceTile ()) {
				SpawnTile ();
				scoreCount++;
			} else {
				EndGame ();
			}
		}
	
		MoveTile ();

		//move stack down
		transform.position = Vector3.Lerp(transform.position, desiredPosition, STACK_MOVING_SPEED * Time.deltaTime);
	} 

	private void MoveTile(){

		if (GameOver) 
		{
			return;
		}
		
		tileTrasition += Time.deltaTime * tileSpeed;
		if(isMovingOnX)
			theStack [stackIndex].transform.localPosition = new Vector3 (Mathf.Sin (tileTrasition) * BOUND_SIZE, scoreCount, secondaryPosition);
		else
			theStack [stackIndex].transform.localPosition = new Vector3 (secondaryPosition, scoreCount, Mathf.Sin (tileTrasition) * BOUND_SIZE);
	}

	private void createRubble(Vector3 pos, Vector3 scale){
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Cube);
		go.transform.localPosition = pos;
		go.transform.localScale = scale;
		go.AddComponent<Rigidbody> ();

		go.GetComponent<MeshRenderer> ().material = RubbleMat;
		ColorMesh (go.GetComponent<MeshFilter>().mesh);

	}

	private void ColorMesh(Mesh mesh){
		Vector3[] vertices = mesh.vertices;
		Color32[] colors = new Color32[vertices.Length];
		float f = Mathf.Sin (scoreCount * 0.25f);

		for (int i = 0; i < vertices.Length; i++)
			colors [i] = Lerp4 (gameColors[0], gameColors[1], gameColors[2], gameColors[3], f);

		mesh.colors32 = colors;
	}

	private void SpawnTile(){

		lastTilePosition = theStack [stackIndex].transform.localPosition;
		stackIndex--;
		if (stackIndex < 0)
			stackIndex = transform.childCount - 1;

		desiredPosition = Vector3.down * scoreCount;
		theStack[stackIndex].transform.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);
//		theStack [stackIndex].transform.localPosition = new Vector3 (0, scoreCount, 0 );

		ColorMesh (theStack [stackIndex].GetComponent<MeshFilter> ().mesh);
	}

	private bool PlaceTile(){

		Transform t = theStack [stackIndex].transform;

		if (isMovingOnX) {
			float deltaX = lastTilePosition.x - t.position.x; 
			if (Mathf.Abs (deltaX) > ERROR_MARGIN) {
				
				//CUT THE TILE
				combo = 0;
				stackBounds.x -= Mathf.Abs (deltaX);
				if (stackBounds.x <= 0)
					return false;

				float middle = lastTilePosition.x + t.position.x / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

				createRubble (
					new Vector3 ((t.position.x > 0) ? t.position.x + (t.localScale.x/2) : t.position.x - (t.localScale.x/2)
						, t.position.y, t.position.z),
					new Vector3 (Mathf.Abs (deltaX), 1, t.localScale.z)
				);

				t.localPosition = new Vector3 (middle - (lastTilePosition.x / 2), scoreCount, lastTilePosition.z);
			} 
			else {
				if (combo >= COMBO_START_GAIN) {
					stackBounds.x += STACK_BOUNDS_GAIN;
					Debug.Log ("GainX");

					if (stackBounds.x > BOUND_SIZE) {
						stackBounds.x = BOUND_SIZE;
						Debug.Log ("Over GainX");
					}

				}
				combo++;
				Debug.Log ("combo");
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
				
		} 
			
		else {
			float deltaZ = lastTilePosition.z - t.position.z; 
			if (Mathf.Abs (deltaZ) > ERROR_MARGIN) {

				//CUT THE TILE
				combo = 0;
				stackBounds.y -= Mathf.Abs (deltaZ);
				if (stackBounds.y <= 0)
					return false;

				float middle = lastTilePosition.z + t.position.z / 2;
				t.localScale = new Vector3 (stackBounds.x, 1, stackBounds.y);

				createRubble (
					new Vector3 (t.position.x, t.position.y, 
						(t.position.z > 0) ? t.position.z + (t.localScale.z/2) : t.position.z - (t.localScale.z/2)),
					new Vector3 (t.localScale.x, 1, Mathf.Abs (deltaZ))
				);

				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, middle - (lastTilePosition.z/2));
			}
			else {
				if (combo >= COMBO_START_GAIN) {
					stackBounds.y += STACK_BOUNDS_GAIN;
					Debug.Log ("GainY");

					if (stackBounds.y > BOUND_SIZE) {
						stackBounds.y = BOUND_SIZE;
						Debug.Log ("Over GainZ");
					}
				}
				combo++;
				Debug.Log ("combo");
				t.localPosition = new Vector3 (lastTilePosition.x, scoreCount, lastTilePosition.z);
			}
		}
			
		secondaryPosition = (isMovingOnX) 
			? t.localPosition.x 
			: t.localPosition.z;
		isMovingOnX = !isMovingOnX;
		return true;
	}

	private Color32 Lerp4(Color32 a, Color32 b, Color32 c, Color32 d,float t){
		if (t < 0.33f)
			return Color.Lerp (a, b, t / 0.33f);
		else if (t < 0.66f)
			return Color.Lerp (b, c, (t - 0.33f) / 0.33f);
		else
			return Color.Lerp (c, d, (t - 0.66f) / 0.66f);
	}

	private void EndGame(){
		Debug.Log ("Game Over");
		GameOver = true;
		theStack [stackIndex].AddComponent<Rigidbody> ();
		Button.SetActive (true);
		CurrentScore.text = "";
		SetScore ();
	}

	void SetScore(){
		scoreText.text = "score : " + scoreCount.ToString ();
		highScoreText.text = highScoreCount.ToString ();

		if (scoreCount > highScoreCount) {
			highScoreCount = scoreCount;
			highScoreText.text = highScoreCount.ToString ();
			PlayerPrefs.SetInt ("HighScore", highScoreCount);
		}
	}

	public void LoadScene(){
		Application.LoadLevel ("GameStack");
	}
		
}
