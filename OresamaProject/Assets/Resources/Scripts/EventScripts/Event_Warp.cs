using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Event_Warp : EventScript {
	public bool isRandom;//移動先がランダムか否か
	public int MoveRange;//移動可能なマス数

	public override void EventGenerat(List<GameObject> t){
		base.EventGenerat (t);
		//Healが実行されました
		Debug.Log("移動イベントが実行されました");
		targets = t;
        StartCoroutine(Warp());
		Debug.Log (name);
	}

	IEnumerator Warp(){
		isEventRuntime = true;

		//イベント画像の表示演出
        yield return StartCoroutine(EventSprite());

		for (int i = 0; i < targets.Count; i++) {
            EffectGenarat(targets[i].transform.position);

			int[] pos = new int[2];
			if (isRandom) {
                while (true) {
                    pos[0] = Random.Range(0, MapCreateScript.mapChips.GetLength(0));
                    pos[1] = Random.Range(0, MapCreateScript.mapChips.GetLength(1));
                    if (MapCreateScript.mapChips[pos[0], pos[1]].cost < 10) { break; }
                }
			} else {
				while(pos == null){
					//移動先を選択するまで待機
					//yield return null;
				}
			}

            if (Effect != null)
                yield return new WaitForSeconds(1.0f);

			targets [i].GetComponent<CharacterMove> ().SetNowpos (pos [0], pos [1]);
			targets [i].transform.position = MapCreateScript.mapChips [pos [0], pos [1]].getPos ();
            EffectGenarat(targets[i].transform.position);
			Debug.Log (pos [0] + ":" + pos [1]);

            Dialog.Dlog.SetText(targets[i].GetComponent<Character>().GetName() + "が移動しました");
		}

		EventEnd ();
	}
}
