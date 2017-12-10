using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MovePowerWindow : MonoBehaviour {
    public static MovePowerWindow MPW;
    private GameObject Chara;
    public GameObject target;
    public Text MovePowerText;
    private int maxMovePower;
    private int nowMovePower;

    void Awake() {
        MPW = this;
    }
	
	// Update is called once per frame
	void Update () {
        if (GameController.Gcon.GamePhase != Phase.Move) {
            RemoveAtScreen();
            return;
        }

        if (target == null && Chara != null) {
            transform.position = Chara.transform.position + new Vector3(-0.15f, 0.2f, 0);
        }
        else if (target == null) RemoveAtScreen();
        else transform.position = target.transform.position;

        MovePowerText.text = (maxMovePower - MapMoveScript.MMS.GetSelectMove()).ToString();
	}

    void RemoveAtScreen() {
        transform.position = new Vector3(-100, -100, -100);
    }

    public void SetTarget(GameObject target) {
        this.target = target;
    }

    public void SetCharacter(GameObject Chara) {
        this.Chara = Chara;
    }

    public void SetMovePower(int movePower) {
        maxMovePower = movePower;
        nowMovePower = maxMovePower;
    }
}
