using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Spawns random objects and supplies an interface that is easy to work with for spawners
/// </summary>
public class object_factory : MonoBehaviour {
	
	public GameObject[] commonItems;
	public GameObject[] rareItems;
	public GameObject[] veryRareItems;
	public List<GameObject> bombs = new List<GameObject>();
	
	public enum BombType {
		NormalBomb,
		LiftBomb,
		SingularityBomb,
		GravityBomb
	}
	
	float commonProb;
	float rareProb;
	/// <summary>
	/// Gets a random object from the arrays
	/// </summary>
	
	/// <returns>The random object.</returns>
	private GameObject GetRandomObject(){
		
		GameObject obj = null;
		float bombProbability = Random.Range (0f, 1f);
		//Debug.Log ("Bomb count" + bombs.Count+"bomb probability "+bombProbability);
		if (bombProbability < MatchSettings.instance.bombProbability && bombs.Count > 0 
		    && match_observer.MO.curGameState != GameState.SuddenDeath){
			int bombIndex = Random.Range(0,bombs.Count);
			//Debug.Log("spawns bomb from index "+bombIndex);
			return bombs[bombIndex] as GameObject;
		}
		else{
			float prob = Random.Range(0f,1f);
			if(prob>=commonProb){
				obj =GetRandomFromArray(commonItems);
			}
			else if (prob>=rareProb) {
				obj = GetRandomFromArray(rareItems);
			}
			else if (prob<rareProb) {
				obj = GetRandomFromArray(veryRareItems);
			}
			else{
				Debug.Log("Problem in factory probability");
			}
		}
		
		return obj;
		
	}
	
	private GameObject GetSpecificObject(int itemClass,int id){
		return null;
	}
	/// <summary>
	/// Decides on a random object and spawns it	/// </summary>
	/// <returns>The spawned object.</returns>
	/// <param name="position">Position to spawn at</param>
	/// <param name="rotation">Rotation to spawn at</param>
	
	public GameObject SpawnRandomObject(Vector3 position, Quaternion rotation){
		GameObject prefab = GetRandomObject();
		GameObject obj = Instantiate(prefab,position,rotation) as GameObject;
		/*float bombProbability = Random.Range (0f, 1f);
		if (!GameMaster.GM.onSuddenDeath && bombProbability < GameMaster.GM.bombProbability){
			AttachRandomBomb(obj);
		}*/
		return obj;
	}
	
	private string prefabNameToFind;
	private bool BombNameObject(GameObject obj) {
		return (obj.name.Contains (prefabNameToFind));
	}
	int FindBombTypeIndex (BombType bombType) {
		
		switch (bombType) {
		case BombType.NormalBomb:
			prefabNameToFind = "Standard_Bomb";
			break;
		case BombType.LiftBomb:
			prefabNameToFind = "Lift_Bomb";
			break;
		case BombType.GravityBomb:
			prefabNameToFind = "Gravity_Bomb";
			break;
		case BombType.SingularityBomb:
			prefabNameToFind = "Singularity_Bomb";
			break;
		default:
			return -1;
		}
		
		return bombs.FindIndex (BombNameObject);
		
	}
	
	public GameObject SpawnBomb(BombType bombType,Vector3 position, Quaternion rotation){
		//TODO: check the index according with the type
		int bombIndex = FindBombTypeIndex (bombType);
		if (bombIndex != -1) {
			GameObject prefab = bombs[bombIndex] as GameObject;
			GameObject obj = Instantiate (prefab, position, rotation) as GameObject;
			return obj;
		}
		//print ("couldnt spawn " + bombType.ToString() + " because it is not available on the bomb list");
		return null;
		
	}
	
	// Use this for initialization
	void Start () {
		commonProb = 0.5f;
		rareProb = 0.2f;
		if (!match_observer.MO.standaloneScene) {
			bombs = MatchSettings.instance.bombList;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	//Divides 
	private GameObject GetRandomFromArray(GameObject[] array){
		int size = array.Length;
		float indexRange = 1.0f/size;
		float rand = Random.Range(0f,1f);
		int index = Mathf.FloorToInt(rand/indexRange);
		return array[index];
	}
	
	
	
	/*private GameObject AttachRandomBomb(GameObject newObj){
		int bombType = Random.Range(1,5); // c# int random is not maximally inclusive
		if(bombType==1){
			newObj.AddComponent<SingBombScript>();
		}
		else if(bombType==2){
			newObj.AddComponent<GlueBombScript>();
		}
		else if(bombType==3){
			newObj.AddComponent<LiftBombScript>();
		}
		else{
			newObj.AddComponent<ExplosiveBombScript>();
		}
		return newObj;
	}*/
}