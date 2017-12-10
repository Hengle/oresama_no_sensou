using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;



public class BattleSceneControl : MonoBehaviour {

	public static BattleSceneControl BSC;
	private GameRoot GR;

	// private List<PlayerChar> players = new List<PlayerChar>();
	//private List<EnemyChar> enemies = new List<EnemyChar>();

	private List<GameObject> attackers = new List<GameObject>();
	private List<GameObject> opponents = new List<GameObject>();

	[SerializeField]
	private List<GameObject> leftUI;
	[SerializeField]
	private List<GameObject> rightUI;

    public List<GameObject> queue = new List<GameObject>();

	public Vector3 playerPosition = new Vector3(-5, 2, 0);

	public Vector3 enemyPosition = new Vector3(5, 2, 0);
	public Vector3 enemyHpbarPosition = new Vector3(1, 4, 0);
	public Vector3 playerHpbarPosition = new Vector3(-5.2f, 4, 0);
	public float characterOffset = 1;

	private GameObject background = null;

	// Debug parameters, these need to be changed later
	public int numberOfPlayers = 1;
	public int numberOfEnemies = 1;


	void Awake() 
	{
		BSC = this;
	}


	// Use this for initialization
	void Start () 
	{
		//BattleStart();
	}

	//戦闘開始
	public void BattleStart(){
		//GameRootのActionQueueを確認
		GR = GameRoot.GRoot;
		queue = GR.ActionQueue;


		// Debugging only
		// Populating players/enemies should be done through map
		background = (GameObject)Instantiate(Resources.Load("prefabs/Backgrounds/bg2"), new Vector3(-23,0,0), this.transform.rotation);
		objs.Add (background);
		/*
		AudioClip bgm = (AudioClip) Resources.Load("sound/music/bgm11");
		iTween.Stab(this.gameObject, bgm, 0.0f);
		*/

		//GetPlayers();
		//GetEmemies();
		SlideIn();
	}

	private void SlideIn()
	{
		float time = 0.1f;
		iTween.MoveTo(background, iTween.Hash("position", new Vector3(0, 0, 0), "time", time));

		int ai = 0;
		foreach (GameObject a in attackers)
		{
			GameObject bar = a.GetComponent<Character>().hpBar;
			bar.GetComponent<BattleSceneUI>().ShowUI();
			//iTween.MoveTo(bar, iTween.Hash("position", playerHpbarPosition - new Vector3(0, 05.5f * ai+1, 0), "time", 0.3f, "delay", time));
			ai++;
		}

		int oi = 0;
		foreach (GameObject o in opponents)
		{
			GameObject bar = o.GetComponent<Character>().hpBar;
			bar.GetComponent<BattleSceneUI>().ShowUI();
			//iTween.MoveTo(bar, iTween.Hash("position", enemyHpbarPosition - new Vector3(0, 0.5f * oi, 0), "time", 0.3f, "delay", time));
			oi++;
		}

		Invoke("RunQueue", time + 0.2f);

	}

	private void SlideOut()
	{
		float time = 1.0f;
		iTween.MoveTo(background, iTween.Hash("position", new Vector3(23, 0, 0), "time", time, "easetype", iTween.EaseType.easeInQuad));

		int ai = 0;
		foreach (GameObject a in attackers)
		{
			GameObject bar = a.GetComponent<Character>().hpBar;
			bar.GetComponent<BattleSceneUI>().HideUI();
			//iTween.MoveTo(bar, iTween.Hash("position", playerHpbarPosition + new Vector3(0, 1.5f, 0), "time", 0.5f, "easetype", iTween.EaseType.easeInQuad));
			ai++;
		}

		int oi = 0;
		foreach (GameObject o in opponents)
		{
			GameObject bar = o.GetComponent<Character>().hpBar;
			bar.GetComponent<BattleSceneUI>().HideUI();
			// iTween.MoveTo(bar, iTween.Hash("position", enemyHpbarPosition + new Vector3(0, 1.5f, 0), "time", 0.5f, "easetype", iTween.EaseType.easeInQuad));
			oi++;
		}

		Invoke("ReturnToMap", time);
        Debug.Log("SlideOut");

	}

	public List<GameObject> objs = new List<GameObject>();

	private void ReturnToMap()
	{
		for (int i = 0; i < objs.Count; i++) {
			Destroy (objs [0]);
			objs.RemoveAt (0);
		}
		Debug.Log ("ReturnToMap");
		GameController.Gcon.TarnEnd();
		//SceneManager.LoadScene(3);
	}

