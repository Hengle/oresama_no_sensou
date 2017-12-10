using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class StatusWindow : MonoBehaviour
{
    public GameObject chara;//セットされているキャラクター

    public GameObject charPortrait;//キャラクターの画像配置Image
    public Image classIcon;

    public Text nameText, classText;
    public GameObject hpBar, mpBar;
    public Text strText, agiText, intText, conText, defText, spdText;

    public bool flipCharacter;

    private const float hpBarMaxWidth = 1;
    private const float mpBarMaxWidth = 1;


    void Start()
    {
        if (flipCharacter)
        {
            charPortrait.transform.localScale = new Vector2(-1, 1);
            //charPortrait.transform.position += new Vector3(60, 0, 0);
        }
    }


	public void SetCharacter(GameObject c)
    {
        chara = c;
        /*
        Rect hpBarRect = hpBar.GetComponent<RectTransform>().rect;
        hpBarMaxWidth = hpBarRect.width;

        Rect mpBarRect = mpBar.GetComponent<RectTransform>().rect;
        mpBarMaxWidth = mpBarRect.width;

        if (flipCharacter)
        {
            charPortrait.transform.localScale = new Vector2(-1, 1);
            charPortrait.transform.position += new Vector3(60, 0, 0);
        }
        */
        UpdateStats();
    }

	//ステータスの更新
    public void UpdateStats()
    {
        Character c = chara.GetComponent<Character>();

        nameText.text = c.GetName();
        classText.text = c.GetClass();

        classIcon.sprite = c.classIcon;
		charPortrait.GetComponent<Image>().sprite = c.charFace;
        //classIcon.GetComponent<SpriteRenderer>().sprite = c.classIcon;


        strText.text = "Str: " + c.GetStr();
        agiText.text = "Agi: " + c.GetAgi();
        intText.text = "Int: " + c.GetInt();
        conText.text = "Con: " + c.GetCon();
        defText.text = "Def: " + c.GetDef();
        spdText.text = "Spd: " + c.GetSpd();

        //RectTransform hpBarRect = hpBar.GetComponent<RectTransform>();
        float hpBarChangedWidth = hpBarMaxWidth * (c.GetHp() / c.GetMaxHp());
        hpBar.transform.localScale = new Vector2(hpBarChangedWidth, hpBar.transform.localScale.y);
        //hpBarRect.sizeDelta = new Vector2(hpBarChangedWidth, 7);

        //RectTransform mpBarRect = mpBar.GetComponent<RectTransform>();
        float mpBarChangedWidth = mpBarMaxWidth * (c.GetMp() / c.GetMaxMp());
        mpBar.transform.localScale = new Vector2(mpBarChangedWidth, mpBar.transform.localScale.y);
        //mpBarRect.sizeDelta = new Vector2(mpBarChangedWidth, 7);

        /*

       RectTransform mpBarRect = mpBar.GetComponent<RectTransform>();
       //hpBarMaxWidth = hpBarRect.width;
       float mpBarChangedWidth = mpBarMaxWidth * (c.GetMp() / c.GetMaxMp());
       mpBarRect.sizeDelta = new Vector2(mpBarChangedWidth, 7);*/
    }

    // Update is called once per frame
    void Update()
    {
        // UpdateStats();
    }
}
