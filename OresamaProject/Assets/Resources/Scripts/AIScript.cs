using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AIScript : MonoBehaviour {
	public static AIScript AI;

	void Awake(){
		AI = this;
	}

	//行動の優先順位確認用
	public struct ActionState{
		public Action action;//行動
		public List<float> level;//優先度 高いほど優先
		public int value;//及ぼせる効果量
		public bool isDestroy;//敵を倒せるか否か
		public bool isHelp;//味方を助けるか否か
		public int AttackRange;//移動と合わせて攻撃可能な距離
		public List<Character> targets;//行動対象になりうるキャラクター
		public List<int> needMove;//行動に必要な移動距離 対象キャラクター数分
		public List<float> probabillty;//移動の成功率 1 ~ 0

		public void Setvalue(int value){ this.value = value; }
		//public void Setlevel(int level){ this.level = level; }
		//public void Setprobabillty(float probabillty){ this.probabillty = probabillty; }
		public void SetAttackRange(int AttackRange){ this.AttackRange = AttackRange; }
	}

	public void AIIsStart(){
		if (GameRoot.GRoot.enemyList.Count > 0) {
			Character c = GameRoot.GRoot.enemyList [0].GetComponent<Character> ();
			GameController.Gcon.SetTarnCharacter (c.gameObject);
			StartCoroutine (AIStart (c));
		}
			
	}

	//AcitonState初期化関数
	ActionState InitActionState(){
		ActionState a = new ActionState();
		a.action = null;//行動
		//a.level = -1000;//優先度 高いほど優先
		a.value = 0;//及ぼせる効果量
		a.probabillty = new List<float>();//移動の成功率
		a.isDestroy = false;//敵を倒せるか否か
		a.isHelp = false;//味方を助けるか否か
		a.AttackRange = 0;//移動と合わせて攻撃可能な距離
		a.targets = new List<Character>();
		a.needMove = new List<int> ();
		a.level = new List<float> ();
		return a;
	}

	public IEnumerator AIStart(Character caster){
		//StartCoroutine (newAIScript.NAI.AIStart (caster));

		Debug.Log ("AI");
		//そのキャラクターの行動の種類を確認
		List<ActionState> ActionList = ActionCheck(caster);
		//ActionList.Add()
		//実行できる行動を確認
		//CharacterMove CMove = caster.GetComponent<CharacterMove>();

		//行動対象を確認
		yield return StartCoroutine(TargetsFind(caster,ActionList));

		//どれくらいの確率で成功するか確認（移動のランダムに成功する確率）
		List<int> moveList = new List<int>(caster.GetMove());
		//昇順で並び替え
		moveList.Sort();
		moveList.Reverse ();
		List<float> movePer = new List<float>();
		float per = 1 / moveList.Count;//どれか一つの移動力の出る確率
		//低いものから順にそれ以上の数字が出る確率計算
		int c = 0;//同じ数字が連続して出た回数
		for (int i = 0; i < moveList.Count; i++) {
			//次の要素と値が同じならcontinue 次とまとめて計算
			if (i < moveList.Count - 1 && moveList [i] == moveList [i + 1]) {
				c++;
				continue;
			}

			//同じ数字が出ていたらその要素数だけ確率をadd
			do {
				movePer.Add (i * per);//確認済みの要素数*一つの確立（降順）
			} while(c-- > 0);
		}
		//低　～　高　に並べ替え
		moveList.Sort ();
		movePer.Sort ();
		//全ての行動を確認
		for (int i = 0; i < ActionList.Count; i++) {
			//必要な移動力を確認
			foreach (int n in ActionList[i].needMove) {
				//移動力を確認
				for (int j = 0; j < moveList.Count; j++) {
					//移動力が足りていたらその移動力を代入
					if (n <= moveList [j]) {
						float x = movePer [j];
						ActionList [i].probabillty.Add (x);
						break;
					}else if(j == moveList.Count - 1){
						float x = 0;
						ActionList [i].probabillty.Add (x);
					}
				}
			}
		}

		string s = "";
		foreach (ActionState a in ActionList) {
			s += a.action.name + " ";
		}
		Debug.Log (s);

		//行動の優先順位を確認
		Action action = null;
		Character target = null;
		float p = -1000;
		int needMove = 0;
		for(int i = 0 ;i < ActionList.Count;i++){
			for (int j = 0; j < ActionList [i].targets.Count; j++) {
				float le = ActionList [i].value;
				le *= ActionList [i].probabillty [j];
				ActionList [i].level.Add (le);
				if (le > p) {
					p = le;
					action = ActionList [i].action;
					target = ActionList [i].targets [j];
					needMove = ActionList [i].needMove [j];
				}
			}
		}


		//ある程度ランダムで行動を決定 //やっぱなし


		//行動を実行
		//Debug.Log (target.GetName () + "に" + action.name + "を実行したいです");
        if (target == null || action == null) {
            MenuScript.MS.BreakButton();
            yield break;
        }
		Debug.Log (needMove + "cost必要です");
		if (needMove > 0) {
			Invoke ("RouretteStop", Random.Range (2, 5));//ルーレットの停止
			yield return StartCoroutine (MapMoveScript.MMS.SetPlayer (caster.gameObject,true));

			do {
				yield return null;
			} while(MapMoveScript.MMS.isCaluculated);

			if (/*rouletteScript.RS.nowMoveSpeed <= needMove*/true) {
				List<int[]> movePos = new List<int[]> ();
				int[] pos = target.GetComponent<CharacterMove> ().nowPos;
				int[] cPos = caster.GetComponent<CharacterMove> ().nowPos;
				Debug.Log (target.GetName () + ":" + action.name);
				do {
					Debug.Log (pos [0] + ":" + pos [1]);
					if (pos[0] == MapCreateScript.mapChips [pos [0], pos [1]].movedPos[0] && pos[1] == MapCreateScript.mapChips [pos [0], pos [1]].movedPos[1])
						break;
					else if(cPos == MapCreateScript.mapChips [pos [0], pos [1]].movedPos)
						break;
					else
						pos = MapCreateScript.mapChips [pos [0], pos [1]].movedPos;
					if (movePos.Count > 100) {
						Debug.LogError ("移動数が多すぎます");
						break;
					}
					movePos.Add (pos);
				} while(true);
				s = "";
				foreach (int[] i in movePos) {
					s += i [0] + ":" + i [1] + " to ";
				}
				Debug.Log (s);
				int del = movePos.Count - 1  - needMove;
				Debug.Log (movePos.Count + "から" + del + "個消去:" + needMove);
				for (int i = 0; i < del; i++) {
					movePos.RemoveAt (0);
				}
				movePos.Reverse ();
				s = "";
				foreach (int[] i in movePos) {
					s += i [0] + ":" + i [1] + " to ";
				}
				Debug.Log (s);

				int count = 0;
				do {
					MapMoveScript.MMS.MapChipDragg (movePos [count] [0], movePos [count] [1]);
				} while(++count < movePos.Count);

				yield return new WaitForSeconds (1.0f);

				MenuScript.MS.MoveEnterButton ();
			} /*else {
				Debug.Log ("移動力が足りませんでした");
			}*/
			while (GameController.Gcon.GamePhase != Phase.MenuAtMoveEnd) {
				yield return null;
			}
		} else {
			Debug.Log ("移動が必要ありません");
		}

        yield return new WaitForSeconds(0.5f);

		if (action == caster.GetAttackAction ().GetComponent<Action> ()) {
			MenuScript.MS.AttackButton ();
		} else
			for (int i = 0; i < caster.skills.GetLength (0); i++) {
				if (action == caster.skills [i].GetComponent<Action> ()) {
					MenuScript.MS.SkillEnterButton (i);
					break;
				}
			}

        yield return new WaitForSeconds(0.5f);
		//行動対象の指定に成功したら
		if (CharacterSelect.CS.CharaSelect (target.gameObject)) {
			yield return new WaitForSeconds (1.0f);
			StatusOverlay.SO.OkButton ();
		} else {
			Debug.Log ("行動できませんでした");
            MenuScript.MS.ActionCancelButton();
			MenuScript.MS.BreakButton ();
		}
		Debug.Log ("移動終了");

		//途中で実行が続けられなくなったら中止して待機、若しくは別の行動に移る
		yield return null;
		Debug.Log ("end");
	}

	void RouretteStop(){
		rouletteScript.RS.StopButton ();
	}

	//全行動の確認
	List<ActionState> ActionCheck(Character caster){
		Debug.Log ("ActionCheck");
		List<ActionState> ActionList = new List<ActionState>();

		foreach(GameObject ac in caster.skills){
			Action a = ac.GetComponent<Action> ();
			ActionState aState = InitActionState ();
			aState.action = a;
			aState.value = Mathf.FloorToInt (caster.GetStr () * a.str_mod + caster.GetAgi () * a.agi_mod + caster.GetInt () * a.int_mod);
			aState.AttackRange = a.GetRange () + caster.GetMaxMove ();

			ActionList.Add (aState);
		}

		GameObject aObj = caster.GetAttackAction ();
		Action action = aObj.GetComponent<Action> ();
		ActionState AState = InitActionState ();
		AState.action = action;
		AState.value = Mathf.FloorToInt (caster.GetStr () * action.str_mod + caster.GetAgi () * action.agi_mod + caster.GetInt () * action.int_mod);
		AState.AttackRange = action.GetRange () + caster.GetMaxMove ();

		ActionList.Add (AState);

		return ActionList;
	}

	//AcitonState内の攻撃対象を検索する 注：行動を行うキャラクターは統一すること 内部で攻撃距離の計算をしているので、他のところで移動や攻撃の計算をしているときは要注意
	IEnumerator TargetsFind(Character caster,List<ActionState> actions){
		Debug.Log ("TargetsFind");

		CharacterMove CMove = caster.gameObject.GetComponent<CharacterMove> ();
		//攻撃距離を確認
		yield return StartCoroutine (ActionRange.AR.AttackCost (CMove.nowPos [0], CMove.nowPos [1], actions[0].action));

		//全行動を確認
		for (int i = 0; i < actions.Count; i++) {
			//エネミーが行動対象なら全エネミーを確認
			if((actions[i].action.canTargetFriendly && caster.tag == "Enemy") || (actions[i].action.canTargetEnemy && caster.tag == "Player")){
				foreach (GameObject t in GameRoot.GRoot.enemyList) {
					CharacterMove tCmove = t.GetComponent<CharacterMove> ();
					int[] pos = tCmove.nowPos;
					int Long = MapCreateScript.mapChips [pos [0], pos [1]].MoveCost;
					if (Long <= actions [i].AttackRange) {
						Character target = t.GetComponent<Character> ();
						actions [i].targets.Add (target);
						actions [i].needMove.Add (Long - actions [i].action.GetRange ());
					}
				}
			}
			//プレイヤーが行動対象なら全プレイヤーを確認
			if ((actions[i].action.canTargetEnemy && caster.tag == "Enemy") || (actions[i].action.canTargetFriendly && caster.tag == "Player")) {
				foreach (GameObject t in GameRoot.GRoot.playerList) {
					CharacterMove tCmove = t.GetComponent<CharacterMove> ();
					int[] pos = tCmove.nowPos;
					int Long = MapCreateScript.mapChips [pos [0], pos [1]].MoveCost;
					if (Long <= actions [i].AttackRange) {
						Character target = t.GetComponent<Character> ();
						Debug.Log (actions[i].targets.Count);
						actions [i].targets.Add (target);
						actions [i].needMove.Add (Long - actions [i].action.GetRange ());
					}
				}
			}
			//自身が対象に含まれるか確認
			if (actions [i].action.canTargetSelf){
				actions [i].targets.Add (caster);
				actions [i].needMove.Add (0);
			}
				
		}
		yield return null;//重そうなので処理を分割
	}

}
