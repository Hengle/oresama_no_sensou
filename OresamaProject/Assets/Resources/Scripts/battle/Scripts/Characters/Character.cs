using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Stat 
{
	str,
	agi,
	inte,
	con,
	def,
	spd,
	crit,
	dodge,
	minMove,
	maxMove,
}

//[System.Serializable]
public class Character : MonoBehaviour {

	[SerializeField]
	private string charName;
    public void SetName(string name) { charName = name; }
	public string GetName() { return charName; }

    public int ID;
    public void SetID(int id) { ID = id; }
    public int GetID() { return ID; }

	[SerializeField]
	private string className;
	public string GetClass() { return className; }

	//キャラクター毎の画像
	public Sprite charSprite;//キャラクター上半身
	public Sprite charFace;//キャラクター顔
    public Sprite classWindow;//ステータス欄のクラス枠

	// base stats;
	private int s_str;
	private int s_agi;
	private int s_int;
	private int s_con;
	private int s_def;
	private int s_spd;
	private int s_crit = 0;
	private int s_dodge = 0;
	private int s_minMove = 0;
	private int s_maxMove = 0;


	private float s_crit_modifier = 2.0f;
	public float GetCritMod() { return s_crit_modifier; }

	// stat mods
	private int m_str;
	private int m_agi;
	private int m_int;
	private int m_con;
	private int m_def;
	private int m_spd;
	private int m_crit;
	private int m_dodge;
	public int m_minMove;
	public int m_maxMove;

	private int s_dmg;
	public void setDamage(int d) { s_dmg = d; }
	public int getDamage() { return s_dmg; }

	// Level stuff
	public TextAsset statCSV;
	private int level = 1;
	private int exp;
	private int expToLevel;

	public  void SetLevel(int v)
	{
		level = v;
		SetStats();
	}
     
    [SerializeField]
    private GameObject characterSprite;
    public GameObject getCharSprite() { return characterSprite; }

    public GameObject prefab;
	public GameObject GetPrefab() { return prefab; }

	public GameObject battleprefab;
	public GameObject GetBattlePrefab() { return battleprefab; }

	public GameObject attackAction;
	public GameObject GetAttackAction() { return attackAction; }

	// Returns MODIFIED Stats
	public int GetStr() { return s_str + m_str; }
	public int GetAgi() { return s_agi + m_agi; }
	public int GetInt() { return s_int + m_int; }
	public int GetCon() { return s_con + m_con; }
	public int GetDef() { return s_def + m_def; }
	public int GetSpd() { return s_spd + m_spd; }
	public int GetCrit() { return s_crit + m_crit + GetAgi() / 2; }
	public int GetDodge() { return s_dodge + m_dodge + GetAgi() / 5; }
	public int GetMinMove(){ return s_minMove + m_minMove; }
	public int GetMaxMove(){ return s_maxMove + m_maxMove; }
	public List<int> GetMove(){ 
		List<int> moves = new List<int>();
		int max = GetMaxMove ();
		for (int i = GetMinMove (); i <= max; i++) {
			moves.Add (i);
		}
		return moves; }


	public int s_hp;
	public int s_maxHp;
	public float GetHp() { return s_hp; }
	public float GetMaxHp() { return s_maxHp; }

	public int s_mp;
	public int s_maxMp;
	public float GetMp() { return s_mp; }
	public float GetMaxMp() { return s_maxMp; } 


	public bool facing = true;

	public Sprite classIcon;

	public GameObject attackAnimation;
	public GameObject bleedAnimation;

	public GameObject hpBar;

	public List<GameObject> statusList = new List<GameObject>();
	public void addStatusEffect(GameObject s) { statusList.Add(s); }

	public GameObject[] skills;

	public AudioClip attackSound;
	public AudioClip GetAttackSound() { return attackSound; } 
	public AudioClip hitSound;

	public bool dontRemove;

	void Start ()
	{
		//DontDestroyOnLoad(transform.root.gameObject);
		SetStats();
		if (characterSprite)
			characterSprite.GetComponent<animationController> ().setHP (s_hp);
		if (hpBar)
			hpBar.SetActive (false);

        // s_maxHp = s_hp;
        // s_maxMp = s_mp;
        // LoopStatusList();
    }

    public void DontDestroy()
	{
		dontRemove = true;
		//DontDestroyOnLoad(transform.root.gameObject);

	}