	public List<Character> casters;
	// Change to get players from map scene, remove PlayerChar class
	private void GetPlayers() 
	{
		/*
        for (int i = 0; i < numberOfPlayers; i++)
        {
            players.Add(new PlayerChar("swordsman"));
        }

        InitPlayers();
        */
		casters.RemoveRange (0, casters.Count);
		foreach (GameObject a in queue)
		{
			Character caster = a.GetComponent<Action>().GetCaster();
			if (!casters.Contains(caster))
			{
				casters.Add(caster);
			}
		}
		InitPlayers();
	}


	private void InitPlayers() 
	{
		for (int i = 0; i < casters.Count; i++)
		{
			Vector3 spawnOffset = new Vector3(characterOffset * i, 0.3f * i, characterOffset * i);
			Vector3 spawnPos = playerPosition - spawnOffset;

			string loadstring = "prefabs/characters/" + casters[i].GetPrefab().name;
			Debug.Log (loadstring);
			GameObject playerChar = (GameObject)Instantiate(Resources.Load(loadstring), spawnPos, this.transform.rotation);


			objs.Add (playerChar);
			playerChar.name = "player" + i;

			if (leftUI.Count <= i) {
				/*
				Debug.Log ("playerのUIの数が足りていません");
				//debug用に最後のオブジェクトをコピー
				GameObject newUI = (GameObject)Instantiate(leftUI[leftUI.Count - 1]);
				leftUI.Add (newUI);
				*/
			}

			GameObject hpbar = leftUI[i];
			Debug.Log (i);
			if (hpbar.GetComponent<BattleSceneUI>())
				hpbar.GetComponent<BattleSceneUI>().SetText(casters[i].GetName());
			
			hpbar.GetComponent<BattleSceneUI>().Setup();
			hpbar.GetComponent<BattleSceneUI>().SetSprite(casters[i].charFace);

			// GameObject hpbar = (GameObject)Instantiate(Resources.Load("prefabs/HPBarPrefab"), playerHpbarPosition - new Vector3(0, 0.5f * i, 0), this.transform.rotation);
			//GameObject hpbar = (GameObject)Instantiate(Resources.Load("prefabs/HPBar"), new Vector3(-5000, -5000, 0), this.transform.rotation);
			// hpbar.transform.SetParent(GameObject.Find("Canvas").transform);
			//hpbar.name = "playerbar";
			// hpbar.GetComponent<HPBarScript>().SetText(casters[i].GetName());
			//hpbar.transform.position = new Vector3(-5000, -5000, 0);

			casters[i].GetComponent<Character>().hpBar = hpbar;
			casters[i].GetComponent<Character>().UpdateHPBar();
			playerChar.transform.position += background.transform.position;
			playerChar.transform.SetParent(background.transform);
			casters[i].battleprefab = playerChar;
			attackers.Add(casters[i].gameObject);

		}
	}

	public List<Character> opponentz;
	// Change to get enemies from map scene, remove EnemyChar class
	private void GetEmemies()
	{
		opponentz.RemoveRange (0, opponentz.Count);
		foreach (GameObject a in queue)
		{
			List<GameObject> targets = a.GetComponent<Action>().GetTargets();
			Debug.Log (queue[0]);
			foreach (GameObject go in targets)
			{
				Character o = go.GetComponent<Character>();

				if (!opponentz.Contains(o))
				{
					opponentz.Add(o);
				}
			}
			/*
            Character opponent = a.GetComponent<Action>().caster;
            if (!casters.Contains(caster))
            {
                casters.Add(caster);
            }*/
		}
		InitEnemies();
	}


