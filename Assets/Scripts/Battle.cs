using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using UnityEditor;
public class Battle
{
    private Player player;
    private Enemy enemy;
    public float XPGain
    {private set; get;}
    private float enemyBlockChance;
    private EnemyType enemyType;
    private float enemyLastAttack = 0f;
    private bool incomingAttack = false;
    private int enemyAttackChordIndex;
    private int? playerAttackChordIndex = null;
    private int consecutiveSameChords = 0;

    public Battle(Player player, Enemy enemy)
    {
        XPGain = 0f;
        this.player = player;
        this.enemy = enemy;
        enemyBlockChance = enemy.getBlockChance();
        enemyType = enemy.getType();
        //BattleUIManager.makeBattleScene(CurrentLevel.Environment.Ints, player.gameObject, enemy.gameObject);
        BattleUIManager.makeBattleScene(new int[]{1}, player.gameObject, enemy.gameObject);
    }

    public void attack(Character attacker, Chord attack, float damage)
    {
        damage *= attack.Quality;
        if (attacker == player)
        {
            if (playerAttackChordIndex == attack.ChordIndex)
            {
                consecutiveSameChords++;
            }
            else
            {
                consecutiveSameChords = 1;
                playerAttackChordIndex = attack.ChordIndex;
            }
            damage *= (float)(1/(2* Math.Pow(consecutiveSameChords, 2))) + 1/(2 * consecutiveSameChords);
            if (incomingAttack && ChordLibrary.IsRelativeMajMin(enemyAttackChordIndex, attack.ChordIndex))
            {
                Debug.Log("Player blocked enemy attack");
                player.Block();
                incomingAttack = false;
                XPGain += Global.BlockXPGain;
            }
            else
            {
                Debug.Log($"Player attacked with {ChordLibrary.GetChordName(attack.ChordIndex)}");
                if (enemyType.StrongAgainst.Contains(attack.ChordIndex))
                {
                    damage *= 0.5f;
                    Debug.Log("Enemy is strong against this attack, damage halved");
                }
                else if (enemyType.WeakAgainst.Contains(attack.ChordIndex))
                {
                    damage *= 2f;
                    Debug.Log("Enemy is weak against this attack, damage doubled");
                    XPGain += Global.StrongXPGain;
                }
                else
                {
                    XPGain += Global.NormalXPGain;
                }
                if (UnityEngine.Random.Range(0f, 1f) < enemyBlockChance)
                {
                    Debug.Log("Enemy blocked player attack");
                    enemy.Block();
                }
                else
                {
                    Debug.Log($"Enemy took {damage} damage");
                    enemy.takeDamage(damage);
                }
            }
        }
        else
        {
            if (incomingAttack)
            {
                Debug.Log($"Player took {damage} damage");
                player.takeDamage(damage);
            }
            else
            {
                incomingAttack = true;
            }
            Debug.Log($"Enemy attacked with {ChordLibrary.GetChordName(attack.ChordIndex)}");
            enemyAttackChordIndex = attack.ChordIndex;
            enemyLastAttack = (float)DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
        }
    }

    public void endBattle(Character loser)
    {
        if (loser == (Character)enemy)
        {
            Debug.Log("Player won battle");
            XPGain += enemyType.XP;
            XPGain *= (1 + enemyBlockChance);
            player.LevelUp(XPGain);
            Debug.Log($"Player gained {XPGain} XP");
            CurrentLevel.Instance.enemiesKilled ++;
            player.exitBattle();
            player.StartCoroutine(endBattleDelay());
        }
        else
        {
            Debug.Log("Player lost battle");
            player.exitBattle();
            enemy.exitBattle();
            BattleUIManager.closeBattleScene();
            Global.GameOver();
        }
        
    }

    public IEnumerator endBattleDelay(){
        yield return new WaitForSeconds(2f);
        UnityEngine.Object.Destroy(enemy.gameObject);
        BattleUIManager.closeBattleScene(player.gameObject);
    }

    void Update()
    {
        if (enemyLastAttack != 0f || ((DateTime.Now.Subtract(new DateTime(1970,1,1,0,0,0)).TotalMilliseconds - enemyLastAttack) > Level.GetBlockTime() && incomingAttack))
        {
            incomingAttack = false;
            Debug.Log($"Player took {enemy.getDamage()} damage");
            player.takeDamage(enemy.getDamage());
        }   
    }
}
