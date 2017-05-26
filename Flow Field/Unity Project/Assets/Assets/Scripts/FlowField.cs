﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour {

	public int resolution;
	int columns;
	int rows;

	Vector2[,] field;

	// Use this for initialization
	void Start () {
		columns = Camera.main.pixelWidth / resolution;
		rows = Camera.main.pixelHeight / resolution;
		field = new Vector2[columns, rows];
		Flow ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0) == true) {
			Flow ();
		}
		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
				DrawField (field[i, j], i * resolution, j * resolution);
			}
		}
	}

	void Flow() {
		float seed = Mathf.Floor(Random.Range(0, 1000));
		float xoff = 0;
		for (int i = 0; i < columns; i++) {
			float yoff = 0;
			for (int j = 0; j < rows; j++) {
				float theta = Map(Mathf.PerlinNoise(seed + xoff, seed + yoff), 0, 1, 0, Mathf.PI * 2);
				field [i, j] = new Vector2 (Mathf.Cos (theta), Mathf.Sin (theta));
				Vector2.ClampMagnitude (field[i, j], 1f);
				yoff += 0.1f;
			}
			xoff += 0.1f;
		}
	}

	float Map(float value, float low1, float high1, float low2, float high2) {
		return low2 + ((value-low1)*(high2-low2))/(high1-low1);
	}

	public Vector2 CurrentField(Vector2 _position) {
		Vector3 pixelPosition = Camera.main.WorldToScreenPoint (_position);
		int column = Mathf.Clamp ((Mathf.RoundToInt( pixelPosition.x) / resolution), 0, columns - 1);
		int row = Mathf.Clamp ((Mathf.RoundToInt(pixelPosition.y) / resolution), 0, rows - 1);
		return field [column, row];		
	} 

	void DrawField(Vector2 _vector, int _x, int _y) {
		Vector2 _start = Camera.main.ScreenToWorldPoint (new Vector3(_x + resolution, _y + resolution, -Camera.main.transform.position.z));
		Debug.DrawRay(_start, _vector * 0.5f, Color.black);
	}
}
