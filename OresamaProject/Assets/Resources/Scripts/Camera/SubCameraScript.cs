using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SubCameraScript : MonoBehaviour {
	public RectTransform Back;

	// Use this for initialization
	void Start () {
		Vector3 pos = MapCreateScript.MCS.mapChipSize;
		pos.x *= MapCreateScript.mapChips.GetLength (0) / 2;
		pos.x += MapCreateScript.MCS.mapChipSize.x / 2;
		pos.y *= MapCreateScript.mapChips.GetLength (1) / 2;
		pos.z = -10;

		transform.position = pos;

		/*
		Camera camera = GetComponent<Camera> ();
		Rect r = camera.pixelRect;
		Debug.Log (camera.pixelRect);
		Vector2 p = new Vector2 (r.x + (r.width / 2), r.y + (r.height / 2));
		Vector2 Aspect = Back.transform.parent.GetComponent<CanvasScaler> ().referenceResolution;
		Debug.Log (Aspect);
		p = p - Aspect / 2;
		*/
		//Back.localPosition = p;
	}
}
