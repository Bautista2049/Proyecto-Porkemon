﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        // hacer que el objeto rote para mirar a la cámara sin cambiar su posición (forma simple)
        Vector3 lookPos = mainCamera.transform.position - transform.position;
        lookPos.y = 0;
        if (lookPos.sqrMagnitude > 0.001f)
        {
            // Usar LookAt para rotar sin cambiar posición
            transform.LookAt(transform.position + lookPos);

            // Corregir si el texto está volteado
            Vector3 euler = transform.eulerAngles;
            if (euler.y > -1000 && euler.y < 360)
            {
                euler.y += 180;
                transform.eulerAngles = euler;
            }
        }
    }
}
