using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base class for all entities
public class Entity : MonoBehaviour
{
    protected Animator animator;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
    }
}
