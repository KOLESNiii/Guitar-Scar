using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Portal class, used to transition between levels
public class Portal : MonoBehaviour
{
    //Time before portal collapses, set in unity editor
    [SerializeField]
    private float aliveTime = 2f;
    public bool isExit = false;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>(); //Gets own animator
        if (!isExit) //If the portal is an entrance, it will collapse a set duration after spawning
        {
            Invoke("Collapse", aliveTime);
        }
    }

    //Collapses the portal to show it has been used
    private void Collapse()
    {
        animator.SetTrigger("collapsePortal");
        if (isExit)
        {
            CurrentLevel.Instance.NextDungeon(); //Loads next dungeon
        }
    }
    public void Delete()
    {
        Destroy(gameObject);
    }

    //Triggers when player collides with portal hitbox, other is player collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isExit)
        {
            Debug.Log("Portal Triggered");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f); //Makes player invisible, as if player has entered portal
            Invoke("Collapse", 1f); //Collapses portal after 1 second
        }
    }
}