using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class CharacterMove : MonoBehaviour,IPointerClickHandler {
	public int MovePower;
	private float MoveSpeed = 0.6f;

	//[HideInInspector]
	public int[] nowPos = new int[2];//現在のマップ上座標 [x, y]
	public void SetNowpos(int x,int y){
        MapCreateScript.mapChips[nowPos[0], nowPos[1]].RideCharacter = null;//元の位置から削除
		nowPos [0] = x;
		nowPos [1] = y;
		MapCreateScript.mapChips [x, y].RideCharacter = gameObject;
	}

	public void PosInit(){
		//Debug.Log (nowPos [0] + ":" + nowPos [1]);
		this.transform.position = MapCreateScript.mapChips [nowPos [0], nowPos [1]].getPos ();
	}

	public IEnumerator MoveTo(int[,] moveTo){
		MapMoveScript.MMS.isMove = true;
        //Debug.Log("Move started ==========================================================");
        GameObject charSprite = GetComponent<Character>().getCharSprite();
        animationController aniCon = charSprite.GetComponent<animationController>();

        int Length = moveTo.GetLength(0);
        bool MoveStop = false;
        //Debug.Log(Length);
        for (int i = 0; i < Length; i++) {
            aniCon.setWalking(true);

            Vector3 pos = MapCreateScript.mapChips[moveTo[i, 0], moveTo[i, 1]].getPos();

            iTween.MoveTo(gameObject, iTween.Hash(
                "position", pos,
                "time", MoveSpeed,
                "easetype", iTween.EaseType.linear
            ));

            //左右で方向転換
            if (transform.position.x < pos.x)
            {
                if (transform.localScale.x != -1)
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else if (transform.position.x > pos.x)
            {
                if (transform.localScale.x != 1)
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }

            yield return new WaitForSeconds(MoveSpeed);

            //イベント呼び出し
            if (MapCreateScript.mapChips[moveTo[i, 0], moveTo[i, 1]].isEvent)
            {
                aniCon.setWalking(false);
                EventListScript.ELS.AddEvent(MapCreateScript.mapChips[moveTo[i, 0], moveTo[i, 1]].gameObject);

                GameObject eventObj = MapCreateScript.mapChips[moveTo[i, 0], moveTo[i, 1]].eventType;
                EventScript ES = eventObj.GetComponent<EventScript>();
                if (ES != null && ES.eventType == Events.Warp) { MoveStop = true; }

                yield return StartCoroutine(EventListScript.ELS.EventListCall(gameObject));
            }

            //移動マーカーの消去
            bool isDestroy;
            isDestroy = true;
            for (int j = i + 1; j < moveTo.GetLength(0); j++)
            {
                //これからもう一度そこに移動するなら後で消去する
                if (moveTo[i, 0] == moveTo[j, 0] && moveTo[i, 1] == moveTo[j, 1])
                {
                    isDestroy = false;
                    break;
                }
            }
            if (isDestroy)
                MapCreateScript.mapChips[moveTo[i, 0], moveTo[i, 1]].ArrowObj.SetActive(false);

            //もしキャラクターが途中で倒れたら処理を中断する
            if (MoveStop) {
                Debug.Log("MoveStop");
                for (int j = 0; j < Length; j++) {
                    MapCreateScript.mapChips[moveTo[j, 0], moveTo[j, 1]].ArrowObj.SetActive(false);
                }
                break;
            }
        }
        
		MapMoveScript.MMS.isMove = false;
        charSprite.GetComponent<animationController>().setWalking(false);

        //Debug.Log("move is done ==========================================================");
    }

    public IEnumerator PathMove(int[,] path,float speed = 3.0f) {

        int Length = path.GetLength(0);
        Vector3[] pos = new Vector3[Length];

        for (int i = 0; i < Length; i++) {
            pos[i] = MapCreateScript.mapChips[path[i, 0], path[i, 1]].getPos();
        }

        float dis = 0;
        Vector2 aPos = transform.position;
        Vector2 bPos;
        for (int i = 0; i < Length; i++) {
            bPos = pos[i];
            dis += Vector2.Distance(aPos, bPos);
        }
        //速度設定
        dis = dis * 2 / 3;

            isMoving = true;
            Debug.Log("pathMove");
        if (pos.GetLength(0) > 1)
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "path", pos,
                "time", dis,
                "easetype", iTween.EaseType.linear
            ));
        }
        else
        {
            iTween.MoveTo(gameObject, iTween.Hash(
                "position", pos[0],
                "time", dis,
                "easetype", iTween.EaseType.linear,
                "oncomplete", "OnCompleteHandler"
            ));
        }

        
        while (isMoving) {
            if (isEnd) {
                isEnd = false;
                if (GetComponent<iTween>() != null) {
                    iTween.Stop(gameObject);
                }
                transform.position = pos[pos.GetLength(0) - 1];
                break;
            }
            yield return null;
        }
        isMoving = false;
        SetNowpos(path[Length - 1, 0], path[Length - 1, 1]);
    }


    //移動を強制終了させる（目的地点へ瞬間移動）
    private bool isEnd = false;
    public void MoveEnd() {
        if (isMoving) {
            isEnd = true;
        }
    }

    public bool isMoving = false;
    public bool GetIsMoving() { return isMoving; }

    void OnCompleteHandler(){
        isMoving = false;
        //Debug.Log("move end");
    }

	public void OnPointerClick(PointerEventData e){
		//アニメーション中なら反応しない
        if (GameController.Gcon.isanimation)
			return;

		//このキャラクターのステータスを表示する
		//

		//行動対象の選択中なら
        if (GameController.Gcon.isCharacterSelect)
        {
			//このキャラクターが対象として正しいか確認する
			//正しければ選択する
			CharacterSelect.CS.CharaSelect (gameObject);
		}
            /*
		//自身の所属する陣営の手番なら
        else if (tag == GameController.Gcon.GameSide.ToString())
        {
            if (GameController.Gcon.GamePhase == Phase.Menu)
            {
				CharacterSelect.CS.SetPlayer (gameObject);
			}
		} else {
			
		}*/
	}
}
