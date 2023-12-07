using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [SerializeField]
    private float armour = 100f;
    [SerializeField]
    private float maxArmour = 100f;
    [SerializeField]
    private float maxHealth = 100f;
    [SerializeField]
    private float tempMaxHealth = 100f;
    [SerializeField]
    private float tempMaxArmour = 100f;
    [SerializeField]
    private float tempDamage = 5f;
    [SerializeField]
    private int level = 0;
    [SerializeField]
    private float XP = 0;
    [SerializeField]
    private float XPToNextLevel = 100;
    [SerializeField]
    private float LevelUpMultiplier = 1.5f;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        var inputs = InputManager.getInputs();
        if (inBattle)
        {
            foreach (var input in inputs)
            {
                if (input.type == InputManager.Type.Chord)
                {
                    battle.attack(this, input.Chord, Math.Max(damage, tempDamage));
                }
            }
        }
        foreach (var input in inputs)
        {
            if (input.type == InputManager.Type.Movement)
            {
                int newAngle;
                if (input.Movement == InputManager.Movement.Up)
                {
                    newAngle = 90;
                }
                else if (input.Movement == InputManager.Movement.Left)
                {
                    newAngle = 180;
                }
                else if (input.Movement == InputManager.Movement.Down)
                {
                    newAngle = 270;
                }
                else
                {
                    newAngle = 0;
                }
                Turn(calculateAngleTurned(newAngle));
                Move();
            }
        }
    }

    public override void takeDamage(float damage)
    {
        if (armour > 0)
        {
            armour -= damage;
            if (armour < 0)
            {
                health += armour;
                armour = 0;
            }
        }
        else
        {
            health -= damage;
        }
        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    public void heal(float addedHealth)
    {
        health += addedHealth;
        if (health > Math.Max(maxHealth, tempMaxHealth))
        {
            health = Math.Max(maxHealth, tempMaxHealth);
        }
    }
    public void regenArmour(float addedArmour)
    {
        armour += addedArmour;
        if (armour > Math.Max(maxArmour, tempMaxArmour))
        {
            armour = Math.Max(maxArmour, tempMaxArmour);
        }
    }
    public void enterBattle(Battle battle)
    {
        inBattle = true;
        this.battle = battle;
    }
    public void exitBattle()
    {
        inBattle = false;
        this.battle = null;
        LevelUp(battle.XPGain);
        armour = Math.Max(maxArmour, tempMaxArmour);
        // TODO: Autosave
    }
    public void exitDungeon()
    {
        heal(Math.Max(maxHealth, tempMaxHealth));
        regenArmour(Math.Max(maxArmour, tempMaxArmour));
    }
    private IEnumerator resetTempStat(float duration, int statNumber, float value)
    {
        yield return new WaitForSeconds(duration);
        if (statNumber == 0)
        {
            tempDamage -= value;
        }
        else if (statNumber == 1)
        {
            tempMaxHealth -= value;
        }
        else if (statNumber == 2)
        {
            tempMaxArmour -= value;
        }
    }
    public void useItem(Drop item)
    {
        if (item.Type == Drop.Types.Health)
        {
            heal(item.Value);
        }
        else if (item.Type == Drop.Types.Armour)
        {
            regenArmour(item.Value);
        }
        else if (item.Type == Drop.Types.Damage)
        {
            tempDamage += item.Value;
            var coroutine = resetTempStat(item.Duration, 0, item.Value);
            StartCoroutine(coroutine);
        }
        else if (item.Type == Drop.Types.MaxHealth)
        {
            tempMaxHealth += item.Value;
            var coroutine = resetTempStat(item.Duration, 1, item.Value);
            StartCoroutine(coroutine);
        }
        else if (item.Type == Drop.Types.MaxArmour)
        {
            tempMaxArmour += item.Value;
            var coroutine = resetTempStat(item.Duration, 2, item.Value);
            StartCoroutine(coroutine);
        }
    }
    public void LevelUp(float XPGain)
    {
        XP += XPGain;
        if (XP >= XPToNextLevel)
        {
            level++;
            XP -= XPToNextLevel;
            damage *= LevelUpMultiplier;
            maxHealth *= LevelUpMultiplier;
            maxArmour *= LevelUpMultiplier;
            regenArmour(maxArmour);
            heal(maxHealth);
            XPToNextLevel *= (float)(1.5*LevelUpMultiplier);
        }
    }
}
