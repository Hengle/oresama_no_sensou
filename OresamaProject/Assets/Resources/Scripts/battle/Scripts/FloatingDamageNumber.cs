using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FloatingDamageNumber : MonoBehaviour
{

    public Text txt;
    public Text shadow;

    private int damageNumber;
	
    void Start () 
    {
        iTween.MoveBy(this.gameObject, iTween.Hash("y", 35f, "time", 1.0f, "oncomplete", "RemoveMe", "easetype", iTween.EaseType.easeOutCirc));
	}


    public void RemoveMe() 
    {
        Destroy(this.gameObject);
    }

    
    public void SetDmgText(int v) 
    {
        if (v > 0)
            txt.color = Color.red;
        else
        { 
            txt.color = Color.green;
            v = Mathf.Abs(v);
        }

        txt.text = "" + v;
        shadow.text = txt.text;
    }
	
}
