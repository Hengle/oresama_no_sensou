using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public enum Actions 
{
    Attack,
    Heal,
    ApplyStatus,
    Item
}


public class Action : MonoBehaviour 
{
    public string actionName;
    public string GetName() { return actionName; }

	//アクションの説明文
	public string actionText;
	public string GetText(){ return actionText;}

    public Sprite ActionSprite;
    public Sprite GetIcon() { return ActionSprite; }

    public Actions type;

    private Character caster;
	public List<GameObject> targets = new List<GameObject>();

    public int MP;//消費するMP
   
    public bool aoe;
    public bool isAoe() { return aoe; }

    // Allowed targets
    public bool canTargetEnemy;
    public bool canTargetFriendly;
    public bool canTargetSelf;

    public GameObject statusEffect;

    public GameObject animationEffect;//表示されるアニメーション

    public AudioClip soundEffect;

    public int range = 0;
    public int GetRange() { return range; } 

    // Modifiers
    public float str_mod; 
    public float agi_mod;
    public float int_mod;

    public void SetCaster(Character c) { caster = c; }
    public Character GetCaster() { return caster; }

    public void AddTargets(GameObject c) { targets.Add(c); }
	public void AddTargets(List<GameObject> c) {
		for (int i = 0; i < c.Count; i++) {
			targets.Add (c [i]);
		}
	}
	public List<GameObject> GetTargets() { return targets; }
	public void RemoveTargets(){ targets.RemoveRange (0, targets.Count); }

    public Actions GetActionType() { return type; }

    void Start()
    {
        //DontDestroyOnLoad(transform.root.gameObject); //TODO: This needs to be changed
    }

    public void RunAction()
    {
        Invoke("DelayedTest", 0.2f);
    }

    public void DelayedTest() 
    {
        caster.SpendMP(MP);

        switch (type)
        {
			case Actions.Attack:
				int dmg_mod = Mathf.FloorToInt (caster.GetStr () * str_mod + caster.GetAgi () * agi_mod + caster.GetInt () * int_mod);
				bool isMagickAttack = false;
				if (str_mod < int_mod)
					isMagickAttack = true;
                foreach (GameObject c in targets)
                {
					
                    caster.Attack(c.gameObject, dmg_mod,isMagickAttack);
                    if (statusEffect)
                    {
                        c.GetComponent<Character>().addStatusEffect(statusEffect);
                        //Instantiate(animationEffect, c.transform.position, this.transform.rotation);
                    }
                    Vector3 pos = c.GetComponent<Character>().GetBattlePrefab().transform.position;
				if (animationEffect) {
					if (isAoe ()) {
						GameObject AniObj = Instantiate (animationEffect, caster.transform.position, this.transform.rotation)as GameObject;
						AniObj.transform.localScale = AniObj.transform.localScale * (GetRange () + 1);
					}
					else
						Instantiate (animationEffect, pos, this.transform.rotation);
				}

                }
                break;

            case Actions.Heal:
                int heal_mod = Mathf.FloorToInt(caster.GetStr() * str_mod + caster.GetAgi() * agi_mod + caster.GetInt() * int_mod);
                foreach (GameObject c in targets)
                {
                    Debug.Log("healng");
                    // caster.Attack(c.gameObject, -heal_mod);
                    c.GetComponent<Character>().SetHp(-heal_mod);
                    // (c.gameObject, -heal_mod);
                    Vector3 pos = c.GetComponent<Character>().GetBattlePrefab().transform.position;
                    Instantiate(animationEffect, pos, this.transform.rotation);
                }
                break;

            case Actions.ApplyStatus:
                foreach (GameObject c in targets)
                {
                    Vector3 pos = c.GetComponent<Character>().GetBattlePrefab().transform.position;
                    Instantiate(animationEffect, pos, this.transform.rotation);
                    // c.addStatusEffect(statusEffect);
                    // Instantiate(animationEffect, c.transform.position, this.transform.rotation);
                }
                break;

            case Actions.Item:
                break;
        }
		Debug.Log ("runAction");

        if (soundEffect)
            SoundManager.SM.playSE(soundEffect);
        //Destroy(gameObject);
    }



    public bool TargetAllowed(GameObject c, GameObject e) 
    {
        bool allowed = false;

        if (canTargetEnemy && c.tag != e.tag)
            allowed = true;
        else if (canTargetFriendly && c.tag == e.tag && c != e)
            allowed = true;
        else if (canTargetSelf && c == e)
            allowed = true;

        return allowed;
    }

    //引数のキャラクターがこのアクションを使用可能か
    public bool IsCanAction(Character caster) {
        if (caster == null) return false;
        if (caster.GetMp() < MP) return false;

        return true;
    }
}
