using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField]
    private float aliveTime = 2f;
    public bool isExit = false;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        if (!isExit)
        {
            Invoke("Collapse", aliveTime);
        }
    }

    private void Collapse()
    {
        animator.SetTrigger("collapsePortal");
        if (isExit)
        {
            CurrentLevel.Instance.NextDungeon();
        }
    }

    public void Delete()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isExit)
        {
            Debug.Log("Portal Triggered");
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            Invoke("Collapse", 1f);
        }
    }
}