	public void LoopStatusList() 
	{
		m_str = 0;
		m_agi = 0;
		m_int = 0;
		m_con = 0;
		m_def = 0;
		m_spd = 0;
		m_dodge = 0;
		m_crit = 0;

		List<GameObject> deleteList = new List<GameObject>();

		foreach (GameObject go in statusList) 
		{
			if (go.GetComponent<StatusEffect>())
			{
				StatusEffect s = go.GetComponent<StatusEffect>();

				switch (s.type)
				{
				case EffectType.Buff:
					switch (s.stat)
					{
					case Stat.str:
						m_str += s.StatChange (s_str);
						break;

					case Stat.agi:
						m_agi += s.StatChange(s_agi);
						break;

					case Stat.inte:
						m_int += s.StatChange(s_int);
						break;

					case Stat.con:
						m_con += s.StatChange(s_con);
						break;

					case Stat.def:
						m_def += s.StatChange(s_def);
						break;

					case Stat.spd:
						m_spd += s.StatChange(s_spd);
						break;

					case Stat.crit:
						m_crit += s.StatChange(s_crit);
						break;

					case Stat.dodge:
						m_dodge += s.StatChange(s_dodge);
						break;

                    case Stat.minMove:
                        m_minMove += s.StatChange(s_minMove);
                        break;

                    case Stat.maxMove:
                        m_maxMove += s.StatChange(s_maxMove);
                        break;

					default:
						Debug.Log("Error, stat not set?");
						break;
					}
					break;

				case EffectType.Damage:
					Debug.Log("dmg!");
					TakeDamage(this.gameObject, s.value, false);
					break;
				}

				if (s.Tick() == 0) 
				{
					deleteList.Add(go);          
				}

			}

			else
				Debug.Log("Error, StatusEffect script not applied?");
		}

		foreach (GameObject go in deleteList) 
		{
			statusList.Remove(go);
		}
	}


	public bool CalcHit()
	{
		float dodgeChance = GetDodge();

		float random = Mathf.Floor(Random.Range(0, 100));
		if (dodgeChance <= random)
			return true;

		return false;
	}


	public bool CalcCritical()
	{
		float critchance = GetCrit(); 

		float random = Mathf.Floor(Random.Range(0, 100));
		if (critchance >= random)
			return true;

		return false;
	}


	public virtual int CalcDamage(int v)
	{

		s_dmg = v;



		if (CalcCritical())
			s_dmg = Mathf.FloorToInt(s_dmg * s_crit_modifier);

		return s_dmg;
	}


