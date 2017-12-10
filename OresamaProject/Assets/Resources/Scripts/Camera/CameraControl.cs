using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {
	public float x;
	public float y;

	void Start(){
		Vector2 s = MapCreateScript.MCS.mapChipSize;
		s.x *= MapCreateScript.mapChips.GetLength (0);
		s.y *= MapCreateScript.mapChips.GetLength (1) / 2;
		if (s.y < 0)
			s.y *= -1;

		x = s.x;
		y = s.y;

		Camera camera = GetComponent<Camera> ();
		Rect rect = calcAspect (x,y);
		camera.rect = rect;
	}

	private Rect calcAspect(float widgh,float height){
		float target_aspect = widgh / height;
		float window_aspect = (float)Screen.width / Screen.height;
		float scale_height = window_aspect / target_aspect;
		Rect rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f);

		if (1.0f > scale_height) {
			rect.x = 0;
			rect.y = (1.0f - scale_height) / 2.0f;
			rect.width = 1.0f;
			rect.height = scale_height;
		} else {
			float scale_widgh = 1.0f / scale_height;
			rect.x = (1.0f - scale_widgh) / 2.0f;
			rect.y = 0.0f;
			rect.width = scale_widgh;
			rect.height = 1.0f;
		}
		return rect;
	}
}
