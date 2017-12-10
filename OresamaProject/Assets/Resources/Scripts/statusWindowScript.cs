using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class statusWindowScript : MonoBehaviour {
	public Character player;//現在設定されているキャラクター
	public GameObject hpBar,mpBar;
	public GameObject[] nowHpNum;
	public GameObject[] nowMpNum;
    public Sprite[] hpNum;//hp数値の画像リスト
	public Sprite[] mpNum;//mp数値の画像リスト
    public Text playerName;
    public GameObject playerIcon;
    public GameObject classIcon;
	public float barMaxSize;

    public GameObject StatesOvarlay;
    //Character States;
    public GameObject CharaSprite;

    public Text HP, MP;//画面右上の簡易ステータス画面用
    private float _hp, _mp;

	//StatusMenu内の物
    public Text NowHP, MaxHp, NowMP, MaxMP, STR, DEF, AGI, SPD, INT, MOVE, CON, Skill1, Skill1Date, Skill2, Skill2Date;
    public GameObject MenuHPBar, MenuMPBar;
	public float MenuBarMaxSize;

    //public menuScript MS;
	public static statusWindowScript SWS;

    void Awake() { 
        //MS = GameObject.FindGameObjectWithTag("menuWindow").GetComponent<menuScript>(); 
		SWS = this;
    }

    void Start() {
        StatesOvarlay.SetActive(false);
		BarSizeInit ();
        /*Transform back = this.gameObject.transform.FindChild("Button");
        back.SetSiblingIndex(0);
        back = this.gameObject.transform.FindChild("backGround");
        back.SetSiblingIndex(1);*/
    }

    void Update() {
        //HPかMPが変更されたらStatesを更新する
        if (player == null) return;
        if (_hp != player.GetHp() || _mp != player.GetMp()) StatesUpdate();
    }

	public void SelectPlayerChenge(Character newPlayer) {
        player = newPlayer;
        //newPlayerのHP・MPゲージと数値を適用
        //プレイヤーのステータスを取得
        //HPとMPのゲージの大きさをHP・MPに比例させる
        //表示しているplayerNameを更新する
        //Character PlayerS = player.GetComponent<Character>();
        playerName.text = player.GetName();
		//名前の色を敵と味方で区別
		if (player.gameObject.tag == "Player") {
			playerName.color = Color.white;
		} else {
			playerName.color = Color.red;
		}
        //職業アイコンを更新する
        //キャラアイコンを更新する
        Image PImage = playerIcon.GetComponent<Image>();
        //Character playerDate = player.GetComponent<Character>();
        PImage.sprite = player.charFace;

        //職業枠の更新
        Image CImage = classIcon.GetComponent<Image>();
        CImage.sprite = player.classWindow;

        //States = player.GetComponent<Character>();

		StatesUpdate ();
    }

	public void StatesUpdate(){
		ChengeHP ();
		ChengeMP ();
		ChengeStates();
        HPandMPUpdate();
        //MS.menuUpdate();
		//ChengeItem ();
	}

	//hp,mpBarのサイズ確認
	void BarSizeInit(){
		barMaxSize = hpBar.transform.localScale.x;
		MenuBarMaxSize = MenuHPBar.transform.localScale.x;
	}

    public void ChengeHP() {
        //HPゲージと数値の更新
		if (player != null){
			Vector3 bSize = hpBar.transform.localScale;

			//ゲージの長さ更新
			int hp = player.s_hp;
			float maxHp = player.s_maxHp;

            float hpBarWidth = hp / maxHp;

			float barSize = barMaxSize * hpBarWidth;

            if (barSize < 0) barSize = 0.0f;

			hpBar.transform.localScale = new Vector3 (barSize, bSize.y, bSize.z);//barのscaleを変更
			MenuHPBar.transform.localScale = new Vector3(MenuBarMaxSize * hpBarWidth, MenuHPBar.transform.localScale.y, 1.0f);
            //Debug.Log(hp + ":" + maxHp + ":" + hpBar + ":" + barSize);

			//数値の更新
			for (int i = 0; nowHpNum.Length > i; i++) 
            {//表示の初期化
                Image HPImage = nowHpNum[i].GetComponent<Image>();
                HPImage.sprite = hpNum[0];
            }

            for (int i = 0; hp > 0; i++, hp = hp / 10)
            {
                int drawHp;
                drawHp = hp % 10;//HPのi桁目を確認
				if(nowHpNum.Length <= i)break;
                Image HPImage = nowHpNum[i].GetComponent<Image>();
                HPImage.sprite = hpNum[drawHp];//HPの表示を変更
            }
		}
    }

    public void ChengeMP() {
        //MPゲージと数値の更新
		if (player != null){
			Vector3 bSize = mpBar.transform.localScale;

            //ゲージの長さ更新
			int mp = player.s_mp;
			float maxMp = player.s_maxMp;

            float mpBarWidth = mp / maxMp;

			float barSize = barMaxSize * mpBarWidth;

			mpBar.transform.localScale = new Vector3(barSize, bSize.y, bSize.z);//barのscaleを変更
			MenuMPBar.transform.localScale = new Vector3(MenuBarMaxSize * mpBarWidth, MenuMPBar.transform.localScale.y, 1.0f);

			for (int i = 0; nowMpNum.Length > i ; i++)
            {//表示の初期化
                Image MPImage = nowMpNum[i].GetComponent<Image>();
                MPImage.sprite = mpNum[0];
            }

            //数値の更新
            for (int i = 0; mp > 0; i++, mp = mp / 10)
            {
                int drawMp;
                drawMp = mp % 10;//MPのi桁目を確認
				if(nowMpNum.Length <= i)break;
                Image MPImage = nowMpNum[i].GetComponent<Image>();
                MPImage.sprite = mpNum[drawMp];//MPの表示を変更
            }
		}
    }

    void ChengeStates() {
		NowHP.text = player.GetHp().ToString();
		MaxHp.text = player.GetMaxHp().ToString();
		if (player.GetHp () < 0 && player.GetMaxHp () > 0)
			NowHP.text = "0";
		NowMP.text = player.GetMp().ToString();
		MaxMP.text = player.GetMaxMp().ToString();
		if (player.GetMp () < 0 && player.GetMaxMp () > 0)
			NowMP.text = "0";
		STR.text = player.GetStr().ToString();
		DEF.text = player.GetDef().ToString();
		AGI.text = player.GetAgi().ToString();
		SPD.text = player.GetSpd().ToString();
		INT.text = player.GetInt().ToString();

		string moveText = "";
		moveText += player.GetMinMove ().ToString () + " ~ ";
		moveText += player.GetMaxMove ().ToString ();
		MOVE.text = moveText;

		CON.text = player.GetCon().ToString();
        Image CharaImage = CharaSprite.GetComponent<Image>();

        Character PlayerDate = player.GetComponent<Character>();
        CharaImage.sprite = PlayerDate.charSprite;
        Action Skill1Ac = PlayerDate.skills[0].GetComponent<Action>();
        Skill1.text = Skill1Ac.GetName();
		Skill1Date.text = Skill1Ac.GetText ();
        Action Skill2Ac = PlayerDate.skills[1].GetComponent<Action>();
        Skill2.text = Skill2Ac.GetName();
		Skill2Date.text = Skill2Ac.GetText ();
    }

    void HPandMPUpdate() {
        //HP
        _hp = player.GetHp();
        string foward = _hp.ToString();
        string back = player.GetMaxHp().ToString();
        int dis = foward.Length - back.Length;
        //Debug.Log("dis is" + dis);

        while(dis != 0){
            if (dis < 0) {
                foward = "  " + foward;
                dis++;
            }
            else { 
                back += "  ";
                dis--;
            }
        }
        HP.text = foward + "/" + back;

        //MP
        _mp = player.GetMp();
        foward = _mp.ToString();
        back = player.GetMaxMp().ToString();
        dis = foward.Length - back.Length;
        while (dis != 0)
        {
            if (dis < 0)
            {
                foward = "  " + foward;
                dis++;
            }
            else
            {
                back += "  ";
                dis--;
            }
        }
        MP.text = foward + "/" + back;
    }

	/*void ChengeItem(){
		Inventory inventory = GameObject.FindGameObjectWithTag ("Root").GetComponent<Inventory> ();

		if (player.tag == "Player") {
			List<GameObject> PIs = inventory.GetPlayerItems ();
			GameObject[] playerItems = PIs.ToArray();
			for (int i = 0; i < MS.ItemText.Length ; i++) {
				if (i < playerItems.Length) {
					Action itemAC = playerItems [i].GetComponent<Action> ();
					MS.ItemText [i].text = itemAC.GetName();
				}
			}
		} else if (player.tag == "Enemy") {
			List<GameObject> EIs = inventory.GetPlayerItems ();
			GameObject[] enemyItems = EIs.ToArray();
			for (int i = 0; i < MS.ItemText.Length; i++) {
				if (i < enemyItems.Length) {
					Action itemAC = enemyItems [i].GetComponent<Action> ();
					MS.ItemText [i].text = itemAC.GetName();
				}
			}
		}


	}*/

    public void StatesClick() {
		if (StatesOvarlay.activeSelf == false) {
			StatesOvarlay.SetActive (true);
		} else
			StatesOvarlay.SetActive (false);
    }
}
