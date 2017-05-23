using UnityEngine;
using System.Collections;

public class Controller2D : MonoBehaviour {

	[HideInInspector] public bool facingRight = true;		// For determining which way the player is currently facing.
	[HideInInspector] public bool jump = false; 			// Condition for whether the player should jump.

	[HideInInspector] public float horizontal;

	[SerializeField]private float initialSpeed = 2.0f; 		//initial speed the gameobject can move
	[SerializeField] private float regularSpeed = 2.0f; 		//maximum speed the gameobject can move

	public float jumpForce = 30.0f;

	private Transform circleController;
	private Transform checkFloor;
	[HideInInspector] public bool onGround = false;
	[SerializeField] private Rigidbody2D objectBody;
	private Animator animationCharacter;
	[SerializeField] private AudioClip footsteps;

	private AudioSource musicSource;

	private GameObject bars;

	private Rigidbody2D _parentBody;

	// Use this for initialization
	void Start() {
		circleController = transform.Find ("Controller2D");
		checkFloor = transform.Find ("checkFloor");
		objectBody = GetComponent<Rigidbody2D> ();
		animationCharacter = this.gameObject.GetComponent<Animator> ();
		musicSource = this.gameObject.GetComponent<AudioSource> ();
		bars = this.gameObject.transform.FindChild("Bars").gameObject;
	}


	// Update is called once per frame
	void Update() {
		//scale bars on x to avoid misleading moving
		BarsScaling ();
		onGround = Physics2D.Linecast(circleController.position, checkFloor.position, (1 << LayerMask.NameToLayer("Ground")) + (1 << LayerMask.NameToLayer("Gravity")) + (1 << LayerMask.NameToLayer("Ground2")));

		if (!onGround)
		{
			this.transform.SetParent (null);
			_parentBody = null;
		}

		if (Input.GetButtonDown ("Jump") && onGround) {
			jump = true;
		}
	}

	void FixedUpdate() {

		
		horizontal = Input.GetAxis("Horizontal");
		// The Speed animator parameter is set to the absolute value of the horizontal input.
		animationCharacter.SetFloat("Speed", Mathf.Abs(horizontal));

		//sound effects
		SFX ();

		if (Mathf.Abs (horizontal) >= 0 && Mathf.Abs (horizontal) <= 0.5f) {
			// set the velocity in X to the initial speed
			objectBody.velocity = new Vector2 (horizontal * initialSpeed, objectBody.velocity.y);
		} 
		else if (Mathf.Abs (horizontal) > 0.5f) {
			// set the velocity in X to the max speed
			objectBody.velocity = new Vector2 (horizontal * regularSpeed, objectBody.velocity.y);
		}

		//since I'm using XOR gates, if horizontal > 0 returns true (player is moving to the right), and if facingright is false, it will flip the player
		//if horizontal > 0 returns false (player is moving to the left), and if facingright is true, it will flip the player
		if (horizontal != 0) {
			if((horizontal > 0) ^ facingRight)
				FlipPlayer();
		}

		// If the jump button (spacebar was pressed)
		if(jump)
		{
			// Add a vertical force to the player.
			objectBody.AddForce(new Vector2(0f, jumpForce));

			animationCharacter.SetTrigger("Jump");

			// Make sure the player can't jump again until the jump conditions from Update are satisfied.
			jump = false;
		}

	}

	void FlipPlayer()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		//if true, it will change to false, and viceversa

		// Multiply the player's x local scale by -1. A negative scale will make it flip
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		//then puts the result into the gameobject
		transform.localScale = theScale;
	}

	void OnCollisionEnter2D( Collision2D colobj) {
		//attach the player as the platform child
		if (colobj.gameObject.tag == "Platform") {
			if (this.transform.position.y > colobj.transform.position.y) {
				this.transform.SetParent (colobj.transform);
				_parentBody = colobj.gameObject.GetComponent<Rigidbody2D>();
		
			}
		}
	}

	void OnCollisionExit2D(Collision2D colobj) {
		if (colobj.gameObject.tag == "Platform") {
			this.transform.SetParent (null);
			_parentBody = null;
		}
	}

	void SFX() {
		if (animationCharacter.GetFloat("Speed") > 0.2f && onGround == true && musicSource.isPlaying == false) {
			musicSource.clip = footsteps;
			musicSource.Play ();
		} else if (animationCharacter.GetFloat("Speed") < 0.2f || onGround == false) {
			musicSource.Stop ();
			musicSource.clip = null;
		}
	}

	void BarsScaling() {
		if (this.gameObject.transform.localScale.x == -1) {
			bars.transform.localScale = new Vector3 (-1, 1, 1);
		} else if (this.gameObject.transform.localScale.x == 1) {
			bars.transform.localScale = new Vector3 (1, 1, 1);
		}
	}
}