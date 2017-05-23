using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Instantiate : MonoBehaviour {

	[SerializeField] protected GameObject activator;

	//where to instantiate the tutorial
	[SerializeField]
	private Transform location;
	protected Transform Location { 
		get { return location; }
		set { location = value; }
	}

	[SerializeField] private float offsetX = 0;
	[SerializeField] private float offsetY;

	//instantiated elements
	private GameObject[] clone;
	protected GameObject[] Clone { 
		get { return clone; }
		set { clone = value; }
	}

	[SerializeField] [TextArea(3,6)]
	private string[] texts;
	protected string[] Texts { 
		get { return texts; }
		set { texts = value; }
	}

	[SerializeField]
	private int fontSize;

	//tutorials to instantiate
	[SerializeField]
	private GameObject[] prefabs;
	protected GameObject[] Prefabs { 
		get { return prefabs; }
		set { prefabs = value; }
	}

	private int totalLength;
	private bool activated = false;

	[SerializeField]
	private bool oneTime = false;

	[SerializeField]
	private float waitTime;
	[SerializeField][Tooltip("Will delete the object on trigger exit. If delete time different than zero, will delete on that time if player is still on trigger")]
	private bool deleteAfterAppear = false;
	[SerializeField]
	private float duration;

	// Use this for initialization
	//how many elements will be instantiated and position
	protected virtual void Start () {
		totalLength = Prefabs.Length + Texts.Length;
		Clone = new GameObject[totalLength];
	}

	protected virtual void Update () {
	}

	//instantiation of elements
	protected virtual void OnTriggerEnter2D (Collider2D other) {
		if (activator == null && activated == false) {
			Invoke ("Process", waitTime);

			if (deleteAfterAppear == true && duration > 0) {
				Invoke ("DestroyAll", duration + waitTime);
			}
		} 
		else if (activator && activated == false) {
			if (other.name == activator.name) {
				Invoke ("Process", waitTime);

				if (deleteAfterAppear == true && duration > 0) {
					Invoke ("DestroyAll", duration + waitTime);
				}
			}
		}
	}

	protected virtual void OnTriggerStay2D (Collider2D other) {
	}

	//destruction of instantiated elements
	protected virtual void OnTriggerExit2D (Collider2D other) {
		if (activator == null && deleteAfterAppear == true) {
			DestroyAll ();
		} 
		else if (activator && deleteAfterAppear == true) {
			if (other.name == activator.name) {
				DestroyAll ();
			}
		}
	}

	protected virtual void DestroyAll() {
		for (int i = 0; i < Clone.Length; i++) {
			Destroy (Clone [i]);
		}
	}

	protected virtual void Process() {
		OneTime ();
		for (int i = 0; i < totalLength; i++) {
			if (i < Texts.Length) {
				Clone [i] = Instantiate (Resources.Load("Text Placeholder"), new Vector3 (Location.position.x + (2f * i) + offsetX, Location.position.y + offsetY, Location.position.z), Quaternion.identity) as GameObject;
				Clone [i].GetComponent<TextMesh> ().text = Texts [i];
				if (fontSize > 0) {
					Clone [i].GetComponent<TextMesh> ().fontSize = fontSize;
				}
				clone [i].transform.SetParent (this.gameObject.transform);
			}
			else {
				Clone [i] = Instantiate (Prefabs [i - Texts.Length], new Vector3 (Location.position.x + (2f * i) + offsetX, Location.position.y + offsetY, Location.position.z), Quaternion.identity) as GameObject;
				clone [i].transform.SetParent (this.gameObject.transform);
			}
		}
	}

	protected virtual void OneTime() {
		if (oneTime == true) {
			activated = true;
		} else {
			activated = false;
		}
	}
}