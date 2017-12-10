using UnityEngine;
using System.Collections;

public enum EffectType 
{
    Buff,
    Damage
};


public class StatusEffect : MonoBehaviour 
{
    public string statusName;

    public EffectType type;
    public Stat stat;

    public bool percent;

    public int duration;
    public int value;

    public GameObject effect;
    
    public int StatChange(int v) 
    {
        int va = value;
        if (percent)
            va = Mathf.FloorToInt((float)v * ((float)value / 100));

        return va;
    }


    public int GetDamage() 
    {
        return value;
    }


    public int Tick() 
    {
        duration--;
        return duration; 
    }

}
