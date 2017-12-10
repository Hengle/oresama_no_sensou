using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;


public class PreparationIcon : MonoBehaviour
{
    [SerializeField]
    private Image portrait;

    [SerializeField]
    private Button button;

    private string[] data;
    private int id;
    private Sprite sprite;
    private bool isPlayer;
    private bool hasChar;

    private bool showMenu = false;
    private List<string[]> availableChars = new List<string[]>();

    private string iconPath = "Sprite/characters_face/";


    public void Setup(int i, string[] r, bool p = false)
    {
        data = r;
        id = i;
        isPlayer = p;

        Sprite s;
        if (data.Length > 1)
        {
            s = Resources.Load<Sprite>(iconPath + data[2]);
            hasChar = true;
        }
        else
        { 
            s = Resources.Load<Sprite>(iconPath + "none");
            hasChar = false;
        }

    portrait.sprite = s;
        
    }

    public void PressedPortrait()
    {
        Debug.Log("boink");

        if (isPlayer)
        {
            PreparationSceneScript pss = PreparationSceneScript.pss;
            availableChars = pss.getAvailableChars();
            showMenu = true;
            Debug.Log("boink");
        }
    }

    void OnGUI()
    {
        if (showMenu)
        {
            int i = 0;
            foreach (string[] s in availableChars)
            {
                if (GUI.Button(new Rect(0, 0 + 40 * i, 200, 40), availableChars[i][0]))
                {
                    if (hasChar)
                        availableChars.Add(data);

                    data = availableChars[i];
                    Setup(id, data, true);


                    availableChars.Remove(availableChars[i]);
                    showMenu = false;

                }
                i++;
            }

            if (hasChar)
            { 
                if (GUI.Button(new Rect(0, 0 + 40 * i, 200, 40), "X"))
                {
                    //string[] c = data;
                    availableChars.Add(data);
                    data = new string[] { "" };
                    Setup(id, data, true);
                    showMenu = false;
                }
            }
        }
    }

}
