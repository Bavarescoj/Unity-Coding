using UnityEngine;
using System.Collections;

public class GlassTube : MonoBehaviour {

	[HideInInspector] public bool playerOnTube = false;
	private Controller2D controller2D;
	private GameObject player;
	private Rigidbody2D playerBody;
	private Collider2D playerBoxCollider;
	private Collider2D playerCircleCollider;

	private GameObject middle;
	private Collider2D middleCollider;

	private GameObject top;
	private Collider2D topCollider;

	private float wheel;
	private float vertical;


	void Start() {
		player = GameObject.FindWithTag ("Player");
		playerBody = player.GetComponent<Rigidbody2D> ();
		playerBoxCollider = player.GetComponent<Collider2D> ();
		playerCircleCollider = player.transform.FindChild ("Controller2D").GetComponent<Collider2D> ();
		controller2D = player.GetComponent<Controller2D> ();

		middle = this.transform.FindChild ("Middle").gameObject;
		middleCollider = middle.GetComponent<Collider2D> ();

		top = this.transform.FindChild ("Top").gameObject;
		topCollider = top.GetComponent<Collider2D> ();
	}

	void Update() {
		wheel = Input.GetAxis ("Mouse ScrollWheel");
		vertical = Input.GetAxis("Vertical");

		//ignore collisions between the ground colliders on the tube with the box collider on the player
		Physics2D.IgnoreCollision (playerBoxCollider, middleCollider, true);
		Physics2D.IgnoreCollision (playerBoxCollider, topCollider, true);

		//if gravity is zero and player is going up
		if (playerBody.gravityScale == 0 && playerBody.velocity.y > 0) {
			//ignore collisions with controller2d collider
			IgnoreCollisions ();
		} 
		//if player is going down and the player is not yet on the ground
		else if (playerBody.velocity.y < 0 && controller2D.onGround == false) {
			// allow collisions with controller2d collider
			AllowCollisions ();
		}
		//if the player is now on the ground, and wheel is moved down
		else if (controller2D.onGround == true && (wheel <= -0.1f || vertical <= -0.1f)) {
			//ignore collisions
			IgnoreCollisions ();
			//then allow them again in 1.5sec
			Invoke ("AllowCollisions", 1.5f);
		}
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.tag == "Player") {
			//to make the antigravity script know that we are on the tube
			playerOnTube = true;
		}
	}

	void OnTriggerExit2D (Collider2D other) {
		if (other.tag == "Player") {
			//to make the antigravity script know that we are not on the tube
			playerOnTube = false;
		}
	}

	void IgnoreCollisions() {
		Physics2D.IgnoreCollision (playerCircleCollider, middleCollider, true);
		Physics2D.IgnoreCollision (playerCircleCollider, topCollider, true);
	}

	void AllowCollisions() {
		Physics2D.IgnoreCollision (playerCircleCollider, middleCollider, false);
		Physics2D.IgnoreCollision (playerCircleCollider, topCollider, false);
	}
}

