using UnityEngine;
using System.Collections;

public class Neet : Character
{
    public override int CalcDamage(int v)
    {
        setDamage(v);

        if (CalcCritical())
            setDamage(Mathf.FloorToInt(getDamage() * GetCritMod()));
        else
            setDamage(1);

        return getDamage();
    }
}
