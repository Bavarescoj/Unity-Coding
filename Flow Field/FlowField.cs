﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField : MonoBehaviour {

	[Tooltip("In pixels size")]
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
		columns = Camera.main.pixelWidth / resolution;
		rows = Camera.main.pixelHeight / resolution;
		if (Input.GetMouseButtonDown(0) == true) {
			Flow ();
		}
		for (int i = 0; i < columns; i++) {
			for (int j = 0; j < rows; j++) {
				DrawField (field[i, j], i * resolution, j * resolution, 0.3f);
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

	void DrawField(Vector2 _vector, int _x, int _y, float _arrowSize) {
		//finding the center spot of the grid box corresponding to the current flow arrow
		Vector2 _start = Camera.main.ScreenToWorldPoint (new Vector3(_x + resolution/2, _y + resolution/2, -Camera.main.transform.position.z));
		//moving the vector with the starting position of the arrow by half the arrow size in the flow direction, so when the arrow is drawn, it's exactly at the half of its own grid box
		_start += _vector * -_arrowSize/2;
		//point where the arrow will end, which is basically the sum of the starting position vector with the size of the flow arrow vector
		Vector2 _end = _start + _vector * _arrowSize;
		//drawing arrow
		Debug.DrawRay (_start, _vector * _arrowSize, new Color (0, 0, 0, 0.3f));
		//drawing vertices of arrow
		Debug.DrawRay(_end, Quaternion.AngleAxis(135, Vector3.forward) * _vector * _arrowSize/3f, new Color(0,0,0,0.3f));
		Debug.DrawRay(_end, Quaternion.AngleAxis(-135, Vector3.forward) * _vector * _arrowSize/3f, new Color(0,0,0,0.3f));

		//METHOD OF DRAWING FLOW FIELD BY USING DRAWLINE INSTEAD OF DRAWRAY

		//Vector2 _start = Camera.main.ScreenToWorldPoint (new Vector3(_x + resolution/2, _y + resolution/2, -Camera.main.transform.position.z));
		//_start += _vector * -_arrowSize/2;
		//Vector2 _end = _start + _vector * _arrowSize;
		//Debug.DrawLine (_start, _end, new Color(0,0,0,0.3f));
		//Vector2 _arrowEdge1 = _end + (Vector2)(Quaternion.AngleAxis(135, Vector3.forward) * _vector * _arrowSize/3f);
		//Vector2 _arrowEdge2 = _end + (Vector2)(Quaternion.AngleAxis(-135, Vector3.forward) * _vector * _arrowSize/3f);
		//Debug.DrawLine(_end, _arrowEdge1, Color.black);
		//Debug.DrawLine(_end, _arrowEdge2, Color.black);
	}
}
