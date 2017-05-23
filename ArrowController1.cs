using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowController1 : MonoBehaviour {

	[HideInInspector]
	public bool facingRight = true;		// For determining which way the player is currently facing.

	[HideInInspector]
	public bool jump = false; 			// Condition for whether the player should jump.

	private Transform checkFloor;
	private Rigidbody2D playerBody;

	[HideInInspector]
	public bool onGround = false;

	[SerializeField]
	private float initialSpeed = 2.0f; 		//initial speed the gameobject can move

	[SerializeField]
	private float regularSpeed = 2.0f; 		//maximum speed the gameobject can move

	[HideInInspector]
	public float horizontal2;

	public ParticleSystem particles;
	private ParticleSystem.NoiseModule noiseModule;
	private ParticleSystem.MainModule mainModule;

	private float timer = 0;
	private bool shootPressed = false;

	public float chargingTime = 0;
	public float noiseFrequency = 0;
	public float simSpeed = 0;

	private GameObject chargeLight;
	private Animator lightAnim;

	private GameObject cross;
	private Animator crossAnim;

	// Use this for initialization
	void Start () {
		playerBody = this.GetComponent<Rigidbody2D> ();
		noiseModule = particles.noise;
		noiseModule.frequency = noiseFrequency;

		mainModule = particles.main;

		mainModule.simulationSpeed = simSpeed;

		cross = this.transform.Find("Cross").gameObject;
		crossAnim = cross.GetComponent <Animator>();

		chargeLight = cross.transform.Find ("Light").gameObject;
		lightAnim = chargeLight.GetComponent<Animator>();


	}

	void Update() {
		Shooting ();

		horizontal2 = Input.GetAxis ("HorizontalRight");
	}

	void FixedUpdate() {
		if (Mathf.Abs (horizontal2) >= 0 && Mathf.Abs (horizontal2) <= 0.5f) {
			// set the velocity in X to the initial speed
			playerBody.velocity = new Vector2 (horizontal2 * initialSpeed, playerBody.velocity.y);
		} else if (Mathf.Abs (horizontal2) > 0.5f) {
			// set the velocity in X to the max speed
			playerBody.velocity = new Vector2 (horizontal2 * regularSpeed, playerBody.velocity.y);
		}
	}

	void Shooting() {
		if (Input.GetButtonDown ("Fire") && particles.particleCount == 0 && shootPressed == false) {
			noiseModule.frequency = noiseFrequency;
			mainModule.simulationSpeed = simSpeed;
			shootPressed = true;
			lightAnim.SetBool ("On", true);
			lightAnim.speed /= chargingTime;
		} 

		if (shootPressed == true) {
			timer += Time.deltaTime;
			if (timer >= chargingTime) {
				crossAnim.SetBool ("Max", true);
			}

			if (Input.GetButtonUp("Fire")) {
				lightAnim.SetBool ("On", false);
				crossAnim.SetBool ("Max", false);
				lightAnim.speed = 1;

				if (timer < chargingTime) {
					noiseModule.frequency -= (timer + 1);
					mainModule.simulationSpeed += (timer + 1);
				} else {
					noiseModule.frequency = 0;
					mainModule.simulationSpeed += (chargingTime + 2);
				}
					
				particles.Play ();
				shootPressed = false;
				timer = 0;
			}
		}
	}
}
