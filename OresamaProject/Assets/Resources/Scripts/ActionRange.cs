using UnityEngine;
using System.Collections;

public class ActionRange : MonoBehaviour {
	public static ActionRange AR;
	public Action action;
	//bool isCaluculated;

	void Awake(){
		AR = this;
	}

    public bool isAttackCaluculated;
	//攻撃範囲の計算 x,y = 原点 ac = 行動
	public IEnumerator AttackCost(int x, int y,Action ac){
		//計算の開始
		//isCaluculated = true;
        isAttackCaluculated = true;

		action = ac;

		//全チップを初期化
		MapMoveScript.MMS.MapChipFormat();

		//現在地の情報を確定
		MapCreateScript.mapChips [x, y].MoveCost = 0;//現在いるマスの移動コストは0
		MapCreateScript.mapChips[x,y].isMove = true;
		MapCreateScript.mapChips [x, y].isDone = true;
		MapCreateScript.mapChips [x, y].setMovedPos (x, y);

		int[] findPos = new int[2]{ x, y };
		int[] newPos = new int[2] { -1, -1 };
		int count = 0;
		bool isContinue;
		do {

			isContinue = false;
			int minCost = 1000;

			int[,] n = MapMoveScript.MMS.NeighbourChip(findPos[0],findPos[1]);


			//nowPosに隣接しているマスの移動コストを計算、既存のものより低ければ更新する
			for(int i = 0;i < n.GetLength(0);i++){
				if(MapCreateScript.mapChips[n[i,0],n[i,1]].isDone != true){
					int cost = /*MapCreateScript.mapChips[n[i,0],n[i,1]].cost +*/ MapCreateScript.mapChips[findPos[0],findPos[1]].MoveCost + 1;
					if(MapCreateScript.mapChips[n[i,0],n[i,1]].MoveCost > cost){
						MapCreateScript.mapChips[n[i,0],n[i,1]].MoveCost = cost;
						//行動範囲内なら範囲内の表示をする
						if(cost <= action.GetRange()){
							if(action.type == Actions.Attack/* || action.type == Actions.ApplyStatus*/){
								MapCreateScript.mapChips[n[i,0],n[i,1]].AttackObj.SetActive(true);
							}else if(action.type == Actions.ApplyStatus || action.type == Actions.Heal){
								MapCreateScript.mapChips[n[i,0],n[i,1]].HealObj.SetActive(true);
							}
						}
						MapCreateScript.mapChips[n[i,0],n[i,1]].isDone = false;
						MapCreateScript.mapChips[n[i,0],n[i,1]].setMovedPos(findPos[0],findPos[1]);
					}
				}
			}

			if(findPos[0] == 0 && findPos[1] == 1){
				string s = "";
				for(int i = 0;i < n.GetLength(0);i++){
					s += n[i,0] + ":" + n[i,1] + " $ ";
				}
			}


			//まだコストが確定していないコスト最小地点を探す
			for(int i = 0;i < MapCreateScript.mapChips.GetLength(0);i++){
				for(int j = 0;j < MapCreateScript.mapChips.GetLength(1);j++){
					if(MapCreateScript.mapChips[i,j].isDone == false && MapCreateScript.mapChips[i,j].MoveCost < minCost){
						minCost = MapCreateScript.mapChips[i,j].MoveCost;
						newPos[0] = i;
						newPos[1] = j;
						isContinue = true;
					}
				}
			}

			//コスト最小の地点への移動コストを確定させる
			if(newPos[0] >= 0){
				MapCreateScript.mapChips[newPos[0],newPos[1]].isDone = true;
				//Debug.Log(newPos[0] + ":" + newPos[1] + "->" + minCost);
			}
			findPos = newPos;

			count++;
			if(count % 10 == 0){
				//Debug.Log(count + ":" + minCost + "->" + newPos[0] + ":" + newPos[1]);
				yield return null;
			}
		} while(isContinue);//コストが更新され続ける限り続ける

		//isCaluculated = false;
        isAttackCaluculated = false;
	}
}
