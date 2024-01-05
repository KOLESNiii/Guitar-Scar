using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all drops
public class Drop : Entity
{
    public enum Types
    {
        Health,
        Armour,
        Damage,
        XP,
        MaxHealth,
        MaxArmour
    }
    public Types Type
    {private set; get;}
    public float Value
    {private set; get;}
    public float Duration
    {private set; get;}
    
    protected override void Start()
    {
        base.Start();
        Type = (Types)Random.Range(0, 6);
        Value = Random.Range(1, 100);
        Duration = Random.Range(1, 120);
    }
}
