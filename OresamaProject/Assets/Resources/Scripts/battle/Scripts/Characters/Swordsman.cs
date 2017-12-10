using UnityEngine;
using System.Collections;

public class Swordsman : Character
{
	public override void TakeDamage(GameObject a, int v, bool counter,bool isMagick = false)
    {
        setDamage(v);
        if (v > GetDef())
        {
            int dmg = v - GetDef();
            SetHp(dmg);
        }
        if (s_hp > 0 && counter)
            Attack(a, getDamage());
    }
}
