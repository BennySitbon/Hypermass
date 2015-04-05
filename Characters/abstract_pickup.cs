using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// An abstract class to be used as a base/interface for all different picking up technicques to be used by different characters
/// </summary>

abstract public class abstract_pickup:MonoBehaviour {

	//List of all objects currently held
	public List<GameObject> heldObjs;
	//Indicates that the character can hold items currently
	public bool canHold = true;
	
	//object hold limit
	public float totalObj = 0;
	public float holdingLimit = 2;

	//Reference to character animator
	public Animator anim;

	//Renables chars to pick up objects
	void UndoStun () {
		canHold = true;
	}
	//virtual so it can be overriden
	public virtual void Start(){
		//init the held objects list
		heldObjs = new List<GameObject>();
	}

	//virtual so it can be overriden
	public virtual void Update(){

	}

	// disable the ability to hold items when stunned
	public void Stun () {
		canHold = false;
		Release();
		anim.SetBool("Hit", true);
		anim.SetBool("Struggling", false);
		Invoke ("UndoStun", 3);
	}
	//Handles the picking up of different objects
	public abstract void Pickup();
	//Handles what happens when the player release the pickup button
	public abstract void Release();
}
