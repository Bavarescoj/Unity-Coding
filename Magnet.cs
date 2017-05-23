using UnityEngine;
using System.Collections;

public class Magnet : MonoBehaviour {

	[Tooltip("Determines if the gameobject can use the PULL ability")]
	public bool pullActivated = false;
	[Tooltip("Determines if the gameobject can use the PUSH ability")]
	public bool pushActivated = false;

	[SerializeField] private PointEffector2D effector;
	[HideInInspector] public float effectorForce;
	bool activatePush = false;
	bool pulling = false;

	private Transform mainTransform;
	private Vector3 mousePosition;

	private BoxCollider2D boxCollider;
	private Collider2D[] insideTrigger;

	private bool pushed = false;
	private bool kinematicChanged = false;

	// Use this for initialization
	void Start () {
		//no object will interact with the point effector yet
		effector.forceMagnitude = 0;
		//when available, will only attract objects that are inside the point effector and that belong to the gravity layer
		effector.colliderMask = 1 << LayerMask.NameToLayer ("Gravity");
		effectorForce = -80f;

		mainTransform = this.gameObject.GetComponent<Transform> ();
		boxCollider = this.gameObject.GetComponent<BoxCollider2D> ();
	}

	// Update is called once per frame
	void Update () {
		// getting the circle that will act as the area of action for the throw
		insideTrigger = Physics2D.OverlapCircleAll (this.gameObject.transform.position, 1, 1 << LayerMask.NameToLayer ("Gravity"));

		//position of the mouse on relation with the world position
		mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z)));

		//if the mouse is clicked && player has pull ability enabled
		if (Input.GetButton ("Fire1") && pullActivated && pushed == false) {
			pulling = true;
			effector.forceMagnitude = effectorForce;
			//if second button on the mosue is clicked
			if (Input.GetButton ("Fire2") && pushActivated ) {
				//activate the repulsion effect
				activatePush = true;
			} else if (!Input.GetButton ("Fire2")) {
				//if it's not being clicked or stopped being clicked before a body could be thrown, deactivate the repulsion effect
				activatePush = false;
			}
		} 
		else if (!Input.GetButton("Fire1")) {
			pulling = false;
			// just after thrown, left click released and then clicked again can the character attract things
			pushed = false;
			//if the mouse is not being clicked, no object will be attracted
			effector.forceMagnitude = 0;
		}

		if (!Input.GetButton ("Fire2")) {
			//if it's not being clicked or stopped being clicked before a body could be thrown, deactivate the repulsion effect
			activatePush = false;
		}
	}

	void OnTriggerStay2D (Collider2D other) {

		if (other.gameObject.layer == LayerMask.NameToLayer ("Gravity")) {
			//get distance between character and object inside the collider
			float distance = DistanceBetweenBodies (mainTransform, other.gameObject.transform);

			//if object has the kinematic tag and we are pulling, change on that process its kinematic value to be attracted
			if (other.gameObject.tag == "Kinematic") {
				if (pulling == true) {
					other.attachedRigidbody.isKinematic = false;
				} else {
					other.attachedRigidbody.isKinematic = true;
				}
			}

			//if repulsion effect is activated and objects that belong to the gravity layer are inside triger collider
			if (activatePush == true) {
				for (int i = 0; i < insideTrigger.Length; i++) {
					//get the direction of the force, will be related to the mouse position
					Vector3 forceDirection = ForceDirection (mousePosition, mainTransform.position);
					//set a force
					float force = ForceMagnitude (mousePosition, mainTransform.position);
					//force vector with force number and direction
					Vector3 forceVector = forceDirection * force;
					//apply it to the bodies on the gravity layer
					insideTrigger [i].attachedRigidbody.AddForce (forceVector);
				}
				// avoid attraction just after thrown
				effector.forceMagnitude = 0;
				pushed = true;
			}

			//avoid collision with box collider on character
			if (Input.GetButton ("Fire1") && pullActivated && other.tag != "AllowBoxCollision") {
				Physics2D.IgnoreCollision (boxCollider, other, true);
			} else {
				Physics2D.IgnoreCollision (boxCollider, other, false);
			}
		}
	}

	public float DistanceBetweenBodies (Transform thisTransform, Transform objectiveTransform) {
		return (Mathf.Abs(Vector3.Magnitude(thisTransform.position - objectiveTransform.position)));
	}

	public Vector3 ForceDirection (Vector3 _end, Vector3 _beginning) {
		return ((_end - _beginning).normalized);
	}

	public float ForceMagnitude (Vector3 _end, Vector3 _beginning) {
		return ((Mathf.Abs (Vector3.Magnitude (_end - _beginning))) * 150f);
	}
}
