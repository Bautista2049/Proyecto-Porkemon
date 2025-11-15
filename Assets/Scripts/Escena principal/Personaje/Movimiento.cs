using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Movimiento : MonoBehaviour
{
    public float velocidad = 5f;
    public Transform cam;

    private void Start()
    {
        if (GameState.posicionJugadorGuardadaDisponible && SceneManager.GetActiveScene().name == "Escena Principal")
        {
            transform.position = GameState.posicionJugadorGuardada;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direccion = new Vector3(horizontal, 0f, vertical).normalized;

        if (Input.GetKey(KeyCode.LeftShift))
            velocidad = 10f;
        else
            velocidad = 5f;

        if (direccion.magnitude >= 0.1f)
        {
            Vector3 camForward = cam.forward;
            camForward.y = 0f;
            camForward.Normalize();

            Vector3 camRight = cam.right;
            camRight.y = 0f;
            camRight.Normalize();

            Vector3 moveDir = camForward * vertical + camRight * horizontal;

            transform.Translate(moveDir * velocidad * Time.deltaTime, Space.World);
            transform.rotation = Quaternion.LookRotation(moveDir);
        }
    }
}