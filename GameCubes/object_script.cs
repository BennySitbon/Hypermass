//object_script.cs
//
//Holds information regarding grab-able objects, applies weight to scales
//Script is attached to every object that can be picked up


using UnityEngine;
using System.Collections;

public class object_script : MonoBehaviour {

	//timer variables
	public float idleTime = 60f;
	public float timer = 60f;

	// idle flag
	private bool idle = false;

	//effect used for idle disappearing
	public GameObject idleEffect;
	public Transform onScaleEffect = null;

	//temporary vars for now - should we remove them?
	public bool isHeld = false;
	public bool isSucked = false;
	public bool onScale = false;

	public bool isBomb = false;
	// Use this for initialization
	void Start () {

		timer = idleTime + 1;
		if (GetComponent<bomb>()) 
			isBomb = true;
	}
	
	// Update is called once per frame
	void Update () {

		/*//check for bad joint
		if (gameObject.GetComponent<FixedJoint>()) {
			if (gameObject.GetComponent<FixedJoint>().connectedBody == null) {
				Destroy(gameObject.GetComponent<FixedJoint>());
			}
		}*/

		// check if object is idle
		if (isBomb || match_observer.MO.curGameState == GameState.SuddenDeath)
			idle = false;
		else
			idle = (onScale) ? false : (rigidbody.velocity.magnitude <= 0.05f); 

		//increment existTimer
		if (idle && match_observer.MO.curGameState != GameState.SuddenDeath) {
			timer -= Time.deltaTime;
		}
		else {
			timer = idleTime;
		}

		if ((int)timer <= 0 && gameObject.tag != "OutgoingObj") {
			DestroyWithEffect();
		}
	}

	void OnCollisionEnter(Collision col) {
		if(onScaleEffect != null) {
			if(col.transform.parent!=null){
				if (col.transform.parent.gameObject.tag == "ScaleRed" || col.transform.parent.gameObject.tag == "ScaleBlue") {
					onScale = true;
					Debug.Log("made effect");
					Transform newScaleEffect = Instantiate(onScaleEffect, transform.position, Quaternion.identity) as Transform;
				}
			}
		}
	}

	void OnCollisionExit(Collision col) {
		if (col.gameObject.tag == "ScaleRed" || col.gameObject.tag == "ScaleBlue") 
			onScale = false;
	}

	void KillObj() {
		Destroy(gameObject);
	}

	void DestroyWithEffect() {
		GameObject objEffect = Instantiate(idleEffect, transform.position, transform.rotation) as GameObject;
		objEffect.transform.parent = gameObject.transform;
		objEffect.transform.localScale = new Vector3(1,1,1);
		Invoke("KillObj", 1f);
		gameObject.tag = "OutgoingObj";
	}
} //end of class