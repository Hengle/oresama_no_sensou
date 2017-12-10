using UnityEngine;
using System.Collections;

public class Thief : Character {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public override void Attack(GameObject target, int v,bool isMagick = false)
    {
        int dmg = v + 40;
            target.GetComponent<Character>().TakeDamage(this.gameObject, dmg, false);
    }
}
