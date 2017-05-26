using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Vehicles : MonoBehaviour {

	public float size;
	float maxspeed;
	float maxforce;

	private Vector3[] vertices = new Vector3[3];
	private int[] triangles = new int[3];
	private Mesh mesh;
	private FlowField flowfield;
	private Rigidbody2D body;

	void Awake() {
		DrawObject ();
	}

	// Use this for initialization
	void Start () {
		this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(0, Camera.main.pixelWidth), Random.Range(0, Camera.main.pixelHeight), -Camera.main.transform.position.z));
		maxspeed = Random.Range (3, 5);
		maxforce = Random.Range (1f, 4f);
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameObject.FindWithTag ("Flowfield") != null) {
			flowfield = GameObject.FindWithTag ("Flowfield").GetComponent<FlowField>();	
			Follow ();
		}
		Borders ();
		Rotation (body.velocity);
	}

	void DrawObject() {
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();

		// drawing a triangle 
		vertices[0] = new Vector3(this.transform.position.x, this.transform.position.y + (size * 2), this.transform.position.z);
		vertices[1] = new Vector3(this.transform.position.x + size, this.transform.position.y - (size * 2), this.transform.position.z);
		vertices[2] = new Vector3(this.transform.position.x - size, this.transform.position.y - (size * 2), this.transform.position.z);

		for (int i = 0; i < triangles.Length; i++) {
			triangles [i] = i;
		}
			
		mesh.vertices = vertices;
		mesh.triangles = triangles;
	}

	void Follow() {
		Vector2 desired = flowfield.CurrentField (this.transform.position);
		desired *= maxspeed;
		Vector2 steering = desired - body.velocity;
		Vector2.ClampMagnitude (steering, maxforce);
		body.AddForce (steering);
	}

	void Borders() {
		Vector3 pixelPosition = Camera.main.WorldToScreenPoint (this.transform.position);
		if (pixelPosition.x < - size) transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth + size, pixelPosition.y, -Camera.main.transform.position.z));
		if (pixelPosition.y < - size) transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pixelPosition.x, Camera.main.pixelHeight + size, -Camera.main.transform.position.z));
		if (pixelPosition.x > Camera.main.pixelWidth + size) transform.position = Camera.main.ScreenToWorldPoint(new Vector3(-size, pixelPosition.y, -Camera.main.transform.position.z));
		if (pixelPosition.y > Camera.main.pixelHeight + size) transform.position = Camera.main.ScreenToWorldPoint(new Vector3(pixelPosition.x, -size, -Camera.main.transform.position.z));
	}

	void Rotation(Vector2 _velocity) {
		float angle = Mathf.Atan2 (_velocity.y, _velocity.x) * Mathf.Rad2Deg;
		//substract 90 to angle since triangle is pointing to 90 already
		angle -= 90f;
		transform.rotation = Quaternion.AngleAxis (angle, Vector3.forward);
	}

	/* //checking the vertices are in good position
	private void OnDrawGizmos () {
		if (vertices == null) {
			return;
		}
		Gizmos.color = Color.black;
		for (int i = 0; i < vertices.Length; i++) {
			Gizmos.DrawSphere(vertices[i], 0.1f);
		}
	}*/
}
