/// <summary>
/// A concetrete implementation of abstract_pickup that will be used by the copterpod character
/// script is to be attached to the magnet
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public class copterpod_pickup : abstract_pickup {

	//layermask for magnet overlap sphere
	public int layerMask = -1;

	//radius and angle for picking up objects
	public float magnetRadius = 1;
	public float magnetAngle = 80.0f;

	public float magnetForce = 75f;
	
	public character_controller characterController;

	private bool _holding;
	private List<FixedJoint> _joints;
	private bool _sucking = false;

	// Use this for initialization
	public override void Start () {
		base.Start();

		//layerMask for grabObjs
		layerMask = 1 << LayerMask.NameToLayer("GrabObjs");

		//Get input device for player
		//Should this be in update? instead of start? 
    this._joints = new List<FixedJoint>();
   // this.inputDevice = (InputManager.Devices.Count > characterController.playerNum) ? InputManager.Devices[ characterController.playerNum ] : null;
	}

	//PICKING UP OBJ
	void OnCollisionEnter( Collision col ) {
		// if our input is "on"
		if ( this._holding ) {
			// if the other object has a rigidbody
			if ( col.rigidbody != null && col.gameObject.tag == "grabObj") {
				// check the angle between the objects, and ensure they're small enough.
				if ( Vector3.Angle( (col.transform.position - this.transform.position), -this.transform.up ) < this.magnetAngle ) {
					// make a joint, and keep track of it.
					FixedJoint joint = col.gameObject.AddComponent<FixedJoint>();
					joint.connectedBody = this.rigidbody;
					this._joints.Add( joint );
					if (col.gameObject.GetComponent<bomb>())
						col.gameObject.BroadcastMessage("PlayerGrabbed");
				}
			}
		}
	}

	void FixedUpdate() {
		if (_sucking) {	
			//sucking up objects-----------------------------------------------------------
			if (totalObj < holdingLimit) {
				//Create an array that holds every grabObj layer object found in the sphere
				Collider[] hitColliders = Physics.OverlapSphere(transform.position - new Vector3(0, 1, 0) , magnetRadius, layerMask);
				
				for (int i = 0; i < hitColliders.Length; i++) {
					
					if (hitColliders[i].gameObject.GetComponent<object_script>()){

						object_script objScript = hitColliders[i].gameObject.GetComponent<object_script>();

						//check if the object is within the angle bounds
						if(Vector3.Angle(-transform.up, (hitColliders[i].transform.position - transform.position - new Vector3(0, 1, 0))) < magnetAngle) {
							
							//while action is pressed and weight limit isn't reached we suck
							if (!hitColliders[i].GetComponent<FixedJoint>() && !objScript.isHeld) {  
								
								//if the object isn't too heavy
								if ((totalObj + hitColliders[i].rigidbody.mass)  <= holdingLimit) {
									
									//being sucked
	                objScript.isSucked = true;
									
									//get direction from object to magnet
									Vector3 dir = (transform.Find("magnetTarget").position - hitColliders[i].gameObject.transform.position).normalized;
									
									//force to contact point
									hitColliders[i].gameObject.rigidbody.AddForce(dir * hitColliders[i].gameObject.rigidbody.mass * magnetForce);
								}
								else {
	                objScript.isSucked = false;
								}
							}
							else {
	              objScript.isSucked = false;
							}
						}
					}
				}
			}
		}
	}

	#region implemented abstract members of AbstractGrip

	public override void Pickup () {
		this._holding = true;
		this._sucking = true;
	}

	
	public override void Release () {
		this._holding = false;

		// just delete all of the joints :)
		foreach ( FixedJoint joint in this._joints ) {
			if (joint == null)
				break;
			else {
				if (joint.GetComponent<bomb>())
					joint.gameObject.BroadcastMessage("PlayerReleased");
				Destroy( joint );
			}
		}
		this._joints.Clear();
		this._sucking = false;


		/*
		canStick = false;
		foreach (GameObject obj in heldObjs) {
				
				//if bomb, let it know it's been thrown
				if (obj.GetComponent<bomb>()) {
					obj.GetComponent<bomb>().PlayerReleased();
					//obj.GetComponent<AbstractBomb>().held = false;
					//obj.GetComponent<AbstractBomb>().holdingPlayer = null;
				}
			}
		}
		*/
	}
	
	#endregion
}//end of class