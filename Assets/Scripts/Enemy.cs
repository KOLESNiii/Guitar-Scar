using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField]
    private int viewRange;
    [SerializeField]
    private float attackSpeed;
    [SerializeField]
    private float blockChance;
    [SerializeField]
    private float lastAttack = 0;
    [SerializeField]
    private EnemyType type;
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (inBattle)
        {
            if (DateTime.Now.Millisecond - lastAttack >= attackSpeed)
            {
                lastAttack = DateTime.Now.Millisecond;
                var attack = type.getAttacks();
                int enemyAttackIndex = attack.Item1;
                float quality = attack.Item2;
                battle.attack(this, new Chord(enemyAttackIndex, quality), damage);
            }
        }
        else
        {
            var collider = Physics2D.OverlapCircle(transform.position, viewRange, 3); //3 is player layer
            if (collider)
            {
                inBattle = true;
                
            }
        }
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
}
