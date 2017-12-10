using UnityEngine;
using System.Collections;

public class GameData : MonoBehaviour {

    public static GameData gd;

    void Awake() 
    {
        if (gd == null)
            gd = this;

        //DontDestroyOnLoad(this);
    }

    public GameObject[] characters;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
