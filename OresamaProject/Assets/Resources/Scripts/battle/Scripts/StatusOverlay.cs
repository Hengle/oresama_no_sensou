using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StatusOverlay : MonoBehaviour
{
	public GameObject ActionWIndow;

    public StatusWindow CasterWindow;
    public StatusWindow TargetWindow;
    public Text ActionText;

    public Image arrow;

    public Sprite attackSprite;
    public Sprite healSprite;
    public Sprite magicSprite;

	GameRoot GR;

	public Action action;

	public static StatusOverlay SO;

	void Awake(){
		SO = this;
		ActionWIndow.SetActive (false);
	}

	//actionの呼び出し
	public void Init(GameObject caster, List<GameObject> target, Action a)
    {
        GR = GameRoot.GRoot;

        action = a;

		//ステータスウィンドウの表示
        CasterWindow.SetCharacter(caster);
        TargetWindow.SetCharacter(target[0]);
        ActionText.text = a.GetName();

		//行動キャラクターの設定
		action.SetCaster(caster.GetComponent<Character>());
		//対象キャラクターの設定
		action.RemoveTargets();
		action.AddTargets(target);

		switch (a.type) {
		case Actions.Attack:
			arrow.sprite = attackSprite;
			break;
		case Actions.Heal:
			arrow.sprite = healSprite;
			break;
		case Actions.ApplyStatus:
			arrow.sprite = magicSprite;
			break;
		case Actions.Item:
			arrow.sprite = magicSprite;
			break;
		default:
			arrow.sprite = attackSprite;
			break;
		}
		/*
        if (a.type == Actions.Attack)
            arrow.sprite = attackSprite;
        else if (a.type == Actions.Heal)
            arrow.sprite = healSprite;
        else if (a.type == Actions.ApplyStatus)
            arrow.sprite = magicSprite;
            */

		ActionWIndow.SetActive (true);
    }

	public void EnemyTarn(){
		GameObject CB = GameObject.Find ("CancelButton");
		CB.SetActive (false);
	}

    public void OkButton()
    {
		GR.ActionQueue.Clear ();
		//行動リストを追加
        GR.ActionQueue.Add(action.gameObject);

		//戦闘中フラグを立てる
		GameController.Gcon.GamePhase = Phase.Battle;

        //移動を反映
		/*
        MMS.MoveUpdate(true);
        */

		//SceneManager.LoadScene("battletest");
		//Invoke("Move",0.2f);
		/*
        Invoke("SceneMove", MapMoveScript.eventDelay);
        */
        //Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
		//行動を実行
		BattleSceneControl.BSC.BattleStart ();
		ActionWIndow.SetActive (false);
    }

    void Move() {
		/*
        Invoke("SceneMove", MapMoveScript.eventDelay);
        */
    }

    void SceneMove() 
    {
        //SceneManager.LoadScene("battletest");
    }

    public void CancelButton()
    {
        Debug.Log("CANCEL");
        //Destroy(this.gameObject);
		/*
        MMS.charSelectTime = false;
        */
		ActionWIndow.SetActive (false);
    }
}
