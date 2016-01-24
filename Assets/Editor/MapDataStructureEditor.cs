using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MapDataStructure))]
public class MapDataStructureEditor : Editor
{
    private SerializedObject mObj;
    private SerializedProperty mPlayerSpawnList;
    private SerializedProperty mItemSpawnList;
    private SerializedProperty mMonsterSpawnList;
    private MapDataStructure mapData;
    private int playerListCount;
    private int itemListCount;
    private int monsterListCount;

    public void OnEnable()
    {
        mapData = (MapDataStructure)target;
        mObj = new SerializedObject(target);
        mPlayerSpawnList = mObj.FindProperty("playerSpawnSpots");
        mItemSpawnList = mObj.FindProperty("itemSpawnSpots");
        mMonsterSpawnList = mObj.FindProperty("monsterSpawnSpots");
    }

    public override void OnInspectorGUI()
    {
        playerListCount = mPlayerSpawnList.arraySize;
        itemListCount = mItemSpawnList.arraySize;
        monsterListCount = mMonsterSpawnList.arraySize;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add PlayerSpots"))
        {
            GameObject tmpObj = mapData.setPlayerSpots();
            Selection.activeGameObject = tmpObj;
            mapData.playerSpawnSpots.Add(tmpObj.transform);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete PlayerSpots"))
        {
            mapData.deletePlayerSpots();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add ItemSpots"))
        {
            GameObject tmpObj = mapData.setItemSpots();
            Selection.activeGameObject = tmpObj;
            mapData.itemSpawnSpots.Add(tmpObj.transform);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete ItemSpots"))
        {
            mapData.deleteItemSpots();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add MonsterSpots"))
        {
            GameObject tmpObj = mapData.setMonsterSpots();
            Selection.activeGameObject = tmpObj;
            mapData.monsterSpawnSpots.Add(tmpObj.transform);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Delete MonsterSpots"))
        {
            mapData.deleteMonsterSpots();
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
        mObj.ApplyModifiedProperties();
    }
}