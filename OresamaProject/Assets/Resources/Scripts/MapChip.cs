using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public enum mapChipType{
	grass,
	water
}

public class MapChip : MonoBehaviour {
	public mapChipType type;//地形の種類
	public int[] ChipNum;//このマスの番号
	Vector2 pos;//このマスの場所
	public Vector2 getPos(){ return pos;}

    //UI用オブジェクト
	public GameObject WalkObj;//移動可能か否か
	public GameObject ArrowObj;//移動選択しているか否か
	public GameObject AttackObj;//攻撃できるか否か
	public GameObject HealObj;//補助できるか否か
	public GameObject EventObj;//イベントがあるか否か

	//イベント確認用
	public GameObject eventType;//イベントタイプ確認用
	public bool isEvent;//イベントがアクタィブか否か

    //移動コスト計算用
	//[HideInInspector]
    public int cost;//隣のマスからこのマスに移動するときのコスト
	public int MoveCost;//現在いるマスからこのマスに移動するコスト
	public bool? isDone;//移動コストの計算終了したか確認用
	public bool isMove;
	public int[] movedPos = new int[2];
    public int[] GetMovedPos() { return movedPos; }
	public void setMovedPos(int x, int y){
		movedPos [0] = x;
		movedPos [1] = y;
		//Debug.Log (gameObject.name + "<-" + x + ":" + y);
	}
    public GameObject RideCharacter;//このマスにいるキャラクター
    //public bool GetRideCharacter() { }
    //キャラクターをこのマスに移動 もし既にキャラクターがいたらfalseを返す
    public bool SetRideCharacter(GameObject c) {
        if (RideCharacter == null)
        {
            RideCharacter = c;
            return true;
        }
        else return false;
    }
    public void RemoveRideCharacter() { RideCharacter = null;}

	void Awake(){
        InitSinbols();
		Instance ();
		SpriteRenderer walk = WalkObj.GetComponent<SpriteRenderer> ();
		walk.sortingOrder = 1;
		Color c = walk.color;
		c.a = 0.5f;
		walk.color = c;
	}

    void InitSinbols() {
        GameObject simbols = Instantiate(Resources.Load("prefabs/mapChips/simbols")) as GameObject;
		simbols.transform.parent = transform;
		simbols.transform.localPosition = Vector3.zero;
        sinbolsScript SS = simbols.GetComponent<sinbolsScript>();
        WalkObj = SS.WalkObj;
        ArrowObj = SS.ArrowObj;
        AttackObj = SS.AttackObj;
        HealObj = SS.HealObj;
		EventObj = SS.EventObj;
    }

	//チップ内の情報を初期化
	public void Instance(){
		pos = this.transform.position;
		MoveCost = 1000;
		isDone = null;
		setMovedPos (-100, -100);
		isMove = false;
		ArrowObj.SetActive (false);
		WalkObj.SetActive (false);
		AttackObj.SetActive (false);
		HealObj.SetActive (false);
	}


	void Update(){
		//移動可能なら移動可能表示
        if (isMove && GameController.Gcon.GamePhase == Phase.Move)
			WalkObj.SetActive (true);
		else
			WalkObj.SetActive (false);
		if (isEvent) {
			EventObj.SetActive (true);
		} else
			EventObj.SetActive (false);
	}

	public void ChipTap(){
		//移動の入力
        if (GameController.Gcon.GamePhase == Phase.Move)
        {
			MapMoveScript.MMS.MapChipDragg (ChipNum [0], ChipNum [1]);
		}
	}

}
