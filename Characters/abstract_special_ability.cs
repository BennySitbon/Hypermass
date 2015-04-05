using UnityEngine;
using System.Collections;
using InControl;

public abstract class abstract_special_ability : MonoBehaviour {

	public InputDevice inputDevice;
	public int playerNum;
	public float actionButton = 0;

	// Use this for initialization
	public virtual void Start () {
		playerNum = gameObject.GetComponent<character_controller>().playerNum;
		inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;

	}
	
	// Update is called once per frame
	public virtual void Update () {
		if(inputDevice!=null){
			actionButton = inputDevice.RightBumper;
		}
	}

	public virtual void FixedUpdate(){

		if(actionButton>0){
			Activate();
		}
		else {
			Idle();
		}
	}
	//What to do when the ability is activated
	public abstract void Activate();
	//What to do when the ability is not active (recharging etc.)
	public abstract void Idle();
}
