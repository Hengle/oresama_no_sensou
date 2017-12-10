using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPBarScript : MonoBehaviour {

    public Text charNameText;

    private bool enemy = false;
    public void setEnemyBool(bool v) { enemy = v; }
    public bool getEnemyBool() { return enemy; }

    public RectTransform uGuiElement;


    public void SetText(string s) 
    {
        charNameText.text = s;
    }


    public void SlideIn() 
    {
        Vector2 startPos = new Vector2(-250, 290);
        Vector2 endPos = new Vector2(-250, 230);
        if (getEnemyBool()) 
        {
            startPos = new Vector2(250, 290);
            endPos = new Vector2(250, 230);
        }

        GameObject bar = this.gameObject;
        iTween.ValueTo(bar, iTween.Hash(
       "from", startPos,
       "to", endPos,
       "time", 0.3f,
       "onupdatetarget", this.gameObject,
       "delay", 2.0f,
       "onupdate", "MoveGuiElement"));
    }


    public void SlideOut()
    {
        Vector2 endPos = new Vector2(-250, 290);
        Vector2 startPos = new Vector2(-250, 230);
        if (getEnemyBool())
        {
            endPos = new Vector2(250, 290);
            startPos = new Vector2(250, 230);
        }

        GameObject bar = this.gameObject;
        iTween.ValueTo(bar, iTween.Hash(
       "from", startPos,
       "to", endPos,
       "time", 0.3f,
       "onupdatetarget", this.gameObject,
       "onupdate", "MoveGuiElement"));
    }


    public void MoveGuiElement(Vector2 position)
    {
        uGuiElement.anchoredPosition = position;
    }
}
