using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIFadeScript : MonoBehaviour {
	public GameObject testObj;
	public float time;
	public GameObject textObj;
	public static UIFadeScript UIFade;

    public GameObject WinConditions;
    public GameObject Win;
    public GameObject LosConditions;
    public GameObject Los;

	void Awake(){
		UIFade = this;

        textObj.SetActive(false);
        WinConditions.SetActive(false);
        Win.SetActive(false);
        LosConditions.SetActive(false);
        Los.SetActive(false);
	}

	public IEnumerator TextFade(float fadetime, string s,Color c){
		Text t = textObj.GetComponent<Text> ();
		t.text = s;
		t.color = c;
		yield return StartCoroutine (Fade (textObj, fadetime));
		t.color = Color.black;
	}

	public IEnumerator Fade(GameObject ui,float fadeTime){
		RectTransform rect = ui.GetComponent<RectTransform> ();
		if (rect == null)
			yield break;

		rect.gameObject.SetActive (true);
		Vector3 defPos = rect.position;
		rect.localPosition = Vector3.zero;

		yield return new WaitForSeconds(fadeTime);

		rect.gameObject.SetActive (false);
		rect.position = defPos;
	}
}
