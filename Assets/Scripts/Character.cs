using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Character : Entity
{
    [SerializeField]
    protected float speed = 1f;
    [SerializeField]
    protected float time = 0.5f;
    [SerializeField]
    protected float direction = 0;
    [SerializeField]
    public float health = 100f;
    [SerializeField]
    public float maxHealth = 100f;
    [SerializeField]
    protected float damage = 5f;
    [SerializeField]
    public bool inBattle = false;
    protected bool isFacingLeft = false;
    public bool isDead = false;
    protected Battle battle;
    protected Rigidbody2D rb;
    public HealthBar healthBar;
    public Animator BlockAnimator;

    protected override void Start()
    {
        base.Start();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    public void Move()
    {
        Vector2 movement = new Vector2(Mathf.Cos(Mathf.Deg2Rad*direction), Mathf.Sin(Mathf.Deg2Rad*direction));
        animator.SetBool("isMoving", true);
        rb.velocity = movement * speed/time;
        Invoke("Stop", time);
    }

    public void Block()
    {
        BlockAnimator.SetTrigger("Block");
    }

    protected void Stop()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    protected void TryFlip(bool isLeft)
    {
        if (isLeft != isFacingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            isFacingLeft = isLeft;
        }
    }

    public void Turn(float angle)
    {
        direction = (direction + angle) % 360;
        if (direction == 0)
        {
            TryFlip(false);
        }
        else if (direction == 180)
        {
            TryFlip(true);
        }
    }

    public int calculateAngleTurned(int newAngle)
    {
        int angleTurned = newAngle - (int)direction;
        return Math.Abs(angleTurned) == 180 ? 180 : angleTurned;
    }

    protected void Die()
    {
        animator.SetTrigger("isDead");
        if (inBattle)
        {
            battle.endBattle(this);
        }
        isDead = true;
    }

    public virtual void exitBattle()
    {
        inBattle = false;
        battle = null;
    }
}
