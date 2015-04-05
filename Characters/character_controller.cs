//CharacterControllerScript.cs
//
//Used to control flying characters
//Script is attached to each character


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityEngine.EventSystems;

public class character_controller : MonoBehaviour {
	
	//Forces used in character movement
	public float vertForce = 100;
	public float sideForce = 75;
	
	//Constant upwards force
	public float floatForce = 15;
	
	//Player Number
	public int playerNum = 0;
	
	//Variables for input
	private float verticalInput;
	private float horizontalInput;
	public bool fireUp;
	public bool fire;
	public bool pauseButton;
	public bool fireDown;
	public bool special;
	public bool startReplay;

	//pickup type bools
	public bool manual = true;
	
	//animator controller
	public Animator anim;
	
	//audio
	public AudioSource magnetAudio;
	
	public AudioClip stunAudio;

	public AudioSource chopperNoise;
	
	public InputDevice inputDevice;
	
	//pause animator
	//public Animator optionsAnim;

	//grip
	public abstract_pickup grip;
	//Special Ability
	public abstract_special_ability ability;

	void StopCollisionAnim() {
		anim.SetBool("Hit", false);
	}
	
	
	public void MoveToInitialPosition() {

	}
	
	
	// Use this for initialization
	void Start () {
		//optionsAnim = GameObject.Find("settings_menu").GetComponent<Animator>();
		
		//make chain more accurate
		foreach (Transform child in transform)
		{
			if (child.GetComponent<Rigidbody>()) {
				child.rigidbody.solverIterationCount = 15;
			}
		}

		//set special ability
    ability = gameObject.GetComponentInChildren<abstract_special_ability>();
		//set grip
		grip = gameObject.GetComponentInChildren<abstract_pickup>();
		//set animator
		anim = gameObject.GetComponentInChildren<Animator>();
	
		inputDevice = (InputManager.Devices.Count > playerNum) ? InputManager.Devices[playerNum] : null;	
	
		//get mass from singleton
		if (PlayerManager.instance.playerList.Count > playerNum)
			rigidbody.mass = PlayerManager.instance.playerList[playerNum].mass;

		//set body mesh scale and floatForce SORRY FOR THE RANDOM ASS FUNCTION, sleepy...
		transform.localScale = new Vector3( .03125f * Mathf.Pow(rigidbody.mass, 2f)+0.7188f, .03125f * Mathf.Pow(rigidbody.mass, 2f)+0.7188f, .03125f * Mathf.Pow(rigidbody.mass, 2f)+0.7188f );
		floatForce = .03125f * Mathf.Pow(floatForce, 2f)+0.7188f;

		//find last joint in chain
		//GameObject lastJoint = transform.Find("magnet").gameObject;
	}
	
	// Update is called once per frame
	void Update () {
		//INPUT----------------------------------------------------------
		if (inputDevice != null) {
			verticalInput = inputDevice.Direction.Y;
			horizontalInput = inputDevice.Direction.X;
			fireUp = inputDevice.Action1.WasReleased;
			fire = inputDevice.Action1;
			fireDown = inputDevice.Action1.WasPressed;
			special = inputDevice.RightBumper;
			startReplay = Input.GetKeyUp("p");
			if (InputManager.Devices.Count == 2 && playerNum == 0) {
				pauseButton = inputDevice.GetControl(InputControlType.Back).WasPressed;
			} else if (InputManager.Devices.Count > 2) {
				pauseButton = inputDevice.MenuWasPressed;
			}
		}		
		//Debug.Log (verticalInput);
		if(verticalInput != 0) {
			chopperNoise.pitch = 1.3f;
			chopperNoise.volume = 0.4f;
		} else if(horizontalInput != 0) {
			chopperNoise.pitch = 1.3f;
			chopperNoise.volume = 0.4f;
		} else {
			chopperNoise.pitch = 1.0f;
			chopperNoise.volume = 0.2f;
		}
		

		// freeze if gameover
		if (match_observer.MO.curGameState == GameState.GameOver) {
			rigidbody.constraints = RigidbodyConstraints.FreezeAll;
		} else {
			rigidbody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		}
		
		
		// SPECIAL ABILITY----------------------------------------------
		if (match_observer.MO.curGameState != GameState.Countdown) {
			if(special){
				ability.Activate();
			}
			else {
				ability.Idle();
			}
		}


		//ANIMATIONS-------------------------------------------
		anim.SetFloat("Horizontal", horizontalInput * Mathf.Sign( transform.localScale.x ) );
		

		if(fire){
			anim.SetBool("Struggling", true);
			grip.Pickup();
		}
		if(fireUp){
			grip.Release();
			anim.SetBool("Struggling", false);
			Debug.Log("released");
		}
		if(fireDown) {
			if (!magnetAudio.isPlaying) {
				magnetAudio.Play();
			}
		}
		//-------------------------------------------------------------------------------------------

	}
	
	// FixedUpdate is called once per physics step
	void FixedUpdate () {
		if(startReplay) GameObject.Find("Match Observer").GetComponent<replay_manager>().PlayObservers();
		//CONSTANT UPWARDS FORCE
		rigidbody.AddRelativeForce (0, floatForce * 1.0f, 0);

		//APPLY MOVEMENT W/ INPUT
		rigidbody.AddRelativeForce( horizontalInput * sideForce, 0, 0);
		rigidbody.AddRelativeForce(0, verticalInput * vertForce, 0);
 
		if ((rigidbody.velocity.x < 0f && horizontalInput > 0f) || (rigidbody.velocity.x > 0f && horizontalInput < 0f)) {
			Vector3 tempVector = new Vector3(rigidbody.velocity.x/2, rigidbody.velocity.y, rigidbody.velocity.z);
			rigidbody.velocity = tempVector;
		}

	}
}