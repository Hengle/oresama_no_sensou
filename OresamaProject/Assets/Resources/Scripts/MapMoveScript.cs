using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapMoveScript : MonoBehaviour {
	public static MapMoveScript MMS;

	public GameObject player;
	public CharacterMove CMove;
	public IEnumerator SetPlayer(GameObject newPLayer,bool isAI = false){
		player = newPLayer;
		CMove = player.GetComponent<CharacterMove> ();
		yield return StartCoroutine(MoveStart(isAI));
	}
	//int MovePower;//プレイヤーの移動力
	int selectMove = 0;//使用済みの移動力
    public int GetSelectMove() { return selectMove; }

	int[] nowPos = new int[2];//現在選択しているマス
	public bool isCaluculated;//計算中か否か
	[HideInInspector]
	public bool isMove = false;//移動中か否か
	[HideInInspector]
	public bool isMoveSelect = true;//移動経路選択中かどうか

	List<int[]> selectPos = new List<int[]> ();

	[HideInInspector]
	public bool isMoved = false;//選択したキャラクターが既に移動したかどうか

	void Start(){
		MMS = this;
		//SetPlayer (player);
	}

	//移動するマスの選択
	public bool MapChipDragg(int x, int y){
		//使用する変数が存在するか確認
		if (player == null || isCaluculated || isMove || isMoved){
			return false;
		}

		if (MapCreateScript.mapChips [x, y].isMove == false)
			return false;

		//もう移動力を使い切っていたら終了
		if (selectMove + MapCreateScript.mapChips [x, y].cost > CMove.MovePower) {
			return false;
		}

		//移動元地点の確認
		int[,] n ;//NeighbourChip (nowPos[0],nowPos[1]);
        int num = selectPos.Count - 1;
		if (selectPos.Count > 0) {//途中なら指定している最終地点から
			n = NeighbourChip (selectPos [num] [0], selectPos [num] [1]);
		}
		//最初なら現在地点
		else n = NeighbourChip (nowPos [0], nowPos [1]);

		//移動元地点から移動できるか確認
		int[] pos = new int[2]{ x, y };
		for (int i = 0; i < n.GetLength (0); i++) {
			if (n [i, 0] == pos [0] && n [i, 1] == pos [1] && MapCreateScript.mapChips [x, y].MoveCost <= CMove.MovePower) {
				selectPos.Add (pos);
				//MapCreateScript.mapChips [pos [0], pos [1]].setMovedPos (nowPos [0], nowPos [1]);
				MapCreateScript.mapChips [pos [0], pos [1]].ArrowObj.SetActive (true);
				nowPos = pos;
				selectMove += MapCreateScript.mapChips [x, y].cost;
                MovePowerWindow.MPW.SetTarget(MapCreateScript.mapChips[x, y].gameObject);
                return true;
			}
		}

        //存在しなかった場合
        List<int[]> moved = new List<int[]>();
        int[] startPos = new int[2];
        if (selectPos.Count > 0) {
            startPos[0] = selectPos[selectPos.Count - 1][0];
            startPos[1] = selectPos[selectPos.Count - 1][1];
        }else {
            startPos[0] = nowPos[0];
            startPos[1] = nowPos[1];
        }

        moved.Add(new int[2] { x, y });
        int c = 0;
        while(moved[c][0] != startPos[0] || moved[c][1] != startPos[1]){
            int posx = moved[c][0];
            int posy = moved[c][1];
            moved.Add(MapCreateScript.mapChips[posx, posy].movedPos);
            c++;
            if (c > 10 || pos[0] < 0) {
                Debug.Log("noneMove");
                return false;
            } 
        }
        moved.Reverse();
        moved.RemoveAt(0);
        foreach (int[] p in moved) {
            selectPos.Add(p);
            //MapCreateScript.mapChips[p[0], p[1]].setMovedPos(nowPos[0], nowPos[1]);
            MapCreateScript.mapChips[p[0], p[1]].ArrowObj.SetActive(true);
            nowPos = p;
            selectMove += MapCreateScript.mapChips[x, y].cost;
            MovePowerWindow.MPW.SetTarget(MapCreateScript.mapChips[x, y].gameObject);
            Debug.Log(p[0] + ":" + p[1]);
        }
        return true;
	}

	//移動先選択の開始
	IEnumerator MoveStart(bool isAI = false){
		if (player == null || CMove == null)
			yield break;

		Debug.Log ("移動開始");

		GameController.Gcon.SetGamePhase (Phase.Move);
		MoveCancel ();

        MapChipFormat();

		//ルーレットを回して移動力決定
        Character c = CMove.GetComponent<Character>();
        Debug.Log(c.GetMaxMove() + "から" + c.GetMinMove());
		rouletteScript.RS.RouletStart (c.GetMove ());
        do {//移動力が決定するまで待機
            yield return null;
        } while (!rouletteScript.RS.isMoveSpeedEnter);
        rouletteScript.RS.Initialize();
        
        CMove.MovePower = rouletteScript.RS.nowMoveSpeed;

		//CMove.SetNowpos (0, 0);
		CMove.gameObject.transform.position = MapCreateScript.mapChips [CMove.nowPos [0], CMove.nowPos [1]].getPos ();
		StartCoroutine (MoveCost (CMove.nowPos [0], CMove.nowPos [1],isAI));
		Debug.Log ("MoveStart");
	}

    public bool isMoveEnd = false;
	public IEnumerator MoveEnd(){
		if (isMove) {
			yield break;
		}

        isMoveEnd = true;
        CMove.SetNowpos(nowPos[0], nowPos[1]);

		if (selectPos.Count >= 0) {
			//移動パスを配列に変更
			int[,] moveTo = new int[selectPos.Count,2];
			for (int i = 0; i < selectPos.Count; i++) {
				moveTo [i, 0] = selectPos [i] [0];
				moveTo [i, 1] = selectPos [i] [1];
			}
			//移動していたら移動関数呼び出し
			if (moveTo.GetLength (0) > 0) {
				yield return (StartCoroutine (CMove.MoveTo (moveTo)));
			}
			selectPos.Clear ();
			selectMove = 0;
		}
        GameController.Gcon.SetGamePhase(Phase.MenuAtMoveEnd);
        yield return StartCoroutine(MoveCost(nowPos[0], nowPos[1]));

		isMoved = false;

        isMoveEnd = false;
	}

	public void MoveCancel(){
		if (isMove)
			return;
		
		int[] pos = CMove.nowPos;
		player.transform.position = MapCreateScript.mapChips [pos [0], pos [1]].getPos ();
		isMoved = false;
		for (int i = 0; i < selectPos.Count; i++) {
			MapCreateScript.mapChips [selectPos [i] [0], selectPos [i] [1]].ArrowObj.SetActive (false);
		}
		nowPos = CMove.nowPos;
		selectPos.Clear ();
		selectMove = 0;
        MovePowerWindow.MPW.SetCharacter(player);
        MovePowerWindow.MPW.SetTarget(null);
	}

	//移動コストの計算
	IEnumerator MoveCost(int x, int y,bool isAI = false){
		//移動が終わるまで待機
		while(isMove){
			yield return null;
		}

		//計算の開始
		isCaluculated = true;

		//全チップを初期化
		MapChipFormat ();

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

			//現在地に隣接しているマスを検索
			int[,] n = NeighbourChip(findPos[0],findPos[1]);

			//nowPosに隣接しているマスの移動コストを計算、既存のものより低ければ更新する
			for(int i = 0;i < n.GetLength(0);i++){
				int cost = MapCreateScript.mapChips[n[i,0],n[i,1]].cost + MapCreateScript.mapChips[findPos[0],findPos[1]].MoveCost;
				if(MapCreateScript.mapChips[n[i,0],n[i,1]].isDone != true){
					if(MapCreateScript.mapChips[n[i,0],n[i,1]].MoveCost > cost){
						MapCreateScript.mapChips[n[i,0],n[i,1]].MoveCost = cost;
                        if (cost <= CMove.MovePower && MapCreateScript.mapChips[n[i, 0], n[i, 1]].RideCharacter == null) MapCreateScript.mapChips[n[i, 0], n[i, 1]].isMove = true;
                        else {
                            MapCreateScript.mapChips[n[i, 0], n[i, 1]].MoveCost = 1000;
                            MapCreateScript.mapChips[n[i, 0], n[i, 1]].isDone = true;
                            MapCreateScript.mapChips[n[i, 0], n[i, 1]].isMove = false;
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

			if(count++ % 10 == 0){ yield return null; }
		} while(isContinue);//コストが更新され続ける限り続ける

		isCaluculated = false;
	}

	//隣のマスの確認
	public int[,] NeighbourChip(int x, int y){
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
	public void MapChipFormat(){
		for (int i = 0; i < MapCreateScript.mapChips.GetLength (0); i++) {
			for (int j = 0; j < MapCreateScript.mapChips.GetLength (1); j++) {
				if (MapCreateScript.mapChips [i, j] != null)
					MapCreateScript.mapChips [i, j].Instance ();
			}
		}
	}


}
