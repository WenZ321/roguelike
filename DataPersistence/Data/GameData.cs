using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // Same Map ID
    public SerializableDictionary<string, GameState> gameState; //ID of the scene, 
    public SerializableDictionary<string, MapState> mapState;   //ID of the scene, 

    public string LatestMapPlayed;
    public SerializableDictionary<string, string> savedScenes;  //name of the scene, ID of the scene


    // The values defined in this constructor will be the default values
    // The game starts with this when there's no data to Load
    public GameData()
    {
        LatestMapPlayed = null;
        savedScenes = new SerializableDictionary<string, string>();
        gameState = new SerializableDictionary<string, GameState>();
        mapState = new SerializableDictionary<string, MapState>();  

    }
}
