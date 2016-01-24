using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MapDataStructure : MonoBehaviour {
    ///////////////////////////////////////////
    //
    //      Custom Editor
    //
    ///////////////////////////////////////////
    public GameObject setPlayerSpots()
    {
        GameObject tempObj = new GameObject("PlayerSpots" + playerSpawnSpots.Count);
		tempObj.transform.position = 
			SceneView.lastActiveSceneView.pivot;
        tempObj.transform.parent = this.transform;
        return tempObj;
    }

    public void deletePlayerSpots()
    {
		if (playerSpawnSpots.Count <= 0)
			return;
        Transform tempObj = playerSpawnSpots[playerSpawnSpots.Count - 1];
        playerSpawnSpots.RemoveAt(playerSpawnSpots.Count-1);
        DestroyImmediate(tempObj.gameObject);
    }
	
	
	
    public GameObject setItemSpots()
    {
		GameObject tempObj = new GameObject("ItemSpots" + itemSpawnSpots.Count);
		tempObj.transform.position = 
			SceneView.lastActiveSceneView.pivot;
        tempObj.transform.parent = this.transform;
        return tempObj;
    }

    public void deleteItemSpots()
	{
		if (itemSpawnSpots.Count <= 0)
			return;
        Transform tempObj = itemSpawnSpots[itemSpawnSpots.Count - 1];
        itemSpawnSpots.RemoveAt(itemSpawnSpots.Count-1);
        DestroyImmediate(tempObj.gameObject);
    }
	
	
	
    public GameObject setMonsterSpots()
    {
		GameObject tempObj = new GameObject("MonsterSpots" + monsterSpawnSpots.Count);
		tempObj.transform.position = 
			SceneView.lastActiveSceneView.pivot;
        tempObj.transform.parent = this.transform;
        return tempObj;
    }

    public void deleteMonsterSpots()
	{
		if (monsterSpawnSpots.Count <= 0)
			return;
        Transform tempObj = monsterSpawnSpots[monsterSpawnSpots.Count - 1];
        monsterSpawnSpots.RemoveAt(monsterSpawnSpots.Count-1);
        DestroyImmediate(tempObj.gameObject);
    }

    ///////////////////////////////////////////
	
	
	public GameObject map;
	public List<Transform> playerSpawnSpots;
	public List<Transform> itemSpawnSpots;
	public List<Transform> monsterSpawnSpots;
	
	[HideInInspector] public bool[] playerOcuppiedSpots;
	[HideInInspector] public bool[] itemOcuppiedSpots;
	[HideInInspector] public bool[] monsterOcuppiedSpots;
	
	
	// Use this for initialization
	void Start () {
		playerOcuppiedSpots = new bool[playerSpawnSpots.Count];
		itemOcuppiedSpots = new bool[itemSpawnSpots.Count];
		monsterOcuppiedSpots = new bool[monsterSpawnSpots.Count];
	}

	public int randomPlayerIndex()
	{
		int index;
		do{
			index = Random.Range(0, playerSpawnSpots.Count);
		}while(playerOcuppiedSpots[index]);
		return index;
	}
	public int randomItemIndex()
	{
		int index;
		do{
			index = Random.Range(0, itemSpawnSpots.Count);
		}while(itemOcuppiedSpots[index]);
		return index;
	}
	public int randomMonsterIndex()
	{
		int index;
		do{
			index = Random.Range(0, monsterSpawnSpots.Count);
		}while(monsterOcuppiedSpots[index]);
		return index;
	}


	//set state at index position
	public Transform setPlayerPosition(int index, bool set)
	{
		playerOcuppiedSpots[index] = set;
		return playerSpawnSpots[index];
	}
	public Transform setItemPosition(int index, bool set)
	{
		itemOcuppiedSpots[index] = set;
		return itemSpawnSpots[index];
	}
	public Transform setMonsterPosition(int index, bool set)
	{
		monsterOcuppiedSpots[index] = set;
		return monsterSpawnSpots[index];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
