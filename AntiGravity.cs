using UnityEngine;
using System.Collections;

public class AntiGravity : MonoBehaviour {
	[Tooltip("Determines if the gameobject can use the ANTIGRAVITY ability")]
	public bool antiGravActivated = false;

	private Rigidbody2D mainBody;
	private float wheel;

	private bool takingOff = false;
	private bool levitating = false;

	private bool alreadyGravitating = false;

	private Controller2D controller2D;
	private GlassTube glassTube;

	[SerializeField] private float useTime;
	private float totalTime;
	private bool barInstantiated = false;
	private GameObject bar;
	private Animator barAnimator;

	[HideInInspector]
	public float vertical;

	// Use this for initialization
	void Start () {
		mainBody = this.gameObject.GetComponent<Rigidbody2D> ();
		controller2D = this.gameObject.GetComponent<Controller2D> ();

		if (GameObject.Find ("Glass Tube")) {
			glassTube = GameObject.Find ("Glass Tube").GetComponent<GlassTube> ();
		}
	}
	
	// Update is called once per frame
	void Update () {

		wheel = Input.GetAxis ("Mouse ScrollWheel");
		vertical = Input.GetAxis("Vertical");

		if (antiGravActivated == true) {
			
			// if glasstube exists
			if (glassTube) {
				//if player is inside of it, make the special antigravity
				if (glassTube.playerOnTube == true) {
					OnTube ();
				} 
			} 

			//if glasstube doesnt exist or player isn't inside of it, do normal anti gravity
			if (glassTube == null || glassTube.playerOnTube == false) {

				//if taking off hasnt taken place
				if (takingOff == false) {
					//gravity to 1 in case we were inside the glass tube
					mainBody.gravityScale = 1;

					//if scroll wheel is moved up and is not gravitating yet
					if (alreadyGravitating == false && (wheel >= 0.1f || vertical >= 0.1f)) {
						// if player has negative y velocity, that means he was falling, so the whole body's velocity is taken to zero to simulate real falling
						if (controller2D.onGround == true && controller2D.jump == false) {
							controller2D.jump = true;
						}
						// state that taking off already happened
						takingOff = true;
					}
				}

				// if player already took off
				if (takingOff == true) {
					//control antigravity duration
					DurationControl();

					//if is not on the ground, he is falling (y velocity less than 0) && hasnt levitated
					if (controller2D.onGround == false && mainBody.velocity.y < 0 && levitating == false) {
						//player is antigravitating, used for reference to make player do it just once, until he touches the floor again
						alreadyGravitating = true;
						//turn gravity off 
						mainBody.gravityScale = 0;
						// state levitation already happened
						levitating = true;
					}
				}
				
				//if levitating
				if (levitating == true) {

					// make velocity on y direction zero, to avoid zero gravity to move the character higher than it should
					mainBody.velocity = new Vector2 (mainBody.velocity.x, 0);

					// if player is not/stopped using WASD or keys to move player, make velocity zero, in order to avoid the zero gravity to continue moving the player
					if (Mathf.Abs (controller2D.horizontal) <= 0) {
						mainBody.velocity = new Vector2 (0, 0);
					}

					//if scroll wheel is moved down while levitating or player touched ground
					if (wheel <= -0.1f || vertical <= -0.1f || controller2D.onGround == true) {
						// turn off antigravity
						OffAntiGravity ();
					}
				}

				// if player is already gravitating and is on floor
				if (alreadyGravitating == true && controller2D.onGround == true) {
					//he can proceed to anti gravitate again
					alreadyGravitating = false;
				}
			}
		}
	}

	void OffAntiGravity() {
		mainBody.gravityScale = 1;
		takingOff = false;
		levitating = false;

		if (bar) {
			barInstantiated = false;
			Destroy (bar);
		}
	}

	void OnTube() {
		//change gravity inside the tube once to 0.5
		if (mainBody.gravityScale == 1) {
			mainBody.gravityScale = 0.5f;
		}

		//if scroll is moved up and gravity scale is the normal inside the tube
		if ((wheel >= 0.1f || vertical >= 0.1f) && mainBody.gravityScale == 0.5f) {
			//add a jump force to the character to make it go to the air
			mainBody.AddForce (new Vector2 (0, controller2D.jumpForce));
			// turn gravity to zero
			mainBody.gravityScale = 0;
		}

		//if gravity scale is zero, then slow down the x movement of the player
		if (mainBody.gravityScale == 0) {
			mainBody.velocity = new Vector2 (0, mainBody.velocity.y);

			// if scroll is moved down while having zero gravity or player is on ground due to falling (rapid gravity change can cause having 0 gravity and falling), if the velocity check is not used...
			//...if the player is on ground and wheel is moved up, a jump is added, but then this code would change gravity quickly to 0.5, which would cause several jump forces to be added
			if (wheel <= -0.1f || vertical <= -0.1f || (controller2D.onGround == true && mainBody.velocity.y < 0)) {
				//turn gravity to 0.5
				mainBody.gravityScale = 0.5f;
			}
		}
	}

	void DurationControl() {
		//if bar hasnt been isntantiated
		if (barInstantiated == false) {
			barInstantiated = true;
			//instatiate it
			bar = Instantiate (Resources.Load ("Bar"), this.gameObject.transform.position + new Vector3 (-1, 0.7f, 0), Quaternion.identity) as GameObject;
			//set it's parent the bars child in the character
			bar.transform.parent = this.gameObject.transform.FindChild("Bars").transform;
			barAnimator = bar.GetComponent<Animator> ();
			//set the animation speed according to the use time 
			barAnimator.speed /= useTime;
			totalTime = Time.time + useTime;
		}
			
		if (barInstantiated == true) {
			//if use time already elapsed
			if (Time.time > totalTime) {
				//turn off gravity
				//inside of this method, bar wil be destroyed
				OffAntiGravity ();
			}
		}
	}
}

