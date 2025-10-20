using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PokemonAnimator : MonoBehaviour
{
   private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdle()
    {
        animator.SetInteger("attackType", 0);
    }

    public void PlayAttack(int attackIndex)
    {
        animator.SetInteger("attackType", attackIndex);

        // volver a Idle al terminar la animación
        Invoke(nameof(ResetToIdle), 1f); // ajustá el 1f al largo real de la animación
    }

    private void ResetToIdle()
    {
        animator.SetInteger("attackType", 0);
    }
}