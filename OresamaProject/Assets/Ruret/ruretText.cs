using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ruretText : MonoBehaviour {
	public Text onText;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	void Update () {
		onText.text = rouletteScript.RS.nowMoveSpeed.ToString ();
	}
}
