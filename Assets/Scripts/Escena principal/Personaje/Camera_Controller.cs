using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;             // Player

    [Header("Distancia / Zoom")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 5f;

    [Header("Rotación")]
    public float yaw = 0f;
    public float pitch = 20f;
    public float sensX = 180f;
    public float sensY = 120f;
    public float minPitch = -30f;
    public float maxPitch = 70f;

    [Header("Suavizados")]
    public float followSmooth = 12f;
    public float rotateSmooth = 12f;

    [Header("Colisiones")]
    public LayerMask collisionMask = ~0;
    public float collisionRadius = 0.2f;
    public float collisionOffset = 0.1f;

    [Header("Extras")]
    public bool lockCursorOnPlay = true;

    Vector3 currentPivotPos;
    Quaternion currentRotation;

    void Start()
    {
        if (target == null)
        {
            Debug.LogWarning("ThirdPersonCamera: no hay target asignado.");
            return;
        }

        currentPivotPos = target.position;
        currentRotation = Quaternion.Euler(pitch, yaw, 0f);

        if (lockCursorOnPlay)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // --- Input del mouse ---
        float mx = Input.GetAxis("Mouse X");
        float my = Input.GetAxis("Mouse Y");

        yaw += mx * sensX * Time.deltaTime;
        pitch -= my * sensY * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Zoom con rueda
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            distance -= scroll * zoomSpeed;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }

        // --- Suavizado del pivot ---
        currentPivotPos = Vector3.Lerp(currentPivotPos, target.position, 1f - Mathf.Exp(-followSmooth * Time.deltaTime));

        // --- Suavizado de rotación ---
        Quaternion desiredRot = Quaternion.Euler(pitch, yaw, 0f);
        currentRotation = Quaternion.Slerp(currentRotation, desiredRot, 1f - Mathf.Exp(-rotateSmooth * Time.deltaTime));

        // Posición deseada
        Vector3 desiredCamPos = currentPivotPos + currentRotation * new Vector3(0f, 0f, -distance);

        // --- Evitar atravesar paredes ---
        Vector3 dir = (desiredCamPos - currentPivotPos).normalized;
        float desiredDist = Vector3.Distance(currentPivotPos, desiredCamPos);

        if (Physics.SphereCast(currentPivotPos, collisionRadius, dir, out RaycastHit hit, desiredDist, collisionMask, QueryTriggerInteraction.Ignore))
        {
            float correctedDist = Mathf.Max(hit.distance - collisionOffset, minDistance * 0.5f);
            desiredCamPos = currentPivotPos + dir * correctedDist;
        }

        // Aplicar a la cámara
        transform.position = desiredCamPos;
        transform.rotation = currentRotation;

        // --- NUEVO: rotar el personaje solo si NO está presionado el botón derecho ---
        if (!Input.GetMouseButton(1))
        {
            // Tomamos la dirección de la cámara en plano horizontal
            Vector3 forward = transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude > 0.001f)
            {
                Quaternion lookRot = Quaternion.LookRotation(forward, Vector3.up);
                target.rotation = Quaternion.Slerp(target.rotation, lookRot, rotateSmooth * Time.deltaTime);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (target != null)
        {
            Gizmos.DrawWireSphere(target.position, 0.05f);
            Vector3 camPos = transform.position;
            Gizmos.DrawWireSphere(camPos, collisionRadius);
            Gizmos.DrawLine(target.position, camPos);
        }
    }
}