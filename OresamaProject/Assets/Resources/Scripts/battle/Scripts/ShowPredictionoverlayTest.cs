using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowPredictionoverlayTest : MonoBehaviour {

    public GameObject caster;
	public List<GameObject> target;
    public Action action;


	// Use this for initialization
	void Start () 
    {
        GameObject overlay = (GameObject)Instantiate(Resources.Load("prefabs/PredictionOverlay"));
        overlay.GetComponent<StatusOverlay>().Init(caster, target, action);
	}

	
	// Update is called once per frame
	void Update () {
	
	}
}
