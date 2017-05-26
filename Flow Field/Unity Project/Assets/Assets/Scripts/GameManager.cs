using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public int quantity;
	private GameObject[] vehicles;
	private GameObject flowfield;

	// Use this for initialization
	void Start () {
		vehicles = new GameObject[quantity];
		for (int i = 0; i < quantity; i++) {
			vehicles [i] = Instantiate(Resources.Load ("Vehicle") as GameObject);
			if (GameObject.Find ("Elements") == false) {
				GameObject elements = new GameObject("Elements");
			}
			vehicles[i].transform.SetParent(GameObject.Find("Elements").transform);
		}

		flowfield = Instantiate (Resources.Load ("Flowfield") as GameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
