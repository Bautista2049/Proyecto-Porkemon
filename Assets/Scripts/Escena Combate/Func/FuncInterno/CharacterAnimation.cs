using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

  
    void Update()
    {
        float speed = rb.velocity.magnitude;

        animator.SetFloat("Speed", speed);
    }
}
