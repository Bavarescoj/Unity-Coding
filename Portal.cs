using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Portal : MonoBehaviour {

	// A dictionary is required for inbound objects as multiple objects may arrive in bound at the same time
	//static so that it's the same dictionary for all portals that have this script
	private static Dictionary<int, bool> inbound; // Inbound objects to this portal (stores the instanceID as the unique key)

	[SerializeField] private float force = 150;

	private enum direction : int {Left, Right, Up, Down};

	[Tooltip("Direction on which this portal is pointing")] [SerializeField]
	private direction portalDirection;

	[Tooltip("The destination portal that object will be teleported to")] [SerializeField]
	private Portal destination; // the actual paired portal to jump to

	//private bool onDestination = false;

	// Use this for initialization
	void Start () {
		inbound = new Dictionary<int, bool> ();
	}

	// Update is called once per frame
	void Update () {

	}

	// Teleport objects and creatures to paired portal
	void OnTriggerEnter2D(Collider2D other) {

		//avoid collision with character controller, and with object that have either on the "NoPortal" tag, or that are special gravity items that have the "Kinematic" tag
		if (other.tag != "PlayerChild" && (other.tag != "NoPortal" || other.tag != "Kinematic")) {

			if (other.gameObject.layer == LayerMask.NameToLayer ("Gravity") || other.gameObject.layer == LayerMask.NameToLayer ("Player")) {
				
				//check to see if the object we collided with is not what just jumped
				if (!inbound.ContainsKey (other.gameObject.GetInstanceID ())) {

					// check that the portal has a paired portal to move to
					if (destination) {
						//if doesnt contains it, object is on main portal, therefore is not on destination, save object on the list
						inbound.Add (other.gameObject.GetInstanceID (), false);
						//change its position to the one from the destination
						other.transform.position = new Vector3(destination.transform.position.x, destination.transform.position.y, 0);
					}
				} 
				//if element is on the dictionary, that means the OnTriggerEnter activated here is the one from destination, therefore element has just jumped
				else if (inbound.ContainsKey (other.gameObject.GetInstanceID ())) {
					inbound[other.gameObject.GetInstanceID()] = true;
					//apply a force according to the direction established as the out direction for the destination
					TransmissionForce (other.attachedRigidbody);
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D other) {

		//avoid collision with character controller
		if (other.tag != "PlayerChild" && (other.tag != "NoPortal" || other.tag != "Kinematic")) {

			if (other.gameObject.layer == LayerMask.NameToLayer ("Gravity") || other.gameObject.layer == LayerMask.NameToLayer ("Player")) {

				//if element exists on dictionary
				if (inbound.ContainsKey (other.gameObject.GetInstanceID ())) {
					//if element is false, that means the OnTriggerExit activated is the one from the main portal
					if (inbound [other.gameObject.GetInstanceID ()] == false) {
						;
					}
					// is it's true, that means the OnTriggerExit activated is the one from the destination, and object is just exiting it 
					else if (inbound [other.gameObject.GetInstanceID ()] == true) {
						// search for it and remove
						inbound.Remove (other.gameObject.GetInstanceID ());
					}
				}
			}
		}
	}

	//apply forces according on direction of destination
	void TransmissionForce(Rigidbody2D body) {

		body.velocity = new Vector2 (0, 0);

		if (portalDirection == direction.Up) {
			body.AddForce (force * body.mass * Vector2.up);
		} 
		else if (portalDirection == direction.Down) {
			//nothing, gravity will do it
			;
			//body.AddForce (force * body.mass * Vector2.down);
		} 
		else if (portalDirection == direction.Left) {
			body.AddForce (force * body.mass * Vector2.left);
		} 
		else if (portalDirection == direction.Right) {
			body.AddForce (force * body.mass * Vector2.right);
		} 
		else {
			;
		}
	}
}
