using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    public Transform cam; // arrastrá la cámara acá en el inspector

    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccion = new Vector3(horizontal, 0f, vertical).normalized;

        // correr con shift
        if (Input.GetKey(KeyCode.LeftShift))
            velocidad = 10f;
        else
            velocidad = 5f;

        if (direccion.magnitude >= 0.1f)
        {
            // tomamos la dirección de la cámara (pero sin inclinación en Y)
            Vector3 camForward = cam.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cam.right;
            camRight.y = 0f;
            camRight.Normalize();

            // movimiento en base a hacia donde mira la cámara
            Vector3 moveDir = camForward * vertical + camRight * horizontal;

            // mover en esa dirección
            transform.Translate(moveDir * velocidad * Time.deltaTime, Space.World);

            // opcional: que el personaje rote hacia donde se mueve
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}