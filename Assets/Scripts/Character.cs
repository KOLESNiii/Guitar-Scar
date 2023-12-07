using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    [SerializeField]
    protected float speed = 1f;
    [SerializeField]
    protected float direction = 0;
    [SerializeField]
    protected float health = 100f;
    [SerializeField]
    protected float damage = 5f;
    [SerializeField]
    protected bool inBattle = false;
    protected Battle battle;
    protected CharacterController characterController;

    protected override void Start()
    {
        base.Start();
        characterController = gameObject.AddComponent<CharacterController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    protected void Move()
    {
        Vector3 movement = new Vector3();
        movement.x += speed * (float)Mathf.Cos(Mathf.Deg2Rad*direction) * Time.deltaTime;
        movement.y += speed * (float)Mathf.Sin(Mathf.Deg2Rad*direction) * Time.deltaTime;
        animator.SetTrigger("isMoving");
        characterController.Move(movement);
        
    }

    protected void Turn(float angle)
    {
        if (direction == 270 && (angle == -90 || angle == 180))
        {
            animator.SetTrigger("Turn");
            animator.SetBool("isFacingLeft", true);
        }
        else if (direction == 0 && (angle == 90 || angle == 180))
        {
            animator.SetTrigger("Turn");
            animator.SetBool("isFacingLeft", true);
        }
        else if (direction == 90 && (angle == -90 || angle == 180))
        {
            animator.SetTrigger("Turn");
            animator.SetBool("isFacingLeft", false);
        }
        else if (direction == 180 && (angle == 90 || angle == 180))
        {
            animator.SetTrigger("Turn");
            animator.SetBool("isFacingLeft", false);
        }
        direction = (direction + angle) % 360;
    }

    protected int calculateAngleTurned(int newAngle)
    {
        int angleTurned = newAngle - (int)direction;
        return Math.Abs(angleTurned) == 180 ? 180 : angleTurned;
    }

    public virtual void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    protected void Die()
    {
        animator.SetTrigger("Die");
        if (inBattle)
        {
            battle.endBattle(this);
        }
        Destroy(gameObject, 1f);
    }
}
