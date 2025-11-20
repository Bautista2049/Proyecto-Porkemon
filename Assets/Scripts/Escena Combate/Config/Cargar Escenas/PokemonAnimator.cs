using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonAnimator : MonoBehaviour
{
   private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void PlayIdle()
    {
        animator.SetInteger("attackType", 0);
    }

    public void PlayAttack(int attackIndex)
    {
        animator.SetInteger("attackType", attackIndex);

     
        Invoke(nameof(ResetToIdle), 1f); 
    }

    private void ResetToIdle()
    {
        animator.SetInteger("attackType", 0);
    }
}