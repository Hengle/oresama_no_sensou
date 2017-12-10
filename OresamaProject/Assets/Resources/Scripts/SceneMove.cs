using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneMove : MonoBehaviour {
    public int MoveToScene;

	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0)) {
            SceneManager.LoadScene(MoveToScene);
        }
	}
}
