using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Events{
	Heal,
	Judgment,
	Warp,
}

public class EventListScript : MonoBehaviour {
	public static EventListScript ELS;

	public GameObject[] EventList;
	public List<GameObject> EventQueue;
	public void AddEvent(GameObject e){ EventQueue.Add (e); }
	public void RemoveEvent(){ EventQueue.RemoveAt (0);}
	public List<GameObject> GetEvent(){ return EventQueue;}

    public GameObject EventEffect;
    public Vector3 EffectOffset;

	public List<GameObject> t;

	void Awake(){
		ELS = this;
	}
	/*
	public void heal(){
		EventCall (Events.Heal, t);
	}
	public void judgment(){
		EventCall (Events.Judgment, t);
	}
	public void warp(){
		EventCall (Events.Warp, t);
	}
	*/
	void Start(){
		for (int i = 0; i < EventList.GetLength (0); i++) {
			//Debug.Log (EventList [i].eventType);
		}
	}

	public bool isEvent;
	public IEnumerator EventListCall(GameObject eChara){
        //Effectの生成
        if (EventEffect != null) {
            Instantiate(EventEffect, eChara.transform.position + EffectOffset, Quaternion.identity);
            yield return new WaitForSeconds(0.5f);
        }

		while(EventQueue.Count > 0){
			yield return null;
			if (isEvent) continue;
			EventScript es = EventQueue [0].GetComponent<MapChip> ().eventType.GetComponent<EventScript> ();
			List<GameObject> targets = new List<GameObject> ();
			switch (es.targetType) {
			case TargetType.my:
				targets.Add (eChara);
				break;
			case TargetType.friend:
				if (eChara.tag == "Player")
					targets = GameRoot.GRoot.playerList;
				else
					targets = GameRoot.GRoot.enemyList;
				break;
			case TargetType.enemy:
				if (eChara.tag != "Player")
					for (int i = 0; i < GameRoot.GRoot.playerList.Count; i++) {
						targets.Add (GameRoot.GRoot.playerList [i]);
					}
				else
					for (int i = 0; i < GameRoot.GRoot.playerList.Count; i++) {
						targets.Add (GameRoot.GRoot.playerList [i]);
					}
				break;
			case TargetType.all:
				for (int i = 0; i < GameRoot.GRoot.playerList.Count; i++) {
					targets.Add (GameRoot.GRoot.playerList [i]);
				}
				for (int i = 0; i < GameRoot.GRoot.enemyList.Count; i++) {
					targets.Add (GameRoot.GRoot.enemyList [i]);
				}
				break;
			default:
				break;
			}

            yield return StartCoroutine(EventCall(es.eventType, targets));
			EventQueue [0].GetComponent<MapChip> ().isEvent = false;
			RemoveEvent ();
			yield return new WaitForSeconds (1.0f);
		}
	}

	public IEnumerator EventCall(Events e,List<GameObject> targets){
		for (int i = 0; i < EventList.GetLength (0); i++) {
			if (EventList [i].GetComponent<EventScript>().eventType == e) {
                EventScript ES = EventList[i].GetComponent<EventScript>();
                ES.EventGenerat(targets);
                //イベント終了まで待機
                while (ES.isEvent) {
                    yield return null;
                }
			}
		}
	}
}
