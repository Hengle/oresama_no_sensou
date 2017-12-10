using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameRoot : MonoBehaviour
{
	[SerializeField]
	private TextAsset mapData;

	public static GameRoot GRoot;

    public List<GameObject> playerList = new List<GameObject>();
    public List<GameObject> enemyList = new List<GameObject>();

	public List<GameObject> ActionQueue;

	// TODO: this needs to be changed
	public GameObject[] allEnemies;
	public GameObject[] allPlayers;

	void Awake()
	{
		//DontDestroyOnLoad(this);

		if (!GRoot)
			GRoot = this;
		else
			Destroy(this.gameObject);

	}

	void OnLevelWasLoaded(int level)
	{
		if (GRoot == this) { 
			// GRoot = this;
			RemoveUnwanted();
			if (level == 3)
				ActionQueue.Clear();

		}
		else
			Destroy(this.gameObject);

	}

	// Use this for initialization
	void Start ()
	{
		InitCharacters();
		turn_Start ();
	}

	public void RemoveUnwanted()
	{
		allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
		allPlayers = GameObject.FindGameObjectsWithTag("Player");

		foreach (GameObject go in allPlayers)
		{
			if (playerList.Contains(go))
			{
				//Debug.Log("Found " + go.GetInstanceID());
			}
			else
			{
				//Debug.Log("delete " + go.GetInstanceID());
				Destroy(go);
			}
		}

		foreach (GameObject go in allEnemies)
		{
			if (enemyList.Contains(go))
			{
				//Debug.Log("Found " + go.GetInstanceID());
			}
			else
			{
				//Debug.Log("delete " + go.GetInstanceID());
				Destroy(go);
			}
		}
	}

	private bool CharsInited = false;
	public void InitCharacters()
	{
		if (!CharsInited) {
            //0 prefab名
            //1 陣営
            //2 マスx
            //3 マスy
            //4 職業
            //5 名前
            //6 ID
			GameObject characters = new GameObject();
			characters.name = "characters";
			for (int i = 0; i < CSVReader.GetLength(mapData); i++)
			{ 

				string[] charData = CSVReader.ReadLine(mapData, i+1);
				//Debug.Log(charData[0]);
				string prefabPath = "prefabs/characters_map/" + charData[0];
				//Debug.Log(prefabPath);

				GameObject chara = (GameObject)Instantiate(Resources.Load(prefabPath), Vector3.zero, this.transform.rotation);
				chara.name = charData [0];
				// Debug.Log(charData[3]);
				GameObject go = chara;
				go.transform.parent = characters.transform;

				/*
				chara.GetComponent<playerMove>().NowPos = int.Parse(charData[3]);
				*/

				//GameObject square = GameObject.Find(i % 5 + ":" + i % 2);
				//chara.transform.position = square.transform.position/* + new Vector3(0.0f, 0.5f, 0.0f)*/;
				// chara.GetComponent<Character>().SetLevel(int.Parse(charData[5]));

				if (charData[1] == "1")
					playerList.Add(chara);
				else if (charData[1] == "2")
					enemyList.Add(chara);
				else
					Debug.Log("Error, is character player or enemy?");

				_characters.Add (chara);

				//Debug.Log("================");

				int x = int.Parse (charData [2]);
				int y = int.Parse (charData [3]);

                //侵入不可領域のキャラクターの配置位置をランダムに変更
				while (true) {
					if (MapCreateScript.mapChips [x, y] == null) {
						x = Random.Range (0, MapCreateScript.mapChips.GetLength (0));
						y = Random.Range (0, MapCreateScript.mapChips.GetLength (1));
					} else
						break;
				}
				CharacterMove CMove = chara.GetComponent<CharacterMove> ();
				if (CMove != null) {
					//Debug.Log (x + ";" + y);
					CMove.SetNowpos (x, y);
					CMove.PosInit ();
				}

                Character c = chara.GetComponent<Character>();
                if (charData[5] != "") {
                    c.SetName(charData[5]);
                }

                if (charData[6] != "") {
                    c.SetID(int.Parse(charData[6]));
                }
			}
			/*
			MapMoveScript.MMS.UpdateAllChars();
			*/
			CharsInited = true;
		}


	}


	//turnorder
	public List<GameObject> _characters = new List<GameObject>();
	public Queue<GameObject> _turnOrder = new Queue<GameObject>();
	public GameObject _activeChar;

	// Use this for initialization
	void turn_Start () {
		// NextTurn();
	}

	/*void OnGUI()
	{
		if (GUI.Button(new Rect(10, 10, 150, 100), "End Turn"))
		{
			NextTurn();
		}
	}*/

	public GameObject NextTurn() 
	{

		if (_turnOrder.Count > 0) { 
			_turnOrder.Dequeue();
		}

        Queue<GameObject> newTurnOrder = new Queue<GameObject>();
        foreach (GameObject t in _turnOrder)
        {
            if (t.GetComponent<Character>().GetHp() > 0)
                newTurnOrder.Enqueue(t);
        }
        _turnOrder = newTurnOrder;

		do
		{
			List<GameObject> cList = new List<GameObject>();

			foreach (GameObject c in _characters)
			{
                Character cc = c.GetComponent<Character>();
                if (cc.GetHp() <= 0) {
                    continue;
                }
                cc.step();
				cList.Add(c);
			}
			cList.Sort(delegate(GameObject a, GameObject b)
				{
					Character aa = a.GetComponent<Character>();
					Character bb = b.GetComponent<Character>();

					int ret = aa.turnTick.CompareTo(bb.turnTick);
					if (ret == 0)
						ret = aa.speed.CompareTo(bb.speed);
					if (ret == 0)
						ret = aa.name.CompareTo(bb.name);

					return ret;
				});

			cList.Reverse();
			_turnOrder.Enqueue(cList[0]);
			cList[0].GetComponent<Character>().calcNext();
		} while (_turnOrder.Count < 6);//表示数

		GetComponent<TurnOrder>().Init(_turnOrder);
        
		_activeChar = _turnOrder.Peek();

        if (_activeChar.GetComponent<Character>().GetHp() <= 0)
        {
            _turnOrder.Dequeue();
            return NextTurn();
            // skip
        }

		return _activeChar;
	}
}