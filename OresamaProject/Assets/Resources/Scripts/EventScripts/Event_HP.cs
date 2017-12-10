using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Event_HP : EventScript {
	public float Value;//回復量
	public bool isCoefficient;//回復量が割合か否か

	public override void EventGenerat(List<GameObject> t){
		base.EventGenerat (t);
		//Healが実行されました
		Debug.Log("HP操作イベント " + name +" が実行されました");
		//StartCoroutine (Heal ());
		targets = t;
        StartCoroutine(Heal());
		Debug.Log (name);
	}

	IEnumerator Heal(){
		isEventRuntime = true;

		//イベント画像の表示演出
        yield return StartCoroutine(EventSprite());

		//対象を回復する
		for(int i = 0;i < targets.Count;i++){
			//Debug.Log (targets [i]);
			Character c = targets [i].GetComponent<Character> ();
            EffectGenarat(targets[i].transform.position);

			//回復量の計算
			float v = Value;
			if (isCoefficient) {
				float mHP = c.GetMaxHp ();
				v = mHP * v;
			}
			int va = (int)v;
            c.TakeDamage(null, va, false, true);
		}

		EventEnd ();

        yield return new WaitForSeconds(1.0f);
	}
}
