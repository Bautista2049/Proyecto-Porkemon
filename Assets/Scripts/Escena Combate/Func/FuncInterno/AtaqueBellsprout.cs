using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtaqueBellsprout : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Activar ataque 1
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            anim.SetBool("Ataque1", true);
            anim.SetBool("Ataque2", false);
        }

        // Activar ataque 2
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            anim.SetBool("Ataque2", true);
            anim.SetBool("Ataque1", false);
        }

        // Cuando se suelta la tecla, el ataque vuelve a false
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            anim.SetBool("Ataque1", false);
        }

        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            anim.SetBool("Ataque2", false);
        }
    }
}