using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Dialog : MonoBehaviour {
    public static Dialog Dlog;
    public GameObject DialogWindow;
    private Image DialogImage = null;
    const float ActiveTime = 5.0f;
    private float TimeCounter = 0;
    public Text[] text;
    public void SetText(string message){
        int length = text.GetLength(0);
        for (int i = 0; i < length; i++)
        {
            if (text[i].text == "") {
                text[i].text = message;
                break;
            }
            if (i + 1 != length) text[i].text = text[i + 1].text;
            else text[i].text = message;
        }
            TimeCounter = 0;
    }

    void Awake() {
        Dlog = this;
        DialogDelete();
    }

    void Update() {
        TimeCounter += Time.deltaTime;

        if (TimeCounter > ActiveTime)
        {
            DialogWindow.SetActive(false);
        }
        else if (TimeCounter > ActiveTime / 2)
        {
            DialogAlpha((ActiveTime - TimeCounter) / (ActiveTime / 2));
        }
        else {
            DialogWindow.SetActive(true);
            DialogAlpha(1);
        } 
    }

    public void DialogDelete() {
        TimeCounter += ActiveTime;
        int length = text.GetLength(0);
        for (int i = 0; i < length; i++) { text[i].text = ""; }
    }

    void DialogAlpha(float A) {
        if (DialogImage == null) DialogImage = DialogWindow.GetComponent<Image>();
        Color c = DialogImage.color;
        c.a = A;
        DialogImage.color = c;
        foreach (Text t in text) {
            c = t.color;
            c.a = A;
            t.color = c;
        }
    }

    public void TestButton(string message) {
        SetText(message);
    }
}
