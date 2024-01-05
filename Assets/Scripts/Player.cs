using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

//Player class
public class Player : Character, IDataPersistence
{
    //variables assigned in unity editor
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
    private Tilemap tilemap;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (Global.Paused) //If the game is paused, do not update
        {
            return;
        }
        var inputs = InputManager.getInputs(); //Get inputs
        if (isDead) //If the player is dead, do not update
        {
            return;
        }
        if (inBattle) //If the player is in battle, attack
        {
            foreach (var input in inputs)
            {
                if (input.type == InputManager.Type.Chord) //Only process attack (chord) inputs
                {
                    battle.attack(this, input.Chord, Math.Max(damage, tempDamage));
                }
            }
        }
        else //If the player is not in battle, move
        {
            foreach (var input in inputs)
            {
                if (input.type == InputManager.Type.Pause)
                {
                    Global.Pause(); //Pause the game
                }
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
    //Takes damage from battle
    public void takeDamage(float damage)
    {
        CurrentLevel.Instance.playerDamageTaken += damage;
        animator.SetTrigger("isHit"); //Play hit animation
        if (armour > 0)
        {
            armour -= damage; //Take damage from armour first
            if (armour < 0) //If armour is depleted, take damage from health
            {
                health += armour;
                armour = 0;
            }
        }
        else
        {
            health -= damage;
        }
        healthBar.SetValue(health, maxHealth); //Update health and armour bars
        armourBar.SetValue(armour, maxArmour);
        armourBarExtra.SetValue(armour, maxArmour, tempMaxArmour);
        healthBarExtra.SetValue(health, maxHealth, tempMaxHealth);
        if (health <= 0)
        {
            health = 0;
            Die(); //If health is depleted, die
        }
    }
    //Heals to max health
    public void heal(float addedHealth)
    {
        health += addedHealth;
        if (health > Math.Max(maxHealth, tempMaxHealth))
        {
            health = Math.Max(maxHealth, tempMaxHealth);
        }
    }
    //Regenerates armour to max armour
    public void regenArmour(float addedArmour)
    {
        armour += addedArmour;
        if (armour > Math.Max(maxArmour, tempMaxArmour))
        {
            armour = Math.Max(maxArmour, tempMaxArmour);
        }
    }
    //Updates behaviours for battle
    public void enterBattle(Battle battle)
    {
        Debug.Log("Player entered battle");
        inBattle = true;
        this.battle = battle;
    }
    //Updates behaviours for leaving battle, regenerates armour and gets XP from battle
    public override void exitBattle()
    {
        inBattle = false;
        LevelUp(battle.XPGain);
        this.battle = null;
        inBattle = false;
        armour = Math.Max(maxArmour, tempMaxArmour);
        // TODO: Autosave
    }
    //Exits dungeon, which heals the player to max health and regenerates armour to max armour
    public void exitDungeon()
    {
        heal(Math.Max(maxHealth, tempMaxHealth));
        regenArmour(Math.Max(maxArmour, tempMaxArmour));
    }
    //Resets temporary stats when potion effect ends, not used
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
    //Logic to use a drop, not used
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
    //Adds XP and levels up if XP is greater than XP to next level
    public void LevelUp(float XPGain)
    {
        XP += XPGain;
        if (XP >= XPToNextLevel)
        {
            level++;
            XP -= XPToNextLevel;
            damage *= Global.LevelUpMultiplier; //Stats increases
            maxHealth *= Global.LevelUpMultiplier;
            maxArmour *= Global.LevelUpMultiplier;
            regenArmour(maxArmour); //Heals and regenerates armour
            heal(maxHealth);
            XPToNextLevel *= (float)(1.5*Global.LevelUpMultiplier); //XP to next level increases
        }
    }
    //Loads data from save file
    public void LoadData(GameData data)
    {
        level = data.playerLevel;
        XP = data.playerXP;
        //Most stats can be calculated from level and starting values alone
        XPToNextLevel = Global.startingXPToNextLevel * (float)Math.Pow(1.5 * Global.LevelUpMultiplier, level);
        damage = Global.startingDamage * (float)Math.Pow(Global.LevelUpMultiplier, level);
        maxArmour = Global.startingArmour * (float)Math.Pow(Global.LevelUpMultiplier, level);
        maxHealth = Global.startingHealth * (float)Math.Pow(Global.LevelUpMultiplier, level);
        health = data.playerHealth; //Health is saved, as it could be lower than max health
        armour = maxArmour;
        tempMaxArmour = maxArmour;
        tempMaxHealth = maxHealth;
        tempDamage = damage;
    }
    //Saves data to save file
    public void SaveData(ref GameData data)
    {
        data.playerLevel = level; //Only saves data that cannot be easily reconstructed
        data.playerXP = XP;
        data.playerHealth = health;
    }
}
