using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ShadowText : MonoBehaviour {
    public Text target;
    private Text t;
    private Vector3 offset;

    void Start() {
        t = GetComponent<Text>();

        if (t == null || target == null) Destroy(this);

        offset = transform.position - target.transform.position;
    }

	void Update () {
        t.text = target.text;

        Vector3 tPos = target.transform.position;
        transform.position = tPos + offset;
        target.transform.position = tPos;
	}
}
