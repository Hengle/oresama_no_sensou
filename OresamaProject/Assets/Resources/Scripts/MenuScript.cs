using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Image MenuBack;
    public GameObject Move;
    public GameObject Movecancel;
    public GameObject MoveEnter;
    public GameObject Attack;
    public GameObject Skill;
	public GameObject SkillChild;
    public GameObject Item;
    public GameObject Break;
    public GameObject ActionCancel;
    public GameObject ActionEnter;
    //public GameObject EnemyTarnMask;


	public static MenuScript MS;
	public Text skill1;
    public Image skill1Icon;
	public Text skill2;
    public Image skill2Icon;
	private bool isSkill;
	private bool isItem;

	void Awake(){
		MS = this;
	}

    //移動ボタン
    public void MoveButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Menu)
        {
			StartCoroutine(MapMoveScript.MMS.SetPlayer(GameController.Gcon.TarnCharacter));
            Debug.Log("Move");
        }
    }

    //移動キャンセルボタン
    public void MoveCancelButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Move)
        {
            MapMoveScript.MMS.MoveCancel();
            Debug.Log("MoveCancel");
        }
    }

    public void MoveEnterButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Move)
        {
            StartCoroutine(MapMoveScript.MMS.MoveEnd());
            Debug.Log("MoveEnter");
        }
    }

    //攻撃ボタン
    public void AttackButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Menu || GameController.Gcon.GamePhase == Phase.MenuAtMoveEnd)
        {
            Debug.Log("Attack");
            //使用する通常攻撃を取得
            GameObject player = GameController.Gcon.TarnCharacter;
            Action SA = player.GetComponent<Character>().GetAttackAction().GetComponent<Action>();
            CharacterSelect.CS.SelectStart(player, SA);
			//ActionEnterButton ();
        }
    }

    //スキルボタン
    public void SkillButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Menu || GameController.Gcon.GamePhase == Phase.MenuAtMoveEnd)
        {
            Debug.Log("Skill");
			if (isSkill) {
				SkillUpdate (false);
			} else {
				SkillUpdate (true);
			}
			//ActionEnterButton ();
        }
    }

    //スキル[1]、[2]選択ボタン
    public void SkillEnterButton(int num)
    {
        Debug.Log("Skill");
        //使用するスキルを取得
        GameObject player = GameController.Gcon.TarnCharacter;
		Action SA = player.GetComponent<Character>().skills[num].GetComponent<Action>();
        CharacterSelect.CS.SelectStart(player, SA);
		SkillUpdate (false);
    }

    //アイテムボタン
    public void ItemButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Menu || GameController.Gcon.GamePhase == Phase.MenuAtMoveEnd)
        {
            Debug.Log("Item");
            //使用するアイテムを取得
            GameObject player = GameController.Gcon.TarnCharacter;
            Action SA = player.GetComponent<Character>().GetAttackAction().GetComponent<Action>();
            CharacterSelect.CS.SelectStart(player, SA);
        }
    }

    //アイテム選択ボタン
    public void ItemEnterButton()
    {
        Debug.Log("Item");
        //使用するアイテムを取得
        GameObject player = GameController.Gcon.TarnCharacter;
        Action SA = player.GetComponent<Character>().GetAttackAction().GetComponent<Action>();
        CharacterSelect.CS.SelectStart(player, SA);
    }

    //キャラクター選択のキャンセルボタン
    public void ActionCancelButton()
    {
        if (GameController.Gcon.isCharacterSelect)
        {
            Debug.Log("ActionCancel");
            //対象選択を中断
            CharacterSelect.CS.selectCancel();
        }
    }

    //待機ボタン
    public void BreakButton()
    {
        if (GameController.Gcon.GamePhase == Phase.Menu || GameController.Gcon.GamePhase == Phase.MenuAtMoveEnd)
        {
            //Debug.Log("Break");
			GameController.Gcon.TarnEnd ();
        }
    }

    //対象決定ボタン
    public void ActionEnterButton()
    {
        if (GameController.Gcon.isCharacterSelect)
        {
            Debug.Log("ActionEnter");
            CharacterSelect.CS.ActionEnter();
        }
    }

    //private Phase? oldPhase = null;//前フレームのPhase
    void Update()
    {
        Movecancel.SetActive(false);
        Move.SetActive(false);
        MoveEnter.SetActive(false);
        Attack.SetActive(false);
        Skill.SetActive(false);
        Item.SetActive(false);
        Break.SetActive(false);
        ActionCancel.SetActive(false);
        ActionEnter.SetActive(false);
        MenuBack.color = new Color(MenuBack.color.r, MenuBack.color.g, MenuBack.color.b, 0.0f);
        //使用できるボタンを表示、それ以外を非表示にする
        bool ActiveFlag = false;//一つでもアクティブになったか確認用
        if (GameController.Gcon.isanimation || GameController.Gcon.GameSide == Side.Enemy) return;
        //oldPhase = GameController.Gcon.GamePhase;
        if (GameController.Gcon.isCharacterSelect)
        {
            ActiveFlag = true;
            ActionCancel.SetActive(true);
            //ActionEnter.SetActive(true);
        }
        else if (GameController.Gcon.GamePhase == Phase.Move)
        {
            ActiveFlag = true;
            Movecancel.SetActive(true);
            MoveEnter.SetActive(true);
        }
        else if (GameController.Gcon.GamePhase == Phase.Menu || GameController.Gcon.GamePhase == Phase.MenuAtMoveEnd)
        {
            ActiveFlag = true;
            Attack.SetActive(true);
            Skill.SetActive(true);
            Item.SetActive(true);
            Break.SetActive(true);
            if (GameController.Gcon.GamePhase != Phase.MenuAtMoveEnd)
                Move.SetActive(true);
        }

        if (ActiveFlag)
        {
            MenuBack.color = new Color(MenuBack.color.r, MenuBack.color.g, MenuBack.color.b, 1.0f);
        }

		if (GameController.Gcon.TarnCharacter) {
			Character tarn = GameController.Gcon.TarnCharacter.GetComponent<Character> ();
			if (skill1 && tarn.skills.Length > 0) {
                Action skill1AC = tarn.skills [0].GetComponent<Action> ();
				skill1.text = skill1AC.GetName ();
                skill1Icon.sprite = skill1AC.GetIcon();
                if (skill1Icon.sprite == null) skill1Icon.gameObject.SetActive(false);
                else skill1Icon.gameObject.SetActive(true);

				if (skill2 && tarn.skills.Length > 1) {
                    Action skill2AC = tarn.skills[1].GetComponent<Action>();
                    skill2.text = skill2AC.GetName();
                    skill2Icon.sprite = skill2AC.GetIcon();
                    if (skill2Icon.sprite == null) skill2Icon.gameObject.SetActive(false);
                    else skill2Icon.gameObject.SetActive(true);
				}
			}
		}

        Item.SetActive(false);
    }

	private void SkillUpdate(bool state){
		isSkill = state;
		SkillChild.SetActive (state);
	}
}
