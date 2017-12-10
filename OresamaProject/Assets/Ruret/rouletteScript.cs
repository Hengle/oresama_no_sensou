using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class rouletteScript : MonoBehaviour {
    public GameObject ban;
    public GameObject hari;

    public bool isStart;//スタートボタンが押されたか
    public bool isStop;//ストップボタンが押されたか
    public bool isMoveSpeedEnter = false;//移動力が決定したか

    public float defaltSpeed;//回転の初速度
    public float axelSpeed;//回転の加速度
    public float maxSpeed;//回転の最大速度
    public float brakeSpeed;//止まるときのフレームごとの速度減少量

	public float stopTime;
	private float _sTime;

    [SerializeField]
    private float nowSpeed = 0;//現在の回転速度

    public static rouletteScript RS;

    void Awake() {
        RS = this;
        gameObject.SetActive(false);
    }

    public void Initialize() {
        isMoveSpeedEnter = false;
    }

	public int debugMove;
    //スタートボタンを押した時に実行される
    public void StartButton()
    {
		List<int> move = new List<int> ();
		for (int i = 0; i < debugMove; i++) {
			move.Add (i + 1);
		}
        RouletStart(move);
    }

	public void RouletStart(List<int> moveSpeeds) {
		if (moveSpeeds.Count <= 0) return;
        ban.transform.eulerAngles = new Vector3(0, 0, 0);
        moveSpeedList = moveSpeeds;
        isMoveSpeedEnter = false;
        RouletteInit(moveSpeedList);
        gameObject.SetActive(true);
        isStart = true;
        nowSpeed = defaltSpeed;
        GameController.Gcon.SetGamePhase(Phase.Roulet);
        if (GameController.Gcon.GameSide == Side.Player) {
            Stop.SetActive(true);
        }
    }

    public GameObject Stop;
    //ストップボタンを押した時に実行される
    public void StopButton()
    {
        isStop = true;
        Stop.SetActive(false);
    }

    //ルーレットが止まった時に実行される関数
    private bool stopNow = false;
    public IEnumerator RuretStop()
    {
        if (stopNow) yield break;
		Debug.Log("Stop");
        stopNow = true;
        yield return new WaitForSeconds(1.0f);
        isStart = false;
        isStop = false;
        isMoveSpeedEnter = true;
        gameObject.SetActive(false);
        GameController.Gcon.SetGamePhase(Phase.Move);
        MovePowerWindow.MPW.SetMovePower(nowMoveSpeed);
        stopNow = false;
    }

    void Update()
    {
        //スタートボタンが押されていたら
        if (isStart)
        {
            //ストップボタンが押されていなければ
            if (!isStop)
            {
                //ルーレットを最大速度になるまで毎フレーム加速させる
                if (nowSpeed < maxSpeed)
                    nowSpeed += axelSpeed;
                if (nowSpeed > maxSpeed)
                    nowSpeed = maxSpeed;
            }
            //ストップボタンが押されていたら
            else
            {
                //ルーレットを速度が0になるまで毎フレーム減速させる
                if (nowSpeed > 0)
                    nowSpeed -= brakeSpeed;

                //ルーレットが停止したら
                if (nowSpeed <= 0)
                {
                    nowSpeed = 0;
                    //移動力の確認を行う
                    StartCoroutine(RuretStop());
                }
            }
        }

        if (isStart)
        {
            ban.transform.Rotate(new Vector3(0, 0, 1), -nowSpeed);
        }

        MoveNumCheck();
    }

    void MoveNumCheck() {
        //針の回転角計算
        hari.transform.LookAt(ban.transform.position);
        float plusAngle = hari.transform.eulerAngles.x + 90.0f;
        //回転が90に近ければ
        if (hari.transform.eulerAngles.y < 180 && hari.transform.eulerAngles.y > 0)
        {
            plusAngle *= -1;
        }
        hari.transform.eulerAngles = new Vector3(0, 0, plusAngle);

        //針と盤の角度計算
        float dx = ban.transform.localPosition.x - hari.transform.localPosition.x;
        float dy = ban.transform.localPosition.y - hari.transform.localPosition.y;
        float rad = Mathf.Atan2(dx, dy) * Mathf.Rad2Deg;
        rad += 180;//一つ目の項目の部分が０度になるように調整

        //盤の回転を計算
        rad += ban.transform.localEulerAngles.z;

        //角度がマイナスになっていたらプラスに修正
        if (rad < 0)
            rad += 360;
        else if (rad >= 360)
            rad -= 360;
        //項目ひとつぶんの角度を計算
        int oneColumnRot = 360 / moveSpeedList.Count;
        int newRad = Mathf.FloorToInt(rad);//３６０度にならないように調整

        nowMoveSpeed = moveSpeedList[newRad / oneColumnRot];
    }

    public List<int> moveSpeedList;
    public int nowMoveSpeed = 0;


	public GameObject splitLine;//分割線
	public Sprite[] numbers;//数字画像
    public GameObject numObj;//数字オブジェクト
	public Transform ZeroPos;
    public List<GameObject> Dests;
	void RouletteInit(List<int> moveList){
        Debug.Log("RIni");
        for (int i = 0; i < Dests.Count; i++) {
            Destroy(Dests[i]);
        }
        Dests.RemoveRange(0, Dests.Count);

		//分割線の角度を確認
		List<int> LineRots = CircleSplitRot (360, moveList.Count);
        
		//分割線の生成
		for (int i = 0; i < LineRots.Count; i++) {
			GameObject Line = Instantiate (Resources.Load ("prefabs/Roulette/" + splitLine.name))as GameObject;
            Dests.Add(Line);//廃棄リストに追加
            RectTransform l = Line.GetComponent<RectTransform>();
            Vector3 size = l.localScale;
            l.SetParent(ban.transform);
            l.localPosition = Vector2.zero;
            l.localScale = size;
			//Line.transform.position = ZeroPos.position;
			Line.transform.eulerAngles = new Vector3 (0, 0, -LineRots [i]);
			Line.name = "moveLine:" + i;
		}

		//移動力表示の生成
		LineRots.Add(360);
		for(int i = 0;i < moveList.Count;i++){
			//分割線の中間角度を確認
			float rot = (LineRots [i] + LineRots [i + 1]) / 2;
			GameObject moveNum = Instantiate (Resources.Load ("prefabs/Roulette/" + numObj.name))as GameObject;
            Dests.Add(moveNum);
            RectTransform r = moveNum.GetComponent<RectTransform>();
            Vector3 size = r.localScale;
            r.SetParent(ban.transform);
            r.localPosition = Vector2.zero;
            r.eulerAngles = new Vector3(0, 0, -rot);
			//r.eulerAngles = new Vector3(0, 0, rot);
            r.localScale = size;
			//画像の変更
            moveNum = moveNum.transform.Find("num").gameObject;
			moveNum.GetComponent<Image> ().sprite = numbers [moveList [i]];
            r.rect.Set(r.rect.x, r.rect.y, numbers[i].rect.width, numbers[i].rect.height);
            
		}
	}

	//円を分割した際の分割線の角度計算 max = 最大（基本は360）・　c = 分割数
	List<int> CircleSplitRot(int max,int c){
		//分割数が1以下なら分割無し
		if (c <= 1) return null;

		//最低値を計算
		int n = max / c;
		//線と同じ数（分割数　＋　1）の配列を確保
		List<int> list = new List<int>();

		//それぞれの角度を計算
		for (int i = 0; i <= c; i++) {
			list.Add (n * i);
		}

		return list;
	}

	/*
	 * max = 360,c = 6
	 * 
	 * n = 630 / 6 = 60;
	 * 
	 * list{
	 * 0,
	 * 60,
	 * 120,
	 * 180,
	 * 240,
	 * 300
	 * }
	*/
}
