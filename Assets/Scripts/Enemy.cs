using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    private int viewRange;
    [SerializeField]
    private double attackSpeed;
    [SerializeField]
    private float blockChance = (float)Level.GetBlockChance();
    [SerializeField]
    private double lastAttack = 0;
    [SerializeField]
    public EnemyType type;
    [SerializeField]
    private bool movementBlocked = false;
    public bool isHardEnemy = false;
    public TextMeshProUGUI attackTextBox;
    //Initialises the character based on the difficilty multiplier of the level
    void Start()
    {
        base.Start();
        float difficultyMultiplier = (float)Level.GetDifficultyMultiplier();
        health = health * difficultyMultiplier;
        maxHealth = health;
        damage = damage * difficultyMultiplier;
        blockChance = (float)Level.GetBlockChance();
        if (isHardEnemy)
        {
            makeHardEnemy();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Blocks behaviour if game is paused or enemy is dead
        if (Global.Paused)
        {
            return;
        }
        if (isDead)
        {
            return;
        }
        if (inBattle) //If in battle, attack
        {
            //Logic for attacking based on attack speed
            if (DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0)).TotalMilliseconds - lastAttack >= attackSpeed) 
            {
                lastAttack = DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0)).TotalMilliseconds;
                var attack = type.getAttacks(); //Gets a random attack from the enemy type
                int enemyAttackIndex = attack.Item1;
                float quality = attack.Item2;
                attackTextBox.text = ChordLibrary.GetChordName(enemyAttackIndex); //Sets the attack text box to the name of the attack
                battle.attack(this, new Chord(enemyAttackIndex, quality), damage); //Attacks the player
            }
        }
        else //If not in battle, move
        {
            //Checks if player is in range
            var collider = Physics2D.OverlapCircle(transform.position, viewRange, 8); //3 is player layer, so 8 is player mask (boolean 100 bitmask)
            if (collider && collider.gameObject.GetComponent<Player>().isDead == false && collider.gameObject.GetComponent<Player>().inBattle == false)
            { //Initiates battle if player is in range
                inBattle = true;
                Debug.Log("Enemy entered battle");
                Player player = collider.gameObject.GetComponent<Player>();
                battle = new Battle(player, this);
                player.enterBattle(battle);
            }
            else if (!movementBlocked) //If player is not in range, move if not blocked
            {
                float movement = UnityEngine.Random.Range(0f,1f);
                if (movement < 0.5)
                {
                    //stay still
                }
                else if (movement < 0.75f)
                {
                    Move();
                }
                else
                {
                    var turn = UnityEngine.Random.Range(0f, 1f);
                    int newAngle;
                    if (turn < 0.25)
                    {
                        newAngle = 90;
                    }
                    else if (turn < 0.5)
                    {
                        newAngle = 180;
                    }
                    else if (turn < 0.75)
                    {
                        newAngle = 270;
                    }
                    else
                    {
                        newAngle = 0;
                    }
                    Turn(calculateAngleTurned(newAngle));
                }
                BlockMovement(); //Blocks movement for a short time
            }
        }
    }
    //Logic to take damage from an attack
    public void takeDamage(float damage)
    {
        CurrentLevel.Instance.playerDamageDealt += damage;
        animator.SetTrigger("isHit");
        health -= damage;
        healthBar.SetValue(health, maxHealth);
        if (health <= 0)
        {
            Die();
        }
    }

    private void ResetMovementBlock() 
    {
        movementBlocked = false;
    }
    //Blocks movement for a short time
    private void BlockMovement()
    {
        movementBlocked = true;
        Invoke("ResetMovementBlock", time); //Resets movement block after time seconds
    }
    public float getBlockChance()
    {
        return blockChance;
    }
    public float getDamage()
    {
        return damage;
    }
    public EnemyType getType()
    {
        return type;
    }
    //Makes the enemy harder by increasing its stats
    public void makeHardEnemy()
    {
        Level.levelNumber += 2;
        blockChance = (float)Level.GetBlockChance();
        Level.levelNumber -= 2;
        health *= 1.5f;
        maxHealth = health;
        damage *= 1.5f;
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1, 0, 1, 1); //Makes the enemy purple to indicate it is harder
    }
}
