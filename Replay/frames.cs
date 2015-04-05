 using UnityEngine;
using UnityEngine.UI;
using System.Collections;
/// <summary>
/// This file file detail all the different types of frames we will have and use in the Recorder script
/// All the frames must inherit from the abstract Frame class
/// </summary>
using System.Collections.Generic;


public abstract class Frame
{	

	//using this CreateInstance will give us a uniform constructor that the recorder will call 
	//but everytime the recorder calls it , it will be able to create more of itself
	//Its basically like using a "blueprint" to produce more frames of the same type instead of the recroder calling FrameChooser() every frame
	public abstract Frame CreateInstance(GameObject obj);
	//Plays this frame
	public abstract void Play();
	//Updates values with current object state
	public abstract void UpdateFrame();
	//Disables the object when if doesn't exist in the current frame in the replay
	public abstract void DisableObj();
	//Used to reactivate the object after disabling it
	public abstract void EnableObj();
}

public class object_frame:Frame
{
	GameObject go;
	Vector3 position;
	Quaternion rotation;
	Color color;
	//Default constructor the FrameChooser
	public object_frame(){}
	//Constructor for the CreateInstance method
	public object_frame(GameObject go)
	{
		this.go = go;
		this.position = go.transform.position;
		this.rotation = go.transform.rotation;
		color = go.renderer.material.color;
	}
	#region implemented abstract members of Frame
	public override Frame CreateInstance(GameObject obj)
	{
		return new object_frame(obj);
	}
	public override void Play ()
	{
		go.renderer.material.color = color;
		go.transform.position = this.position;
		go.transform.rotation = this.rotation;
	}
	public override void UpdateFrame ()
	{
		color = go.renderer.material.color;
		this.position = go.transform.position;
		this.rotation = go.transform.rotation;
	}

	public override void DisableObj ()
	{
		//Debug.Log ("Disabling obj " + go);
		go.renderer.enabled = false;
		go.collider.enabled = false;
		Behaviour[] comps = go.GetComponents<Behaviour>();
		foreach (var item in comps) {
			if(!(item is Recorder))
			{
				item.enabled = false;	
			}
		}
	}
	public override void EnableObj ()
	{
		//Debug.Log ("Reactivating obj " + go);
		go.collider.enabled = true;
		go.renderer.enabled = true;
		Behaviour[] comps = go.GetComponents<Behaviour>();		
		foreach (var item in comps) {
			if(!(item is Recorder))
			{
				item.enabled = true;	
			}
		}
	}
	
	#endregion
}
public class text_frame:Frame{
	#region implemented abstract members of Frame
	string text;
	Text t;
	public text_frame(){
	}
	public text_frame(GameObject go){
		this.t = go.GetComponent<Text>();
		this.text = t.text;
	}
	public override Frame CreateInstance (GameObject obj)
	{
		return new text_frame (obj);
	}

	public override void Play ()
	{
		t.text = text;
	}

	public override void UpdateFrame ()
	{
		this.text = t.text;
	}

	public override void DisableObj ()
	{
	}
	
	public override void EnableObj ()
	{
	}

	#endregion
}
public class copterpod_frame:Frame{
	#region implemented abstract members of Frame
	GameObject copt;
	Transform face;
	List<State> children = new List<State> ();
	Material faceMat;
	TrailRenderer[] trails;
	public copterpod_frame(){
	}
	public copterpod_frame(GameObject copterpod){
		copt = copterpod;
		Transform[] ch = copterpod.GetComponentsInChildren<Transform> ();
		foreach (var item in ch) {
			children.Add(new State(item,item.position,item.rotation));
		}
		trails = copterpod.GetComponentsInChildren<TrailRenderer>();
		face = copterpod.transform.Find ("body_parent/body/body_main");
		faceMat = face.renderer.material;
	}
	public override Frame CreateInstance (GameObject obj)
	{
		return new copterpod_frame (obj);
	}

	public override void Play ()
	{
		face.renderer.material= faceMat;
		foreach (var item in trails) {
			item.enabled = false;	
		}
		foreach (var item in children) {
			item.trans.position = item.position;
			item.trans.rotation = item.rotation;
		}
	}

	public override void UpdateFrame ()
	{
		faceMat = face.renderer.material;
		foreach (var item in children) {
			item.position = item.trans.position;
			item.rotation = item.trans.rotation;
		}

	}

	public override void DisableObj ()
	{
	}
	
