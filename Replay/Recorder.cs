using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This script will record any object for the replay_manager
/// The difference between the objects will be the type of frames they use
/// since each frame plays itself and knows what details of the GameObject to save and play, all a 
/// frame needs is basically to be passed the GameObject which the Recorder is on
/// </summary>
public class Recorder: MonoBehaviour{

	public replay_manager.ReplayState replayState;

	public Frame[] film;

	public bool activeObj = true; //is the object active in the current playing frame

	public Frame frameStyle; //a reference to the type of frames the recorder should use
	
	private replay_manager replay_m;

	private GameObject replay_clone;

	public void Start()
	{
		frameStyle = FrameChooser(); //Choose what type of frame this Recorder will use (different frames for different GameObjects)
		Register();
	}
	public void FixedUpdate()
	{
		if(replayState == replay_manager.ReplayState.Recording){
			RecordFrame();
		}
		else if(replayState==replay_manager.ReplayState.Playing){ //Speedfactor is used to slow down replays
			PlayFrame();
		}
	}
	//Register to the replay manager - do it on creation - observer pattern
	public void Register()
	{
		replay_m = GameObject.Find("Match Observer").GetComponent<replay_manager>();
		replay_m.RegisterObserver(this);
	}
	//Unregister from the replay manager- do it on real destruction (not when invisible)
	public void Unregister()
	{
		replay_m.UnregisterObserver(this);
	}
	public void OnDestroy(){
		Unregister();
	}
	//Records one frame
	private void RecordFrame()
	{
		int curRecFrame = replay_m.GetRecFrame(); //gets the frame number to currently record
		if (!activeObj)
		{
			film[curRecFrame] = null;
			return;
		}
		if(film[curRecFrame]==null) //if this frame wasnt recorded before make and record a new one
		{
			film[curRecFrame] = frameStyle.CreateInstance(gameObject);
		}
		else //If we alredy recorded in this frame, just update it, saves garbage collection
		{
			film[curRecFrame].UpdateFrame();
		}
	}
	//Play one frame - uses the position pointer
	public void PlayFrame()
	{
		int curPlayFrame = replay_m.GetPlayFrame(); //gets the frame number to play
		if(film[curPlayFrame]!=null) 
		{
			if(activeObj==false)
			{
				activeObj = true;
				frameStyle.EnableObj();
			}
			//Have the play take in the clone to affect it
			film[curPlayFrame].Play();
		}
		else if(activeObj==true)
		{
			activeObj = false;
			frameStyle.DisableObj();
		}
	}

	//Starts the playback
	public void StartPlaying()
	{
		replayState = replay_manager.ReplayState.Playing;
	}
	//Stops the playback
	public void StopPlaying()
	{
		replayState = replay_manager.ReplayState.Paused;
	}
	//Starts Recording
	public void StartRecording()
	{
		replayState = replay_manager.ReplayState.Recording;
	}
	//Stops recording
	public void StopRecording()
	{
		replayState = replay_manager.ReplayState.Paused;
	}
	//Chooses the type of frame to use for this recorder
	//TODO: its not the most elegant but will do for first draft
	public Frame FrameChooser()
	{
		if(gameObject.GetComponent<bomb>()!=null) //If is bomb
		{
			return new bomb_frame(gameObject);
		}
		if(gameObject.GetComponent<object_script>()!=null) //if is object
		{
			return new object_frame(gameObject);
		}
		if(gameObject.name.Contains("pulse")){
			return new single_animation_frame(gameObject);
		}
		if(gameObject.name.Contains("effect")||gameObject.name.Contains("Effect")|| gameObject.name.Contains("Explosion")|| gameObject.name.Contains("particle")){ //If is a particle effect
			return new particle_frame(gameObject);
		}

		if (gameObject.GetComponent<Text>()!=null) {// if is a canvas
			return new text_frame(gameObject);
		}
		if (gameObject.GetComponent<character_controller> () != null) { //if a copterpod
			return new copterpod_frame(gameObject);
		}
		if (gameObject.name.Contains ("Scale")) { //if a scale
			return new scale_frame(gameObject);
		}
		print("Couldn't find a matching frame");
		return null;
	}

}
