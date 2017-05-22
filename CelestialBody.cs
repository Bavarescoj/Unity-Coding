using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class CelestialBody : MonoBehaviour {

	private Rigidbody2D body;
	public Rigidbody2D Body {
		get { return body; }
		set { body = value; }
	}

	private float mass;
	public float Mass {
		get { return mass; }
		set { mass = value; }
	}

	[Range(0f, 360f)][Tooltip("Angle of rotation for the planet")][SerializeField]
	private float rotation;
	public float Rotation {
		get { return rotation; }
		set { rotation = value; }
	}

	private Transform bodyTransform;
	public Transform BodyTransform {
		get { return bodyTransform; }
		set { bodyTransform = value; }
	}

	private Collider2D[] insideTrigger;

	private CircleCollider2D circleCol;

	public bool attracting = true;

	private Collider2D[] celestialBodies;
	public Collider2D[] CelestialBodies {
		get { return celestialBodies; }
		set { celestialBodies = value; }
	}

	[SerializeField]
	private float attractionRadius = 0;
	public float AttractionRadius {
		get { return attractionRadius; }
		set { attractionRadius = value; } 
	}
		
	public enum SpeedDirection {Up, Down, Left, Right, Random, None}; 
	public SpeedDirection speedDirection;

	[SerializeField]
	private float initialSpeed = 0;
	public float InitialSpeed {
		get { return initialSpeed; }
		set { initialSpeed = value; } 
	}

	[SerializeField]
	private ParticleSystem explosion;
	public ParticleSystem Explosion {
		get { return explosion; }
		set { explosion = value; }
	}

	private Planet player;
	private float elapsedTime = 0f;

	[HideInInspector]
	public float distancePlayer = 0f;

	protected virtual void Start() {
		Body = this.GetComponent<Rigidbody2D> ();
		Mass = Body.mass;

		BodyTransform = this.GetComponent<Transform> ();
		circleCol = this.gameObject.GetComponent<CircleCollider2D> ();

		if (speedDirection != SpeedDirection.None)
			Body.AddForce(Direction(speedDirection) * initialSpeed);

		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Planet> ();
	}

	protected virtual void Update() {
		//creating the gravitational circle on which this object has an effect over other celestial bodies
		CelestialBodies = Physics2D.OverlapCircleAll(BodyTransform.position, attractionRadius, 1 << LayerMask.NameToLayer ("Celestial"));
		distancePlayer = DistanceBetweenBodies (transform.position, player.transform.position);
	}

	protected virtual void FixedUpdate() {
		Rotate ();

		if (CelestialBodies != null) {
			if (attracting == true) {
				for (int i = 0; i < CelestialBodies.Length; i++) {
					//the object inside circle isn't this object
					if (CelestialBodies [i].name != this.gameObject.name) {
						//attract objects
						GravitationalPull (this.gameObject.GetComponent<CelestialBody> (), CelestialBodies [i].GetComponent<CelestialBody> ());
						// if the body being attracted is the player
						if (CelestialBodies [i].tag == "Player") {
							AttractingPlayer ();
						}
					}
				}
			}
			// if in the array isn't the player but this object was affecting it previously
			if (((Array.Exists (CelestialBodies, element => element.tag == "Player") == false) || attracting == false) && (Planet.affectingPlayer.ContainsKey (this) == true)) {
				// delete it from the list
				Planet.affectingPlayer.Remove (this);
			}
		}
	}

	public void Rotate() {
		BodyTransform.Rotate (Vector3.back * Rotation * Time.deltaTime); 
	}

	public void GravitationalPull (CelestialBody main, CelestialBody objective) {

		float distance = DistanceBetweenBodies (main.BodyTransform.position, objective.BodyTransform.position);

		//newton's law of universal gravitation
		float force = (GameManager.gravitationalConstant * main.Mass * objective.Mass)/(Mathf.Pow(distance, 2));

		//direction of vector from the objective towards the main body (attraction)
		Vector3 forceDirection = ForceDirection(main.BodyTransform.position, objective.BodyTransform.position);
		Vector3 forceVector = forceDirection * force;

		objective.Body.AddForce (forceVector);
	}

	//distance between the bodies
	public float DistanceBetweenBodies (Vector3 mainBodyPosition, Vector3 objectiveBodyPosition) {
		return (Mathf.Abs(Vector3.Magnitude(mainBodyPosition - objectiveBodyPosition)));
	}

	//direction of the force
	public Vector3 ForceDirection (Vector3 mainBodyPosition, Vector3 objectiveBodyPosition) {
		return ((mainBodyPosition - objectiveBodyPosition).normalized);
	}

	//explosion when collision
	public void OnCollisionEnter2D(Collision2D coll) {
		this.gameObject.SetActive (false);
		Instantiate (explosion, this.transform.position, Quaternion.identity);
	}

	public void OnMouseOver() {
		if (Input.GetMouseButtonDown (0) == true) {
			attracting = !attracting;
		}
	}

	//initial direction of the celestial body
	public static Vector2 Direction(SpeedDirection sd) {
		switch (sd) {
		case SpeedDirection.Up:
			return Vector2.up;
		case SpeedDirection.Down:
			return Vector2.down;
		case SpeedDirection.Left:
			return Vector2.left;
		case SpeedDirection.Right:
			return Vector2.right;
		case SpeedDirection.Random:
			Vector2 random = new Vector2 (UnityEngine.Random.Range (0f, 1f), UnityEngine.Random.Range (0f, 1f));
			return random;
		default:
			return Vector2.zero;
		}
	}

	//drawing the attraction area for development easiness
	void OnDrawGizmosSelected() {
		Gizmos.DrawWireSphere (transform.position, attractionRadius);
	}

	public void AttractingPlayer() {
		// if this is the first time this object affects the player
		if (Planet.affectingPlayer.ContainsKey (this) == false) {
			// add this game object as one of the objects that is attracting the player
			Planet.affectingPlayer.Add (this, 0f);
			elapsedTime = 0f;
		}
		// if the object has been affecting te player for a while
		else if (Planet.affectingPlayer.ContainsKey (this) == true)
			elapsedTime += Time.deltaTime;
			Planet.affectingPlayer [this] = elapsedTime;
	}
}