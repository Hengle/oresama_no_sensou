using UnityEngine;
using System.Collections;

class PlayerChar
{
   public PlayerChar(string prefab)
    {
        playerPrefab = prefab;
    }
    private string playerPrefab;
    public string getPlayerPrefab()
    {
        return playerPrefab;
    }
}


public class PlayerCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
