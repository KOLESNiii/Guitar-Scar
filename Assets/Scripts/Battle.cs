using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;
public class Battle
{
    private Player player;
    private Enemy enemy;
    public float XPGain // XP gained from battle
    {private set; get;}
    private float enemyBlockChance;
    private EnemyType enemyType;
    private float enemyLastAttack = 0f;
    private bool incomingAttack = false; //from enemy to player
    private int enemyAttackChordIndex;
    private int? playerAttackChordIndex = null;
    private int consecutiveSameChords = 0; //consecutive same chords played by player

    //Constructor takes a player and an enemy as parameters
    public Battle(Player player, Enemy enemy)
    {
        XPGain = 0f; //initialize XP gain to 0
        this.player = player;
        this.enemy = enemy;
        enemyBlockChance = enemy.getBlockChance(); //get enemy block chance
        enemyType = enemy.getType(); //get enemy type
        BattleUIManager.makeBattleScene(new int[]{1}, player.gameObject, enemy.gameObject); //make battle scene with grassy background
    }

    //Attack method takes an attacker, an attack, and a damage value as parameters
    public void attack(Character attacker, Chord attack, float damage)
    {
        damage *= attack.Quality; //multiply damage by attack quality, only relevant for player
        if (attacker == player) //specific behaviour for player attack
        {
            if (playerAttackChordIndex == attack.ChordIndex)
            {
                consecutiveSameChords++; //increase consecutive same chords played by player
            }
            else
            {
                consecutiveSameChords = 1; //reset consecutive same chords played by player
                playerAttackChordIndex = attack.ChordIndex;
            }
            damage *= (float)(1/(2* Math.Pow(consecutiveSameChords, 2))) + 1/(2 * consecutiveSameChords); //decreasing damage for consecutive same chords played by player
            if (incomingAttack && ChordLibrary.IsRelativeMajMin(enemyAttackChordIndex, attack.ChordIndex)) //if player blocks enemy attack
            {
                Debug.Log("Player blocked enemy attack");
                player.Block();
                incomingAttack = false; //reset incoming attack, as player blocked it
                XPGain += Global.BlockXPGain;
            }
            else //if player doesn't block enemy attack or there is no incoming attack
            {
                Debug.Log($"Player attacked with {ChordLibrary.GetChordName(attack.ChordIndex)}");
                if (enemyType.StrongAgainst.Contains(attack.ChordIndex)) //if enemy is strong against player attack
                {
                    damage *= 0.5f;
                    Debug.Log("Enemy is strong against this attack, damage halved");
                }
                else if (enemyType.WeakAgainst.Contains(attack.ChordIndex)) //if enemy is weak against player attack
                {
                    damage *= 2f;
                    Debug.Log("Enemy is weak against this attack, damage doubled");
                    XPGain += Global.StrongXPGain;
                }
                else //if enemy is neutral against player attack
                {
                    XPGain += Global.NormalXPGain;
                }
                if (UnityEngine.Random.Range(0f, 1f) < enemyBlockChance) //if enemy blocks player attack
                {
                    Debug.Log("Enemy blocked player attack");
                    enemy.Block();
                }
                else //if enemy doesn't block player attack
                {
                    Debug.Log($"Enemy took {damage} damage");
                    enemy.takeDamage(damage);
                }
            }
        }
        else //specific behaviour for enemy attack
        {
            if (incomingAttack) //if there is already an incoming attack from enemy
            {
                Debug.Log($"Player took {damage} damage");
                player.takeDamage(damage); //player takes damage from previous attack
            }
            else //if there is no incoming attack from enemy
            {
                incomingAttack = true; //set incoming attack to true, and deal no immedate damage
            }
            Debug.Log($"Enemy attacked with {ChordLibrary.GetChordName(attack.ChordIndex)}");
            enemyAttackChordIndex = attack.ChordIndex;
            enemyLastAttack = (float)DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds; //set enemy last attack to current time
        }
    }

    //end battle method takes a loser as parameter
    public void endBattle(Character loser) 
    {
        if (loser == (Character)enemy) //if player wins battle
        {
            Debug.Log("Player won battle");
            XPGain += enemyType.XP; //add enemy XP to XP gain
            XPGain *= (1 + enemyBlockChance); //increase XP gain by enemy block chance
            Debug.Log($"Player gained {XPGain} XP");
            CurrentLevel.Instance.enemiesKilled ++; //increase enemies killed stat
            player.exitBattle(); 
            player.StartCoroutine(endBattleDelay(player.gameObject)); //end battle after 2 seconds, for dramatic effect and enemy death animation
        }
        else //if player
        {
            Debug.Log("Player lost battle");
            player.exitBattle();
            enemy.StartCoroutine(endBattleDelay()); //end battle after 2 seconds, for dramatic effect and player death animation
            Global.GameOver(); //open game over screen
        }
        
    }

    public IEnumerator endBattleDelay(GameObject player = null){ //delayed function
        yield return new WaitForSeconds(2f);
        UnityEngine.Object.Destroy(enemy.gameObject);
        if (player != null)
        {
            BattleUIManager.closeBattleScene(player);
        }
        else
        {
            BattleUIManager.closeBattleScene();
        }
    }

    void Update()
    {
        //timeout for player to block enemy attack
        if (enemyLastAttack != 0f || ((DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0)).TotalMilliseconds - enemyLastAttack) > Level.GetBlockTime() && incomingAttack))
        {
            incomingAttack = false;
            Debug.Log($"Player took {enemy.getDamage()} damage");
            player.takeDamage(enemy.getDamage());
        }   
    }
}