	private void InitEnemies()
	{
		for (int i = 0; i < opponentz.Count; i++)
		{
			Vector3 spawnOffset = new Vector3(-characterOffset * i, 0.3f * i, characterOffset * i);
			Vector3 spawnPos = enemyPosition - spawnOffset;

			//string loadstring = "prefabs/characters/" + opponentz[i].GetPrefab().name;
			GameObject sprite = opponentz[i].prefab;
			GameObject enemyChar = (GameObject)Instantiate(sprite, spawnPos, this.transform.rotation);
			objs.Add (enemyChar);

			// GameObject enemyChar = (GameObject)Instantiate(Resources.Load(loadstring), spawnPos, this.transform.rotation);
			// GameObject enemyChar = (GameObject)Instantiate(Resources.Load("prefabs/"+ opponentz[i].GetPrefab()), spawnPos, this.transform.rotation);
			enemyChar.name = "enemy" + i;

			if (enemyChar.GetComponent<SpriteRenderer>().flipX)
				enemyChar.GetComponent<SpriteRenderer>().flipX = false;
			else
				enemyChar.GetComponent<SpriteRenderer>().flipX = true;

			if (rightUI.Count <= i) {
				/*
				Debug.Log ("EnemyのUIの数が足りていません");
				//debug用に最後のオブジェクトをコピー
				GameObject newUI = (GameObject)Instantiate(rightUI[rightUI.Count - 1]);
				rightUI.Add (newUI);
				*/
			}
			GameObject hpbar = rightUI[i];
			if (hpbar.GetComponent<BattleSceneUI>())
				hpbar.GetComponent<BattleSceneUI>().SetText(opponentz[i].GetName());
			hpbar.GetComponent<BattleSceneUI>().Setup();
			hpbar.GetComponent<BattleSceneUI>().SetSprite(opponentz[i].charFace);
			
			// GameObject hpbar = (GameObject)Instantiate(Resources.Load("prefabs/HPBar"),  new Vector3(-5000, -5000, 0), this.transform.rotation);

			hpbar.transform.SetParent(GameObject.Find("Canvas").transform);
			// hpbar.transform.position = new Vector3(-5000, -5000, 0);
			// hpbar.name = "enemybar";
			// hpbar.GetComponent<HPBarScript>().setEnemyBool(true);
			// hpbar.GetComponent<HPBarScript>().SetText(opponentz[i].GetName());

			opponentz[i].GetComponent<Character>().hpBar = hpbar;
			opponentz[i].GetComponent<Character>().UpdateHPBar();

			enemyChar.transform.position += background.transform.position;
			enemyChar.transform.SetParent(background.transform);

			opponentz[i].battleprefab = enemyChar;
			opponents.Add(opponentz[i].gameObject);
		}
	}




	private void RunQueue()
	{
		Debug.Log ("RunQueue");
		foreach (GameObject a in queue)
		{
			float time = 0.5f;

			Action aa = a.GetComponent<Action>();
			aa.RunAction();

			// ------- ↓ New code ↓ ------
			if (aa.GetActionType() == Actions.Attack)
			{ 
				aa.GetCaster();
				List<GameObject> enemy = aa.GetTargets();
				//GameObject caster = aa.GetCaster().GetBattlePrefab();
				//Vector3 startPos = caster.transform.position;

				Character e = enemy[0].GetComponent<Character>();
				//Vector3 pos = e.GetBattlePrefab().transform.position - new Vector3(1, 0, 0);
				//iTween.MoveTo(caster, iTween.Hash("position", pos, "easetype", iTween.EaseType.easeInOutBack, "time", time));
				//iTween.MoveTo(caster, iTween.Hash("position", startPos, "easetype", iTween.EaseType.easeOutQuad, "time", time, "delay", time));

				AudioClip soundEffect = aa.GetCaster().GetAttackSound();
				if (soundEffect != null)
					iTween.Stab(e.gameObject, soundEffect, time / 2);
				else
					Debug.Log("no soundfile set?");

				for (int i = 0; i < enemy.Count; i++)
				{
					//Character c = enemy[i].GetComponent<Character>();
					//iTween.ShakePosition(c.GetBattlePrefab(), iTween.Hash("x", 0.2f, "delay", time / 2, "speed", 10f));
				}
				//iTween.MoveBy(e.GetBattlePrefab(), iTween.Hash("x", 1f, "easetype", iTween.EaseType.easeOutElastic, "delay", time / 5));

				// old
				//iTween.MoveBy(e.GetBattlePrefab(), iTween.Hash("x", 0.5f, "easetype", iTween.EaseType.easeInOutBack, "delay", time / 5));

				iTween.ShakePosition(background, iTween.Hash("x", .1f, "y", .1, time, 0.05f, "delay", time / 2));
				iTween.ShakeRotation(background, iTween.Hash("x", .3f, "y", .3f, time, 0.05f, "delay", time / 2));
			}

			// heal = no one moves
			// ranged attack, attacker doesnt move, damaged enemy moves
			else if (aa.GetActionType() == Actions.Heal)
			{
				aa.GetCaster();
				//List<GameObject> enemy = aa.GetTargets();
				//GameObject caster = aa.GetCaster().GetBattlePrefab();

				//Character e = enemy[0].GetComponent<Character>();
				//Vector3 pos = e.GetBattlePrefab().transform.position - new Vector3(4.5f, 0, 0);
				//iTween.MoveTo(caster, iTween.Hash("position", pos, "easetype", iTween.EaseType.easeOutQuad, "time", time));
			}
			// -----------------------------

		}
			
		Invoke("SlideOut", 1.9f);
	}


	// For debugging only, remove
	void OnGUI() 
	{

	}

}