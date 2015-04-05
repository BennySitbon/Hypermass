using UnityEngine;
using System.Collections;
using InControl;

public class character_turbo : abstract_special_ability {

	public float turbo = 5;
	public float turboLeft = 70;
	//recharge rate
	public float turboRechargeRate = 3f;
	public float maxTurbo = 70;


	// Update is called once per frame
	public override void Update () {
		base.Update();
	}

	public override void Activate ()
	{
		//Apply turbo force
		if(turboLeft>0){
			//Apply force
			gameObject.rigidbody.AddRelativeForce( gameObject.rigidbody.velocity.normalized * turbo );
			//Burn turbo
			turboLeft = Mathf.Clamp(turboLeft - actionButton, 0, maxTurbo);
			//Activate trail
			gameObject.GetComponent<TrailRenderer>().time = 1;
		}
		else{
			Idle ();
		}
	}

	public override void Idle () {

		//REPLENISH TURBO
		if (actionButton == 0) {
			turboLeft = Mathf.Clamp(turboLeft + Time.deltaTime * turboRechargeRate, 0, maxTurbo);
		}

		//Deactivate trail
			gameObject.GetComponent<TrailRenderer>().time = Mathf.Lerp(gameObject.GetComponent<TrailRenderer>().time, 0.0f, 0.1f);
		if (gameObject.GetComponent<TrailRenderer>().time < .05f) {
			gameObject.GetComponent<TrailRenderer>().time = 0;
		}

	}

}
