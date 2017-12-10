using UnityEngine;
using System.Collections;


public class MapCreateScript : MonoBehaviour {
	//public GameObject[] mapChips;
	public string mapChip;//mapChipへのパス
	public string objectsPath;//objectsへのパス

	public Vector2 mapChipSize;

	public Layer2D mapDate;//マップ
	public Layer2D eventDate = null;//イベント
	public Layer2D objectDate;//配置物

	public static MapChip[,] mapChips;
    public Sprite[] mapChipSprites;

	//public string path;
	public TextAsset mapPath;

    public static MapCreateScript MCS;

	// Use this for initialization
	void Awake () {
        MCS = this;

		//マップ関連のデータ読み込み
		Layer2D[] Date = MapDateReader.AllRead (mapPath);
		mapDate = Date[0];
		objectDate = Date [1];
        eventDate = Date[2];

		MapInit ();
		ObjectsInit ();
        EventInit();
	}

	//Mapの生成　先にmapDateを定義しておくこと
	void MapInit(){
		mapChips = new MapChip[mapDate._vals.GetLength (1), mapDate._vals.GetLength (0)];

		GameObject obj = new GameObject("Map");
		//obj.name = "Map";
		int Length1 = mapDate._vals.GetLength (1);
		int Length2 = mapDate._vals.GetLength (0);
		for(int i = 0;i < Length1;i++){
			//GameObject oya = new GameObject (i.ToString ());
			for (int j = 0; j < Length2; j++) {
				/*GameObject Chip = new GameObject (j + ":" + i);
				Chip.transform.parent = oya.transform;
				Chip.transform.position = new Vector2 (mapChipSize.x * j, mapChipSize.y * i);*/
				Vector2 chipPos = new Vector2 (mapChipSize.x * i, mapChipSize.y * j);
				if (j % 2 == 1)
					chipPos.x += mapChipSize.x / 2;
				//Chip.transform.position = chipPos;
				if(mapDate._vals[j, i] < 0){

					continue;
				}
				GameObject Chip = Instantiate (Resources.Load (mapChip + mapDate._vals [j, i]), chipPos, Quaternion.identity)as GameObject;

				Chip.transform.parent = obj.transform;
				Chip.name = i + ":" + j;
				mapChips [i, j] = Chip.GetComponent<MapChip> ();
				mapChips [i, j].ChipNum = new int[2]{ i, j };

				if (!mapDate.walk [j, i]) {
					mapChips [i, j].cost = 1000;
					//Debug.Log ("haikei" + i + ":" + j);
				} else {
					//Debug.Log ("canWalk");
				}

				//イベントの決定
                if (eventDate == null) {
                    if (EventListScript.ELS != null && Random.Range(0, 10) < 1)
                    {
                        mapChips[i, j].isEvent = true;
                        int num = Random.Range(0, EventListScript.ELS.EventList.GetLength(0));
                        mapChips[i, j].eventType = EventListScript.ELS.EventList[num];
                        mapChips[i, j].EventObj.GetComponent<SpriteRenderer>().sprite = mapChips[i, j].eventType.GetComponent<EventScript>().EventChip;
                        mapChips[i, j].EventObj.GetComponent<SpriteRenderer>().color = Color.white;
                        mapChips[i, j].EventObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    }
                    else if (EventListScript.ELS == null)
                        Debug.Log("noneaddEvent");
                }
			}
		}
	}

	//objectの生成　先にobjectDateとMapChipsを定義しておくこと
	void ObjectsInit(){
		int Length1 = objectDate._vals.GetLength (1);
		int Length2 = objectDate._vals.GetLength (0);

		for(int i = 0;i < Length1;i++){
			for (int j = 0; j < Length2; j++) {

				//対象が無ければ次のループへ
				if (objectDate._vals [j, i] < 0)
					continue;

				//Debug.Log (objectDate._vals [j, i]);
				Vector2 chipPos = new Vector2 (mapChipSize.x * i, mapChipSize.y * j);
				if (j % 2 == 1)
					chipPos.x += mapChipSize.x / 2;
				
				GameObject Obj = Instantiate (Resources.Load (objectsPath + (objectDate._vals [j, i])), chipPos, Quaternion.identity)as GameObject;
				//Debug.Log (Obj.name);
				if (mapChips [i, j] != null)
					Obj.transform.SetParent (mapChips [i, j].transform);
			}
		}
	}

    void EventInit() {
        int Length1 = eventDate._vals.GetLength(0);
        int Length2 = eventDate._vals.GetLength(1);

        Debug.Log(Length1 + ":" + Length2);

        for (int i = 0; i < Length1; i++)
        {
            for (int j = 0; j < Length2; j++)
            {
                int num = eventDate._vals[j, i];
                //対象が無ければ次へ
                if (num < 0)
                    continue;

                //Debug.Log(num);
                mapChips[i, j].eventType = EventListScript.ELS.EventList[num];
                mapChips[i, j].EventObj.GetComponent<SpriteRenderer>().sprite = mapChips[i, j].eventType.GetComponent<EventScript>().EventChip;
                mapChips[i, j].EventObj.GetComponent<SpriteRenderer>().color = Color.white;
                mapChips[i, j].EventObj.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                mapChips[i, j].isEvent = true;
            }
        }
    }
}