	// Called when character attacks
	public virtual void Attack(GameObject target, int v,bool isMagick = false) 
	{   

        // set attacking state
        //characterSprite.GetComponent<animationController>().setAttacking(true);
        Debug.Log("attacking 1");
        characterSprite.GetComponent<animationController>().setAttacking();

        if (transform.position.x < target.transform.position.x)
        {
            if (transform.localScale.x != -1)
                transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
        else if (transform.position.x > target.transform.position.x)
        {
            if (transform.localScale.x != 1)
                transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }


		target.GetComponent<Character>().TakeDamage(this.gameObject, CalcDamage(v), false,isMagick);
		Vector3 effectOffset = new Vector3(0, 1, 0);
		Instantiate(bleedAnimation, target.GetComponent<Character>().GetBattlePrefab().transform.position + effectOffset, this.transform.rotation);
		Instantiate(attackAnimation, target.transform.position, this.transform.rotation);
	}


	// Called when character is being attacked
	public virtual void TakeDamage(GameObject a, int v, bool counter,bool isMagick = false)
	{
        Debug.Log("getHit");
        characterSprite.GetComponent<animationController>().TakeDamage();
        if (a != null) {
            if (transform.position.x < a.transform.position.x)
            {
                if (transform.localScale.x != -1)
                    transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
            }
            else if (transform.position.x > a.transform.position.x)
            {
                if (transform.localScale.x != 1)
                    transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
            }
        }

        if (CalcHit()) 
		{
			//魔法攻撃の場合は防御力無効
			if (isMagick) {
				SetHp (v);
			}
			else if (v > GetDef())
			{
				int dmg = v - GetDef();
				SetHp(dmg);
			} else if (v == 1)
				SetHp(1);
		}
	}

    public virtual void SpendMP(int mp) {
        s_mp -= mp;
        if (s_mp < 0) s_mp = 0;
        else if (s_mp > s_maxMp) s_mp = s_maxMp;
    }


	public void SetHp(int v) 
	{
		s_hp -= v;
		if (battleprefab == null)
			battleprefab = gameObject;
        Vector2 pos = characterSprite.transform.position + new Vector3(0.2f,0.0f,0);  
		Vector2 onScreenPos = Camera.main.WorldToScreenPoint(pos);

		GameObject floatingNumber = (GameObject)Instantiate(Resources.Load("prefabs/FloatinDmgNum"), onScreenPos, Quaternion.identity);
		floatingNumber.transform.SetParent(GameObject.Find("Canvas").transform);
		floatingNumber.transform.localScale = new Vector3(2, 2, 2);
		floatingNumber.GetComponent<FloatingDamageNumber>().SetDmgText(v);

        characterSprite.GetComponent<animationController>().setHP(s_hp);

        if (s_hp > s_maxHp) 
		{
			s_hp = s_maxHp;
		}

		else if (s_hp <= 0)
		{
			// dead, do something
			Debug.Log(charName + " is dead");
			Dead ();
		}

		UpdateHPBar();
	}

	//キャラクター死亡関数
	public void Dead(){
		switch(tag){
		case "Player":
			GameRoot.GRoot.playerList.Remove(this.gameObject);
			break;
		case "Enemy":
			GameRoot.GRoot.enemyList.Remove(this.gameObject);
			break;
        default:
            GameRoot.GRoot._characters.Remove(this.gameObject);
            break;
		}

        //this.gameObject.SetActive (false);
        int[] pos = GetComponent<CharacterMove>().nowPos;
        MapCreateScript.mapChips[pos[0], pos[1]].RemoveRideCharacter();

        if (GameController.Gcon.TarnCharacter == gameObject) {
			GameController.Gcon.TarnEnd ();
		}
	}


	public void GainExp(int v) 
	{
		exp += v;

		Debug.Log("Exp: " + exp);
		Debug.Log("Needed: " + expToLevel);

		if (exp >= expToLevel) 
		{
			Debug.Log("Level up! Lvl: " + level);
			level++;
			exp -= expToLevel;
			SetStats();
		}
	}


	private void SetStats() 
	{
		if (statCSV != null)
		{
			// 0 NAME, 1 STR, 2 AGI, 3 INT, 4 CON, 5 DEF, 6 SPD, 7 EXP
			string[] stats = CSVReader.ReadLine(statCSV, level);
			s_str = int.Parse(stats[1]);
			s_agi = int.Parse(stats[2]);
			s_int = int.Parse(stats[3]);
			s_con = int.Parse(stats[4]);
			s_def = int.Parse(stats[5]);
			s_spd = int.Parse(stats[6]);
			expToLevel = int.Parse(stats[7]);

		}
		else
			Debug.Log("!!! ERROR: Character CSV not set in inspector !!!");
	}

	public void UpdateHPBar()
	{
		float barSize = (float)s_hp / (float)s_maxHp;
		//Debug.Log("hp " + s_hp + " max " + s_maxHp);
		if (barSize < 0) 
			barSize = 0;

		if (hpBar != null) {
			hpBar.SetActive (true);
			GameObject bar = hpBar.transform.Find ("hpbar/Bar").gameObject;
			iTween.ScaleTo (bar, iTween.Hash ("x", barSize, "easeType", iTween.EaseType.easeOutSine, "time", 0.3f));
			Invoke ("HPUpdateEnd", 0.3f + 1.0f);
		} else {
			//Debug.Log ("hpbar is null");
		}

	}

	public void HPUpdateEnd(){
		hpBar.SetActive (false);
	}

	//  ↓↓↓ DEBUG ONLY REMOVE ↓↓↓
	void Update()
	{
		if (Input.GetKeyDown("space")) 
		{
			if (gameObject.name == "NEET") 
			{
				GainExp(5);
			}
		}
	}


	///turnorder
	public int speed;
	public int turnTick = 0;

	public Sprite faceSprite;

	public int step()
	{
		turnTick += speed;
		return turnTick;
	}

	public void calcNext()
	{
		turnTick = 0;
	}
}
