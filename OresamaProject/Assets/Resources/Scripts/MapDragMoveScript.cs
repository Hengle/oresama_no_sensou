using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public class MapDragMoveScript : MonoBehaviour {

	public static MapDragMoveScript MMDS;
	public GameObject player;

	int[] nowPos = new int[2];//現在選択しているマス
	bool isCaluculated;//計算中か否か
	[HideInInspector]
	public bool isMove;//移動中か否か
	[HideInInspector]
	public bool isMoveSelect = true;//移動経路選択中かどうか

	List<int[]> selectPos = new List<int[]> ();
	int selectMove = 0;

	[HideInInspector]
	public bool isMoved = false;//選択したキャラクターが既に移動したかどうか

	void Start(){
		MMDS = this;
		CharacterMove CMove = player.GetComponent<CharacterMove> ();
		CMove.SetNowpos (0, 0);
		CMove.gameObject.transform.position = MapCreateScript.mapChips [0, 0].getPos ();
		StartCoroutine (MoveCost (CMove.nowPos [0], CMove.nowPos [1]));
	}

	public void MapChipTap(int x, int y){
		if (player == null || isCaluculated || isMove || isMoved)
			return;

		CharacterMove CMove = player.GetComponent<CharacterMove> ();

		if (CMove.MovePower >= MapCreateScript.mapChips [x, y].MoveCost) {
			StartCoroutine (CMove.MoveTo (x,y));
			nowPos [0] = x;
			nowPos [1] = y;
			isMoved = true;
		}
		else {
			Debug.Log ("noneMve" + CMove.MovePower + ":" + MapCreateScript.mapChips [x, y].MoveCost);
		}
	}

	public void MapChipDragg(int x, int y){
		if (player == null || isCaluculated || isMove || isMoved)
			return;

		CharacterMove CMove = player.GetComponent<CharacterMove> ();
		if (selectMove + MapCreateScript.mapChips [x, y].cost > CMove.MovePower)
			return;

		int[,] n ;//NeighbourChip (nowPos[0],nowPos[1]);
		if (selectPos.Count > 0) {
			int num = selectPos.Count - 1;
			n = NeighbourChip (selectPos [num] [0], selectPos [num] [1]);
		}
		else n = NeighbourChip (nowPos [0], nowPos [1]);

		int[] pos = new int[2]{ x, y };
		for (int i = 0; i < n.GetLength (0); i++) {
			if (n [i, 0] == pos [0] && n [i, 1] == pos [1]) {
				selectPos.Add (pos);
				MapCreateScript.mapChips [pos [0], pos [1]].setMovedPos (nowPos [0], nowPos [1]);
				MapCreateScript.mapChips [pos [0], pos [1]].ArrowObj.SetActive (true);
				nowPos = pos;
				selectMove += MapCreateScript.mapChips [x, y].cost;
				break;
			}
		}
	}

	public void MoveEnd(){
		if (isMove)
			return;

		CharacterMove CMove = player.GetComponent<CharacterMove> ();

		if (selectPos.Count >= 0) {
			StartCoroutine (CMove.MoveTo (nowPos [0], nowPos [1]));
			for (int i = 0; i < selectPos.Count; i++) {
				MapCreateScript.mapChips [selectPos [i] [0], selectPos [i] [1]].ArrowObj.SetActive (false);
			}
			selectPos.Clear ();
			selectMove = 0;
		}

		CMove.SetNowpos (nowPos [0], nowPos [1]);
		StartCoroutine (MoveCost (nowPos [0], nowPos [1]));

		isMoved = false;
	}

	public void MoveCancel(){
		if (isMove)
			return;
		CharacterMove CMove = player.GetComponent<CharacterMove> ();
		int[] pos = CMove.nowPos;
		player.transform.position = MapCreateScript.mapChips [pos [0], pos [1]].getPos ();
		isMoved = false;
	}

	//移動コストの計算
	IEnumerator MoveCost(int x, int y){
		isCaluculated = true;
		MapChipFormat ();//計算前に初期化
		MapCreateScript.mapChips [x, y].MoveCost = 0;//現在いるマスの移動コストは0
		MapCreateScript.mapChips [x, y].isDone = true;
		MapCreateScript.mapChips [x, y].setMovedPos (x, y);

		int[] findPos = new int[2]{ x, y };
		int[] newPos = new int[2] { -1, -1 };
		int count = 0;
		bool isContinue;
		do {

			isContinue = false;
			int minCost = 1000;

			int[,] n = NeighbourChip(findPos[0],findPos[1]);


			//nowPosに隣接しているマスの移動コストを計算、既存のものより低ければ更新する
			for(int i = 0;i < n.GetLength(0);i++){
				if(MapCreateScript.mapChips[n[i,0],n[i,1]].isDone != true){
					int cost = MapCreateScript.mapChips[n[i,0],n[i,1]].cost + MapCreateScript.mapChips[findPos[0],findPos[1]].MoveCost;
					if(MapCreateScript.mapChips[n[i,0],n[i,1]].MoveCost > cost){
						MapCreateScript.mapChips[n[i,0],n[i,1]].MoveCost = cost;
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
		isCaluculated = false;
	}

	//隣のマスの確認
	int[,] NeighbourChip(int x, int y){//x,yが逆になってる？不具合発生中です。返り値がおかしい場合は逆にしてみてください
		int[,] n = new int[6, 2];//六角形マップなので最大で6、ｘとｙで2
		int count = 0;//実際にいくつ隣接しているか確認

		bool oddNum = false;
		if(y % 2 == 1){
			oddNum = true;
		}

		for (int i = -1; i < 2; i++) {//y方向計算
			int start = -1;//計算開始位置
			int max = 2;//計算終了位置
			if (i == 0) {
				start = -1;
			} else if (oddNum) {
				start = 0;
			} else {
				max = 1;
			}
			while(start < max){
				if (i == 0 && start == 0) {
					start++;
				}
				//指定された場所が存在すればその場所を記録
				//Debug.Log(x + start+":"+(y+i));
				if (y + i >= 0 && y + i < MapCreateScript.mapChips.GetLength(1) && x + start >= 0 && x + start < MapCreateScript.mapChips.GetLength(0)) {
					n [count, 0] = x + start;
					n [count++, 1] = y + i;
				}
				start++;
			}
		}
		int[,] N = new int[count, 2];

		for (int i = 0; i < count; i++) {
			N [i, 0] = n [i, 0];
			N [i, 1] = n [i, 1];
		}

		return N;
	}

	//全チップの初期化処理
	void MapChipFormat(){
		for (int i = 0; i < MapCreateScript.mapChips.GetLength (0); i++) {
			for (int j = 0; j < MapCreateScript.mapChips.GetLength (1); j++) {
				MapCreateScript.mapChips [i, j].Instance ();
			}
		}
	}


}
*/