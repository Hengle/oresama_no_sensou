using UnityEngine;
using System.Collections;

public class AutoLayer : MonoBehaviour {
	public int order;

	// Update is called once per frame
	void Update () {
		gameObject.transform.SetSiblingIndex (100);
	}
}
