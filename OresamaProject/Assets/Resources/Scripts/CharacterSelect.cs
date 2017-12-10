using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSelect : MonoBehaviour {
	public GameObject SelectMark;
	//[HideInInspector]
	public GameObject Player = null;//選択中のキャラクター
	public void SetPlayer(GameObject newPlayer){
		Player = newPlayer;
		StartCoroutine(MapMoveScript.MMS.SetPlayer (Player));
	}
	public static CharacterSelect CS;

	public Action action;
	public void SetAction(Action ac){ action = ac;}
	public List<GameObject> Target;

	public StatusOverlay SO;

	void Awake(){
		CS = this;
	}

	void Update(){
		//ターゲットに追従
		/*if (Target != null){
            SelectMark.transform.position = Target.transform.position;
        }
		else
			SelectMark.transform.position = new Vector3 (10000, 10000, 0);*/
	}

    //行動対象の選択
	public void SelectStart(GameObject caster, Action ac){
        //行動条件を満たしているか確認（MP等）
        if (!ac.IsCanAction(caster.GetComponent<Character>())) {
            Dialog.Dlog.SetText("MPが足りません");
            return;
        }

		action = ac;
		Target = new List<GameObject> ();

		Player = caster;
		CharacterMove CMove = caster.GetComponent<CharacterMove> ();

		GameController.Gcon.isCharacterSelect = true;
		if (action.isAoe()) {
			StartCoroutine (AOEAction (CMove,ac));
			return;
		}

		//攻撃範囲の確認
		StartCoroutine (ActionRange.AR.AttackCost (CMove.nowPos [0], CMove.nowPos [1], action));
	}

	public IEnumerator AOEAction(CharacterMove CMove,Action ac){
		//攻撃範囲の確認
		yield return StartCoroutine (ActionRange.AR.AttackCost (CMove.nowPos [0], CMove.nowPos [1], ac));
		List<GameObject> targets = AOEActionTarget ();
		foreach (GameObject t in targets) {
			Debug.Log (t.name);
		}
		if (targets.Count > 0)
			CharaSelect (targets);
		else {
			Debug.Log ("行動対象が存在しません");
			selectCancel ();
		}
	}

	public List<GameObject> AOEActionTarget() {
        List<GameObject> targets = new List<GameObject>();
        //全てのマスを確認
        for (int i = 0; i < MapCreateScript.mapChips.GetLength(0); i++)
        {
            for (int j = 0; j < MapCreateScript.mapChips.GetLength(1); j++)
            {
				if (MapCreateScript.mapChips [i, j].RideCharacter == null)
					continue;
                //自身と違う陣営のキャラクターを確認して保存
				if(isTarget(MapCreateScript.mapChips [i, j].RideCharacter)){
					targets.Add (MapCreateScript.mapChips [i, j].RideCharacter);
				}
            }
        }

		return targets;
    }

    public void Lis(List<GameObject> g) { }

    //行動のキャンセル
	public void selectCancel(){
		action = null;
		Target = new List<GameObject> ();
		Player = null;
        GameController.Gcon.isCharacterSelect = false;
		MapMoveScript.MMS.MapChipFormat ();
	}

    /*
	//行動のキャンセル
	public void ActionCancel(){
		//アクションの初期化
		action = null;
		//キャラクター選択フラグを下げる
		GameController.isCharacterSelect = false;
	}
    */



	//行動対象の選択
	public bool CharaSelect(List<GameObject> objs){
		if (objs.Count <= 0 || Player == null)
			return false;
		Debug.Log (objs[0] + "を選択しました");

		//有効対象か確認
		foreach(GameObject o in objs){
			if(isTarget(o) == false)return false;
		}

		//
		Target = objs;
		MenuScript.MS.ActionEnterButton ();
		return true;
	}

	//対象選択（一人用）
	public bool CharaSelect(GameObject obj){
		List<GameObject> objs = new List<GameObject>();
		objs.Add (obj);
		return CharaSelect (objs);
	}

	//行動対象の決定
	public void ActionEnter(){
		if (Target.Count <= 0) {
			Debug.Log ("まだ対象が選択されていません");
			return;
		}

		List<GameObject> t = Target;
        //t.Add(Target);
		//アクションの開始
		StartCoroutine (ActionStart (t));
	}

	IEnumerator ActionStart(List<GameObject> t)
	{
		//次の行動まで待機
		SO.Init(Player,t,action);
		//yield return action.Attack(t);

		//攻撃を終了する
		ActionEnd();
		yield return null;
	}

    public void ActionEnd() {
        //行動の初期化
        action = null;
		Target = new List<GameObject> ();
        Player = null;
        GameController.Gcon.isCharacterSelect = false;
        MapMoveScript.MMS.MapChipFormat();

        //次のキャラクターの行動待ちに入る
        
    }

	//有効対象か確認
	private bool isTarget(GameObject t){
		bool isEnemy;//対象が自身の敵か確認
		if (Player.tag == t.tag)//同じ陣営（味方）
			isEnemy = false;
		else//違う陣営（敵）
			isEnemy = true;

		CharacterMove Cmove = t.GetComponent<CharacterMove>();

		//攻撃範囲内か確認
		if (MapCreateScript.mapChips [Cmove.nowPos [0], Cmove.nowPos [1]].MoveCost <= action.GetRange()) {
			//敵に攻撃
			if (isEnemy && action.type == Actions.Attack/* || action.type == ActionType.debuff*/) {

			}
			//味方に補助
			else if (!isEnemy && action.type == Actions.Heal || action.type == Actions.ApplyStatus || action.type == Actions.Item) {

			} else
				return false;
		}
		else{//対象が間違っていれば弾く
			Debug.Log("対象が間違っています！");
			return false;
		}

		return true;
	}
}