	public override void EnableObj ()
	{
	}

	#endregion
	private class State{
		public Transform trans;
		public Vector3 position;
		public Quaternion rotation;

		public State(Transform t, Vector3 v, Quaternion q)
		{
			this.trans = t;
			this.position = v;
			this.rotation = q;
		}
	}
}
public class scale_frame:Frame{
	Transform trans;
	Vector3 position;

	public scale_frame(){}

	public scale_frame(GameObject go)
	{
		trans = go.transform.Find("Scale/model_scale_novabase");
		position = trans.position;
	}

	#region implemented abstract members of Frame

	public override Frame CreateInstance (GameObject obj)
	{
		return new scale_frame (obj);
	}

	public override void Play ()
	{
		trans.position = this.position;
	}

	public override void UpdateFrame ()
	{
		this.position = trans.position;
	}
	
	public override void DisableObj ()
	{
	}
	
	public override void EnableObj ()
	{
	}
	#endregion
}
public class bomb_frame:Frame{

	GameObject go;
	Vector3 bomb_position;
	Quaternion bomb_rotation;
	Transform particle_effect;
	Quaternion particle_rotation;
	Color sphere_color,cube_color;
	Renderer sphere_rend,cube_rend;

	public bomb_frame(){}

	public bomb_frame(GameObject obj){
		this.go = obj;
		particle_effect = obj.transform.Find ("inner particle");
		particle_rotation = particle_effect.rotation;
		bomb_position = obj.transform.position;
		bomb_rotation = obj.transform.rotation;
		cube_rend = go.transform.FindChild("outer cube").renderer;
		sphere_rend = go.transform.FindChild("particle collider").FindChild("inner sphere").renderer;
		sphere_color = sphere_rend.material.color;
		cube_color = cube_rend.material.color;
	}
	#region implemented abstract members of Frame

	public override Frame CreateInstance (GameObject obj)
	{
		return new bomb_frame (obj);
	}

	public override void Play ()
	{
		sphere_rend.material.color = sphere_color;
		cube_rend.material.color = cube_color;
		go.transform.position = bomb_position;
		go.transform.rotation = bomb_rotation;
		particle_effect.rotation = particle_rotation;
	}

	public override void UpdateFrame ()
	{
		sphere_color = sphere_rend.material.color;
		cube_color = cube_rend.material.color;
		bomb_position = go.transform.position;
		bomb_rotation = go.transform.rotation;
		particle_rotation = particle_effect.rotation;
	}
	
	public override void DisableObj ()
	{
		Renderer[] rends = go.GetComponentsInChildren<Renderer>();
		foreach (var item in rends) {
			item.enabled = false;		
		}
	}
	
	public override void EnableObj ()
	{
		Renderer[] rends = go.GetComponentsInChildren<Renderer>();
		foreach (var item in rends) {
			item.enabled = true;		
		}
	}
	#endregion
}
public class particle_frame:Frame{

	ParticleSystem particles;
	ParticleSystem.Particle[] parts = new ParticleSystem.Particle[500];
	public particle_frame(){}

	public particle_frame(GameObject go){
		particles = go.GetComponentInChildren<ParticleSystem>();

		particles.GetParticles(parts);

	}
	#region implemented abstract members of Frame

	public override Frame CreateInstance (GameObject obj)
	{
		return new particle_frame(obj);
	}

	public override void Play ()
	{
		if(particles) particles.SetParticles(parts,parts.Length);
	}

	public override void UpdateFrame ()
	{
		particles.GetParticles(parts);
	}
	
	public override void DisableObj ()
	{
		if(particles) particles.Clear();
	}
	
	public override void EnableObj ()
	{
	}
	#endregion
}
public class single_animation_frame:Frame{
	#region implemented abstract members of Frame
	Animator anim;
	AnimatorStateInfo pulse;
	float time;
	public single_animation_frame(GameObject obj){
		anim = obj.GetComponent<Animator>();
		pulse = anim.GetCurrentAnimatorStateInfo(0);
		time = pulse.normalizedTime;
	}
	public override Frame CreateInstance (GameObject obj)
	{
		return new single_animation_frame(obj);
	}

	public override void Play ()
	{
		anim.Play(pulse.nameHash,0,time);
	}

	public override void UpdateFrame ()
	{
		time = pulse.normalizedTime;
	}

	public override void DisableObj ()
	{

	}

	public override void EnableObj ()
	{

	}

	#endregion


}

