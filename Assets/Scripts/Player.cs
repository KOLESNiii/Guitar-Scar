using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : Character, IDataPersistence
{
    [SerializeField]
    private float armour = 100f;
    [SerializeField]
    private float maxArmour = 100f;
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
    public HealthBar armourBar;
    public ExtraBar healthBarExtra;
    public ExtraBar armourBarExtra;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        var inputs = InputManager.getInputs();
        if (isDead)
        {
            return;
        }
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
        else
        {
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
    }
    public void takeDamage(float damage)
    {
        CurrentLevel.Instance.playerDamageTaken += damage;
        animator.SetTrigger("isHit");

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
        healthBar.SetValue(health, maxHealth);
        armourBar.SetValue(armour, maxArmour);
        armourBarExtra.SetValue(armour, maxArmour, tempMaxArmour);
        healthBarExtra.SetValue(health, maxHealth, tempMaxHealth);
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
        Debug.Log("Player entered battle");
        inBattle = true;
        this.battle = battle;
    }
    public override void exitBattle()
    {
        inBattle = false;
        LevelUp(battle.XPGain);
        this.battle = null;
        inBattle = false;
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
            damage *= Global.LevelUpMultiplier;
            maxHealth *= Global.LevelUpMultiplier;
            maxArmour *= Global.LevelUpMultiplier;
            regenArmour(maxArmour);
            heal(maxHealth);
            XPToNextLevel *= (float)(1.5*Global.LevelUpMultiplier);
        }
    }

    public void LoadData(GameData data)
    {
        level = data.playerLevel;
        XP = data.playerXP;
        XPToNextLevel = Global.startingXPToNextLevel * (float)Math.Pow(1.5 * Global.LevelUpMultiplier, level);
        damage = Global.startingDamage * (float)Math.Pow(Global.LevelUpMultiplier, level);
        maxArmour = Global.startingArmour * (float)Math.Pow(Global.LevelUpMultiplier, level);
        maxHealth = Global.startingHealth * (float)Math.Pow(Global.LevelUpMultiplier, level);
        health = data.playerHealth;
        armour = maxArmour;
        tempMaxArmour = maxArmour;
        tempMaxHealth = maxHealth;
        tempDamage = damage;
    }

    public void SaveData(ref GameData data)
    {
        data.playerLevel = level;
        data.playerXP = XP;
        data.playerHealth = health;
    }
}
