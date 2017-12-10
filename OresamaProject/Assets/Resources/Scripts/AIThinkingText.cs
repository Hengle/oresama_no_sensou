using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AIThinkingText : MonoBehaviour {
    private Text t;
    private const string defaultText = "思考中";
    private const string addText = "･･･";
    private int nowTextLength = 0;

    private float time = 0;
    private float lastTime = 0;
    private const float updateTime = 0.5f;

    public Vector3 offSet;
    private GameObject target;

    void Awake() {
        t = GetComponent<Text>();
        if (t == null) Destroy(gameObject);
    }

    public void setTarget(GameObject target) {
        this.target = target;
    }

    void Update() {
        if (target != null)
        {
            //時間の加算
            time += Time.deltaTime;
            float elapsed = time - lastTime;
            if (elapsed > updateTime)
            {
                nowTextLength += (int)(elapsed / updateTime);
                nowTextLength = nowTextLength % (addText.Length + 1);
                lastTime = time;
            }

            string think = defaultText + addText.Substring(0, nowTextLength);
            t.text = think;

            transform.position = target.transform.position + offSet;
        }
        else {
            transform.position = new Vector3(-1000, -1000, -1000);
        }
    }
}
