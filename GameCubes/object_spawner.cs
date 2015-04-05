using UnityEngine;
using System.Collections;
/// <summary>
/// Spawns objects into the arena at the position of the spawner
/// </summary>
public class object_spawner : MonoBehaviour {
	
	private Transform objectsParent; //The GameObject to add the object to after creation
	private object_factory factory; //The factory to use for spawning the objects
	private float Timer;			//Keeps track of spawn time intervals
	private float spawnTime;			//The time between spawns
	private float overlap;			//The sphere that checks the area is clear to spawn in
	private Vector3 originalPos;	//The center point the spawner jumps around
	private float spawnRadius = 8;	//Radius from originalPos to spawn in
	private AudioSource audioSrc;	//AudioSource to play item phase in sfx
	public int objToSpawn;			//The number of objects to spawn - can be set from outside
	public AudioClip[] clips;		//Item phase in sfx audio clips

	public GameObject tweetCanvas;	//Canvas that holds the name of the Twitter on screen
	
	// Use this for initialization
	void Start () {
		spawnTime = MatchSettings.instance.spawnInterval;
		overlap = 3.0f;
		objToSpawn = 4;
		originalPos = gameObject.transform.position;
		objectsParent= GameObject.Find("SpawnedObjs").transform;
		factory = gameObject.GetComponent<object_factory>();
		audioSrc = gameObject.GetComponent<AudioSource>();
		Timer = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		spawnTime = MatchSettings.instance.spawnInterval;
		
		if(match_observer.MO.curGameState == GameState.SuddenDeath || match_observer.MO.curGameState == GameState.InGame){
			MoveAround();
			//TODO: change the OverlapSphere to 0 after ObjectChecker is kicked out
			if(Timer< Time.time && (Physics.OverlapSphere(transform.position,overlap).Length<=1)){
				if(objToSpawn>0){
					if(!audioSrc.isPlaying) {
						if(LoadClip ())
							audioSrc.Play ();
					}
					GameObject newObj = factory.SpawnRandomObject(transform.position,transform.rotation);
					newObj.transform.parent = objectsParent;
					objToSpawn--;
				}
				if(objToSpawn==0 && match_observer.MO.curGameState != GameState.SuddenDeath){
					Timer = Time.time+spawnTime;
					objToSpawn = Random.Range(1,5);
				}
			}
		}
	}
	
	public void SpawnBomb(object_factory.BombType bombType)
	{
		if(match_observer.MO.curGameState != GameState.Paused && match_observer.MO.curGameState != GameState.Countdown){
			MoveAround();
			//TODO: change the OverlapSphere to 0 after ObjectChecker is kicked out
			if(Physics.OverlapSphere(transform.position,overlap).Length<=1){
				GameObject newObj = factory.SpawnBomb(bombType,transform.position,transform.rotation);
				if (newObj != null) {
					newObj.transform.parent = objectsParent;
				}
			}
		}
	}

	public bool SpawnBombTwitter(object_factory.BombType bombType, TwitterData TweetInfo)
	{
		//print ("trying to spawn a bomb from tweet");
		if(match_observer.MO.curGameState != GameState.Paused && match_observer.MO.curGameState != GameState.Countdown){
			MoveAround();
			//TODO: change the OverlapSphere to 0 after ObjectChecker is kicked out
			if(Physics.OverlapSphere(transform.position,overlap).Length<=1){
				GameObject newObj = factory.SpawnBomb(bombType,transform.position,transform.rotation);
				if (newObj != null) {
					newObj.transform.parent = objectsParent;
					//print ("Tweet bomb created with name: " + TweetInfo.screenName);
					GameObject newTweet = Instantiate(tweetCanvas, newObj.transform.position, newObj.transform.rotation) as GameObject;
					print (newTweet.name);
					newTweet.transform.SetParent(newObj.transform);
					newTweet.GetComponent<TwitterCanvas>().name = "@"+TweetInfo.screenName;
					twitter_controller.Instance.spawnedId.Add(TweetInfo.id);
					return true;
				}
			}
		}

		return false;
	}

	private void OnDrawGizmos(){
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position,overlap);
	}
	
	void MoveAround(){
		//Moves the spawner in a circle
		Vector2 newPosition = Random.insideUnitCircle *spawnRadius;
		gameObject.transform.position = new Vector3(originalPos.x+newPosition.x,originalPos.y+newPosition.y,originalPos.z);
	}
	
	bool LoadClip() {
		if(clips.Length == 2) {
			int r = Random.Range (0,2);
			audioSrc.clip = clips[r];
			return true;
		}
		return false;
	}
}