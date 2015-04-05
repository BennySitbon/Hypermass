using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// Handles the replay abilities for the game
/// Uses an observer pattern to notify all the objects when to record and when to playback
/// also holds the which frame number to play and record to synchronise all the records in the game
/// Place this on the match_observer or on a different object for replay_manager if needed
/// </summary>
public class replay_manager : MonoBehaviour {

	//Enum to control the recorder
	public enum ReplayState{Paused,Recording,Playing};
	public int filmSize = 300;
	public int curRecFrame = 0;
	public int curPlayFrame = 0;
	public float speedFactor = 1.0f;
	public int num_replays = 2; //TODO: figure out why this plays 4 times instead of 2 times
	private List<Recorder> observers = new List<Recorder>();
	public ReplayState replayState;
	public GameObject replayObj = null;
	// Use this for initialization
	void Start () {
		RecordObservers();
	}
	
	// Update is called once per frame
	void Update () {
	}
	void FixedUpdate(){
		if(replayState==ReplayState.Recording) AdvanceRecPointer();
		if (replayObj != null) {
			if(replayState==ReplayState.Playing)
				replayObj.SetActive(true);
			else
				replayObj.SetActive(false);
		}
	}
	//When we register a new recorder the current state of replay(recording/playing/paused), and the maximum film size
	public void RegisterObserver(Recorder observer){
		observers.Add(observer);
		observer.replayState = this.replayState;
		observer.film = new Frame[filmSize];
	}
	public void UnregisterObserver(Recorder observer){
		observers.Remove(observer);
	}
	private void AdvancePlayPointer()
	{
		curPlayFrame = (curPlayFrame<filmSize-1)? curPlayFrame+1: 0;
	}
	//Moves the recording pointer forward to keep recording
	private void AdvanceRecPointer()
	{
		curRecFrame = (curRecFrame<filmSize-1)? curRecFrame+1: 0;
	}
	public int GetRecFrame(){
		return curRecFrame;
	}
	public int GetPlayFrame(){
		return curPlayFrame;
	}
	public void PlayObservers(){
		Time.timeScale = 0.0f; //Freeze the game during replay
		print("Starts playing");
		//Set the play pointer to start right after the end of the recording (start of the circular array)
		curPlayFrame = (curRecFrame<filmSize-1)?curRecFrame+1:0;
		replayState = ReplayState.Playing;
		StartCoroutine(PushPlay());
	}

	public IEnumerator PushPlay()
	{
		for (int i = 0; i < (filmSize*num_replays)-5; i++) {
			AdvancePlayPointer();
			foreach (var item in observers)
			{
				item.PlayFrame();
			}
			yield return StartCoroutine(CoroutineUtil.WaitForRealSeconds(0.02f*speedFactor));
		}
		Time.timeScale = 1.0f;
		RecordObservers();
	}
	public void StopPlayObservers(){
		foreach (var item in observers) {
			item.StopPlaying();	
		}
	}
	public void RecordObservers(){
		print("Starts recording");
		replayState = ReplayState.Recording;
		foreach (var item in observers) {
			item.StartRecording();	
		}
	}
	public void StopRecordObservers(){
		foreach (var item in observers) {
			item.StopRecording();	
		}
	}
	public static class CoroutineUtil
	{
		public static IEnumerator WaitForRealSeconds(float time)
		{
			float start = Time.realtimeSinceStartup;
			while (Time.realtimeSinceStartup < start + time)
			{
				yield return null;
			}
		}
	}

}
