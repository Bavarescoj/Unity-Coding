using UnityEngine;
using System.Collections;

public class GravitySwitch : MonoBehaviour {

	[Tooltip("Determines if the gameobject can use the GRAVITY SWITCH ability")]
	public bool gravitySwitchActivated = false;

	private CircleCollider2D triggerCollider;
	private float colliderRadius;

	private Collider2D[] insideTrigger;

	void Start() {
		//setting to the variable triggerCollider, the trigger circle collider 2d on the character
		TriggerCollider (ref triggerCollider);
	}
	// Update is called once per frame
	void Update () {
		// if there was a trigger circle collider 2d on the character
		if (triggerCollider != null) {
			//get the radius of that trigger
			colliderRadius = triggerCollider.radius;
			// if there was no trigger circle collider 2d
		} else {
			// set the radius to a safe number
			colliderRadius = 2.5f;
		}

		// getting the circle that will act as the area of action for the switch
		insideTrigger = Physics2D.OverlapCircleAll (this.gameObject.transform.position, colliderRadius, 1 << LayerMask.NameToLayer ("Gravity"));

		// if only the right mouse click is pushed, switch gravity once
		// also ensure player has the grav switch skill enabled
		if (!Input.GetButton ("Fire1") && Input.GetButtonDown ("Fire2") && gravitySwitchActivated) {
			SwitchGravity ();
		}
	}

	void SwitchGravity() {
		// for each element inside the area of action
		for (int i = 0; i < insideTrigger.Length; i++) {
			if (insideTrigger [i].attachedRigidbody) {
				//change the gravity
				insideTrigger [i].attachedRigidbody.gravityScale = insideTrigger [i].attachedRigidbody.gravityScale * -1;
			}			
		}
	}

	//method to get the trigger circle collider on the character
	public void TriggerCollider(ref CircleCollider2D triggerCollider) {
		CircleCollider2D[] circles;
		circles = gameObject.transform.FindChild("Magnet").GetComponents<CircleCollider2D> ();

		for (int i = 0; i < circles.Length; i++) {
			if (circles [i].isTrigger == true) {
				triggerCollider = circles [i];
			}
		}
	}
}
