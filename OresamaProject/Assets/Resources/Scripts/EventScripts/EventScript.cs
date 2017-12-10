using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum TargetType{
	my,//自分のみ
	friend,//仲間全員
	enemy,//敵全員
	all,//全員
}

public class EventScript : MonoBehaviour {
    public string eventName;
	public Events eventType;
	public GameObject sprite;
    public GameObject Effect;
    public Sprite EventChip;
    private const float EventSpriteTime = 1.5f;
	public bool isAOE;
	public bool isRundomTarget;
	public TargetType targetType;

	//このイベントが呼び出されたらフラグを立てる
	public bool isEvent;
	public bool isEventRuntime;

	public List<GameObject> targets;

	//イベントを発生させる
	public virtual void EventGenerat(List<GameObject> t){
        isEvent = true;

        string eventText = eventName;
        if (eventText == "") { eventText = "イベント"; }
        Dialog.Dlog.SetText(eventText + "が発動しました");
		Debug.Log ("イベントが実行されました");
	}

    public void EffectGenarat(Vector3 pos) {
        if (Effect != null) {
            Instantiate(Effect, pos, Quaternion.identity);
        }
    }

	//イベント画像の表示
	public IEnumerator EventSprite(){
        if (sprite != null)
        {
            sprite.SetActive(true);
            yield return new WaitForSeconds(EventSpriteTime);
            sprite.SetActive(false);
        }
	}

	//イベントを終了させる
	public void EventEnd(){
		isEventRuntime = false;
		isEvent = false;
		EventListScript.ELS.isEvent = false;
	}
}
