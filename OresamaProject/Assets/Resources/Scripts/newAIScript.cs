using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class newAIScript : MonoBehaviour {
	public static newAIScript NAI;
    public AIThinkingText t;

	void Awake(){
		NAI = this;
	}

	public struct ActionState{
		public Action action;//行動
		public void SetAction(Action action){ this.action = action;}
		public Action GetAction(){return action;}

		public int value;//及ぼせる効果量//複数回計算しなくて済むよう、ここにダメージ等保管 防御力の計算は都度行う
		public void SetValue(int value){this.value = value;}
		public int GetValue(){return value;}

		public bool isMagick;
		public void SetIsMagick(bool isMagick){this.isMagick = isMagick;}
		public bool GetIsMagick(){return isMagick;}
	}

    public struct MostAction{
        public Action action;
        public Character target;
    }

	//AIの開始
	//isMoveOnlyがtrueなら、行動の確認は移動範囲内のみ（範囲外の敵を感知しない）
    public IEnumerator AIStart(Character caster, bool isMoveRangeOnly = true)
    {
        t.setTarget(caster.gameObject);
        //自身の行えるAction確認
        List<ActionState> ActionList = ActionsCheck(caster);

        //移動力のリスト
        int[,] map = MoveRange(caster);
        yield return null;
        //行動評価点格納用の配列確保
        yield return StartCoroutine(PointsCheck(map, caster, ActionList, isMoveRangeOnly));

        CharacterMove CMove = caster.GetComponent<CharacterMove>();
        int[] maxPos = MaxPosCheck(points, CMove.nowPos);//デバッグ中
        //int[] maxPos = new int[2];
        yield return null;

        Debug.Log(maxPos[0] + ":" + maxPos[1] + "に移動して行動:" + points[maxPos[0], maxPos[1]]);

        t.setTarget(null);

        if (maxPos[0] != CMove.nowPos[0] && maxPos[1] != CMove.nowPos[1])
        {
            //ルーレットを回して移動力決定
            Invoke("RouretteStop", Random.Range(3, 6));
            yield return StartCoroutine(MapMoveScript.MMS.SetPlayer(caster.gameObject, true));

            //計算終了まで待機
            do
            {
                yield return null;
            } while (MapMoveScript.MMS.isCaluculated);

            int[] movePos = MovePosEnter(map, points, CMove.MovePower);
            Debug.Log("移動力は" + CMove.MovePower);
            Debug.Log(movePos[0] + ":" + movePos[1] + "に移動して行動:" + points[movePos[0], movePos[1]]);

            //移動に成功したら
            if (MapMoveScript.MMS.MapChipDragg(movePos[0], movePos[1]))
            {
                MenuScript.MS.MoveEnterButton();
                do
                {
                    yield return null;
                    Debug.Log("移動待機中");
                } while (MapMoveScript.MMS.isMoveEnd);
                yield return new WaitForSeconds(0.1f);
            }
        }

        /*行動の実行*/
        MostAction MA = MostActionCheck(caster, new int[2] { CMove.nowPos[0], CMove.nowPos[1]}, ActionList);
        if (MA.action != null)
        {
            Debug.Log(MA.action.GetName());
            if (MA.action == caster.GetAttackAction().GetComponent<Action>())
            {
                MenuScript.MS.AttackButton();
            }
            else
            {
                MenuScript.MS.SkillButton();
                for (int i = 0; i < caster.skills.GetLength(0); i++)
                {
                    if (MA.action == caster.skills[i].GetComponent<Action>())
                    {
                        MenuScript.MS.SkillEnterButton(i);
                    }
                }
            }
            do { yield return null; } while (ActionRange.AR.isAttackCaluculated);
            yield return new WaitForSeconds(0.1f);
            bool isTarget = false;
            //対象の選択
            if (!MA.action.isAoe())
            {
                isTarget = CharacterSelect.CS.CharaSelect(MA.target.gameObject);
            }
            else {

            }

            //対象の選択に成功した場合
            if (isTarget) {
                //MenuScript.MS.ActionEnterButton();
                StatusOverlay.SO.OkButton();
            }
        }
        else
        {
            Debug.Log("最適な行動がありませんでした");
            MenuScript.MS.BreakButton();
        }

        //yield return null;
    }

    void RouretteStop()
    {
        rouletteScript.RS.StopButton();
    }

    //casterの持つActionの確認
	List<ActionState> ActionsCheck(Character caster){
		List<ActionState> ASList = new List<ActionState>();

		//通常攻撃の確認
		GameObject Obj = caster.GetAttackAction();
		if (Obj != null) {
			Action action = Obj.GetComponent<Action> ();
            if (action.IsCanAction(caster)) {
                ASList.Add(ActionCheck(caster, action));
            }
		}

		//スキルの確認
		foreach (GameObject actionObj in caster.skills) {
			if (actionObj == null)
				continue;
			Action action = actionObj.GetComponent<Action> ();
            if (action.IsCanAction(caster)) {
                ASList.Add(ActionCheck(caster, action));
            }
		}

		return ASList;
	}

    //ActionStateの設定
	ActionState ActionCheck(Character caster,Action action){
		ActionState AS = new ActionState ();

		AS.SetAction (action);
		AS.SetValue (Mathf.FloorToInt (caster.GetStr () * action.str_mod + caster.GetAgi () * action.agi_mod + caster.GetInt () * action.int_mod));
		if (action.str_mod < action.int_mod) {
			AS.SetIsMagick (true);
		}

		return AS;
	}


    float[,] points;
	//各移動ポイントの評価点作成 計算結果はpointsの中に保存
	IEnumerator PointsCheck(int[,] map, Character caster,List<ActionState> ActionList, bool isMoveRangeOnly = true){
		int x = MapCreateScript.mapChips.GetLength (0);
		int y = MapCreateScript.mapChips.GetLength (1);
		points = new float[x, y];

		CharacterMove CMove = caster.GetComponent<CharacterMove> ();
        int MovePower = caster.GetMaxMove();
        int[] MaxPos = new int[2] { CMove.nowPos[0], CMove.nowPos[1] };//最大地点の保存用、全て0なら現在地が優先
		float MaxPoint = 0;

		int[] pos = new int[2];
		for (int i = 0; i < x; i++) {
			pos [0] = i;
			for (int j = 0; j < y; j++) {
				//侵入不可なら0pt
                if (map[i, j] >= 100)
                {
                    points[i, j] = 0;
                    continue;
                }
                else if(isMoveRangeOnly && map[i,j] > MovePower) 
                {
                    points[i, j] = 0;
                    continue;
                }
				//Debug.Log (map [i, j]);
				//そのマスにおける最適な行動とその評価点の確認
				pos [1] = j;
                //Debug.Log("pos" + pos[0] + ":" + pos[1]);
				float point = PointCheck (caster,pos, ActionList);
                //Debug.Log(i + ":" + j + ";" + point);

				if (point > MaxPoint) {
					MaxPoint = point;
					MaxPos [0] = i;
					MaxPos [1] = j;
				}
				points [i, j] = point;
                yield return null;
			}
		}
	}

    //一つの場所の評価点確認
	float PointCheck(Character caster,int[] pos,List<ActionState> ActionList){
		int[,] Range = AttackRange(pos);

		float MaxPoint = 0;
		float point = 0;
		foreach (ActionState a in ActionList) {
			List<Character> targets = TargetCheck (caster,Range,a);
			//Debug.Log (targets.Count);
			foreach (Character target in targets) {
				point = PointRating (a, target);

				if (point > MaxPoint)
					MaxPoint = point;
			}
		}

		return MaxPoint;
	}

    //最適な行動の確認
    MostAction MostActionCheck(Character caster, int[] pos, List<ActionState> ActionList)
    {
        MostAction MA = new MostAction();
        int[,] Range = AttackRange(pos);

        float MaxPoint = 0;
        float point = 0;
        foreach (ActionState a in ActionList)
        {
            List<Character> targets = TargetCheck(caster, Range, a);
            //Debug.Log (targets.Count);
            foreach (Character target in targets)
            {
                point = PointRating(a, target);

                if (point > MaxPoint)
                {
                    MaxPoint = point;
                    MA.action = a.GetAction();
                    MA.target = target;
                }
            }
        }

        return MA;
    }

    //ひとつのアクションの評価点確認
	int PointRating(ActionState a,Character target){
		int v = a.GetValue ();
		if (a.GetIsMagick ())
			v -= target.GetDef ();
		switch (a.action.GetActionType ()) {
		case Actions.Attack:
			//敵を倒せる場合は高評価
			if (v >= target.GetHp ())
				v += 100;
			break;
		case Actions.Heal:
			//味方の体力が半分以下なら高評価
			if (target.GetHp () <= target.GetMaxHp () / 2)
				v += 10;
			break;
		case Actions.Item:
			break;
		case Actions.ApplyStatus:
			break;
		default:
			break;
		}
		return v;
	}

	//最も評価点の高い地点の確認
	int[] MaxPosCheck(float[,] points,int[] nowPos){
        int[] pos = new int[2] { nowPos[0], nowPos[1] };

		int x = points.GetLength (0);
		int y = points.GetLength (1);
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				if (points [i, j] > points [pos [0], pos [1]]) {
					pos [0] = i;
					pos [1] = j;
				}
			}
		}

        //if (points[pos[0], pos[1]] > 0) Debug.Log(points[pos[0], pos[1]]);

		return pos;
	}

	//攻撃対象の確認
	List<Character> TargetCheck(Character caster,int[,] map,ActionState Action){
		List<Character> Targets = new List<Character>();
		int x = map.GetLength (0);
		int y = map.GetLength (1);
		//範囲内に対象がいればリストに追加
		for(int i = 0;i < x;i++){
			for (int j = 0; j < y; j++) {
				if (MapCreateScript.mapChips [i, j].RideCharacter != null && map [i, j] <= Action.action.GetRange ()) {
					Character target = MapCreateScript.mapChips [i, j].RideCharacter.GetComponent<Character> ();

					bool isEnemy;
					if (caster.tag == target.tag)
						isEnemy = false;
					else
						isEnemy = true;

					if (Action.action.canTargetSelf && caster.gameObject == target.gameObject) {
					} else if (Action.action.canTargetEnemy && isEnemy) {
					} else if (Action.action.canTargetFriendly && !isEnemy) {
					} else
						continue;

					Targets.Add (target);
				} else if (MapCreateScript.mapChips [i, j].RideCharacter != null) {
					//Debug.Log (map [i, j] + " " + i + ":" + j);
				}
			}
		}

		return Targets;
	}

    //移動先の決定
    int[] MovePosEnter(int[,] map, float[,] points, int movePower)
    {
        int x = map.GetLength(0);
        int y = map.GetLength(1);

        int[] pos = new int[2] { 0, 0 };

        float max = -100;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                if (map[i, j] <= movePower)
                {
                    if (max < points[i, j])
                    {
                        max = points[i, j];
                        pos[0] = i;
                        pos[1] = j;
                    }
                    else if (max == points[i, j] && Random.Range(0, 10) > 7)
                    {
                        pos[0] = i;
                        pos[1] = j;
                    }
                }
            }
        }

        return pos;
    }

    List<int[]> MovePath(int[] StartPos,int[] EndPos) {
        List<int[]> movePos = new List<int[]>();
        int[] pos = StartPos;
        int[] cPos = EndPos;

        Debug.Log(pos[0] + ":" + pos[1] + "から" + cPos[0] + ":" + cPos[1]);

        do
        {
            Debug.Log(pos[0] + ":" + pos[1]);
            if (pos[0] == MapCreateScript.mapChips[pos[0], pos[1]].movedPos[0] && pos[1] == MapCreateScript.mapChips[pos[0], pos[1]].movedPos[1])
                break;
            else if (cPos == MapCreateScript.mapChips[pos[0], pos[1]].movedPos)
                break;
            else
                pos = MapCreateScript.mapChips[pos[0], pos[1]].movedPos;
            if (movePos.Count > 100)
            {
                Debug.LogError("移動数が多すぎます");
                break;
            }
            movePos.Add(pos);
        } while (true);

        string s = "";
        foreach(int[] p in movePos){
            s += p[0] + ":" + p[1] + ";";
        }

        
        int del = movePos.Count - 1/* - needMove*/;
        for (int i = 0; i < del; i++)
        {
            movePos.RemoveAt(0);
        }
        movePos.Reverse();
        
        int count = 0;
        do
        {
            MapMoveScript.MMS.MapChipDragg(movePos[count][0], movePos[count][1]);
        } while (++count < movePos.Count);

        return movePos;
    }

    void ActionEnter(Character caster,Character target,Action action) {

    }

	//移動範囲の確認
	int[,] MoveRange(Character caster){
		int x = MapCreateScript.mapChips.GetLength (0);
		int y = MapCreateScript.mapChips.GetLength (1);
		int[,] map = new int[x, y];
		bool?[,] isEnd = new bool?[x, y];

		//初期化
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				map [i, j] = 1000;
				isEnd [i, j] = null;
			}
		}
		CharacterMove CMove = caster.GetComponent<CharacterMove> ();
		map [CMove.nowPos [0], CMove.nowPos [1]] = 0;
		isEnd[CMove.nowPos [0], CMove.nowPos [1]] = true;

		int[] findPos = new int[2]{ CMove.nowPos [0], CMove.nowPos [1] };
		int[] newPos = new int[2] { -1, -1 };
		bool isContinue;
		int count = 0;
		do {
			isContinue = false;
			int minCost = 1000;

			//現在地に隣接しているマスを検索
			int[,] n = MapMoveScript.MMS.NeighbourChip(findPos[0],findPos[1]);

			//nowPosに隣接しているマスの移動コストを計算、既存のものより低ければ更新する
			for(int i = 0;i < n.GetLength(0);i++){
				int cost = MapCreateScript.mapChips[n[i,0],n[i,1]].cost + map[findPos[0],findPos[1]];
				if(isEnd[n[i,0],n[i,1]] != true && map[n[i,0],n[i,1]] > cost){
					map[n[i,0],n[i,1]] = cost;
					isEnd[n[i,0],n[i,1]] = false;
                    if (MapCreateScript.mapChips[n[i, 0], n[i, 1]].RideCharacter != null)
                    {
                        map[n[i, 0], n[i, 1]] = 1000;
                        isEnd[n[i, 0], n[i, 1]] = true;
                    }
				}
			}

			//まだコストが確定していないコスト最小地点を探す
			for(int i = 0;i < x;i++){
				for(int j = 0;j < y;j++){
					if(isEnd[i,j] == false && map[i,j] < minCost){
						minCost = map[i,j];
						newPos[0] = i;
						newPos[1] = j;
						isContinue = true;
					}
				}
			}

			//コスト最小の地点への移動コストを確定させる
			if(newPos[0] >= 0){
				isEnd[newPos[0],newPos[1]] = true;
				//Debug.Log(newPos[0] + ":" + newPos[1] + "->" + minCost);
			}
			findPos = newPos;

			count++;
			/*if(count % 10 == 0){
				//Debug.Log(count + ":" + minCost + "->" + newPos[0] + ":" + newPos[1]);
				yield return null;
			}*/
		} while(isContinue);//コストが更新され続ける限り続ける

		return map;
	}

	//攻撃範囲の確認
	int[,] AttackRange(int[] pos){
		int x = MapCreateScript.mapChips.GetLength (0);
		int y = MapCreateScript.mapChips.GetLength (1);
		int[,] map = new int[x, y];
		bool?[,] isEnd = new bool?[x, y];

		//初期化
		for (int i = 0; i < x; i++) {
			for (int j = 0; j < y; j++) {
				map [i, j] = 100;
				isEnd [i, j] = null;
			}
		}

		map [pos[0], pos[1]] = 0;
		isEnd[pos[0], pos[1]] = true;

        int[] findPos = new int[2] { pos[0], pos[1] };
        //Debug.Log(x + ":" + y + " isFindpos");
		int[] newPos = new int[2] { -1, -1 };
		bool isContinue;
		int count = 0;
		do {
            //Debug.Log("do");
			isContinue = false;
			int minCost = 1000;

			//現在地に隣接しているマスを検索
			int[,] n = MapMoveScript.MMS.NeighbourChip(findPos[0],findPos[1]);
            //Debug.Log(n.GetLength(0));

			//nowPosに隣接しているマスの移動コストを計算、既存のものより低ければ更新する
			for(int i = 0;i < n.GetLength(0);i++){
                //Debug.Log(n[i, 0] + "" + n[i, 1]);
				int cost = 1 + map[findPos[0],findPos[1]];
				if(isEnd[n[i,0],n[i,1]] != true && map[n[i,0],n[i,1]] > cost){
					map[n[i,0],n[i,1]] = cost;
					isEnd[n[i,0],n[i,1]] = false;
                    //Debug.Log("更新");
				}
			}

			//まだコストが確定していないコスト最小地点を探す
			for(int i = 0;i < x;i++){
				for(int j = 0;j < y;j++){
					if(isEnd[i,j] == false && map[i,j] < minCost){
						minCost = map[i,j];
						newPos[0] = i;
						newPos[1] = j;
						isContinue = true;
                        //Debug.Log("更新2");
					}
				}
			}

			//コスト最小の地点への移動コストを確定させる
			if(newPos[0] >= 0){
				isEnd[newPos[0],newPos[1]] = true;
				//Debug.Log(newPos[0] + ":" + newPos[1] + "->" + minCost);
			}
			findPos = newPos;

			count++;
			/*if(count % 10 == 0){
				//Debug.Log(count + ":" + minCost + "->" + newPos[0] + ":" + newPos[1]);
				yield return null;
			}*/
		} while(isContinue);//コストが更新され続ける限り続ける

        if (farst)
        {
            for (int i = 0; i < x; i++)
            {
                string line = "";
                for (int j = 0; j < y; j++)
                {
                    line += map[i, j] + ",";
                }
                Debug.Log(line);
            }
            farst = false;
        }

		return map;
	}
	bool farst = false;
}
