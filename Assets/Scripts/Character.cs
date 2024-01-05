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
    protected Rigidbody2D rb; //rigidbody of character, used for movement
    public HealthBar healthBar;
    public Animator BlockAnimator;

    protected override void Start()
    {
        base.Start();
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    public void Move()
    {
        Vector2 movement = new Vector2(Mathf.Cos(Mathf.Deg2Rad*direction), Mathf.Sin(Mathf.Deg2Rad*direction)); //calculate movement vector
        animator.SetBool("isMoving", true); //set animator to moving
        rb.velocity = movement * speed/time; //set velocity to movement vector
        Invoke("Stop", time); //stop movement after time, for blocky movement
    }

    //called by battle manager, to make battle scene ui show block animation
    public void Block()
    {
        BlockAnimator.SetTrigger("Block");
    }

    //stops movement and suspends animation
    protected void Stop()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    //attempts to flip character sprite if new direction is different to original direction
    protected void TryFlip(bool isLeft)
    {
        if (isLeft != isFacingLeft)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            isFacingLeft = isLeft;
        }
    }

    //takes a float angle as a parameter, and turns the character by that angle
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

    //gets the angle turned, taking the new angle the character should be facing as a parameter
    public int calculateAngleTurned(int newAngle)
    {
        int angleTurned = newAngle - (int)direction;
        return Math.Abs(angleTurned) == 180 ? 180 : angleTurned;
    }

    //functionality for character death
    protected void Die()
    {
        animator.SetTrigger("isDead"); //set animator to dead
        if (inBattle)
        {
            battle.endBattle(this); //end battle if character is in battle
        }
        isDead = true; //blocks behaviours from being called
    }

    //resets the character's fields to not in battle values
    public virtual void exitBattle()
    {
        inBattle = false;
        battle = null;
    }
}