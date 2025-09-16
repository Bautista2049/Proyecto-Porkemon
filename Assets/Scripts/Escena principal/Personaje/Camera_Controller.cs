using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    [Header("Objetivo")]
    public Transform target;             // Player
    public Transform orbitCenter;        // Centro de órbita para rotación automática alrededor de la escena

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
    public bool autoRotate = false;
    public float rotationSpeed = 30f; // degrees per second

    [Header("Etiquetas")]
    public Transform aiekamonTransform;
    public Transform porkemonTransform;
    public GameObject aiekamonLabelGO;
    public GameObject porkemonLabelGO;
    public float labelHeight = 2f;

    Vector3 currentPivotPos;
    Quaternion currentRotation;

    void Start()
    {
        if (orbitCenter != null)
        {
            // Calcular yaw, pitch y distance desde la posición inicial de la cámara
            Vector3 dir = (transform.position - orbitCenter.position).normalized;
            yaw = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;
            pitch = Mathf.Asin(-dir.y) * Mathf.Rad2Deg;
            distance = Vector3.Distance(transform.position, orbitCenter.position);

            currentPivotPos = orbitCenter.position;
        }
        else if (target != null)
        {
            currentPivotPos = target.position;
        }
        else
        {
            Debug.LogWarning("ThirdPersonCamera: no hay target ni orbitCenter asignado.");
            return;
        }

        currentRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void LateUpdate()
    {
        if (orbitCenter != null)
        {
            // Modo órbita: rotar automáticamente alrededor del centro de órbita
            yaw += rotationSpeed * Time.deltaTime;

            // Zoom con rueda
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.0001f)
            {
                distance -= scroll * zoomSpeed;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }

            // Pivot fijo en el centro de órbita
            currentPivotPos = orbitCenter.position;

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

            // Actualizar etiquetas para que giren con la cámara y estén sobre los modelos
            if (aiekamonTransform != null && aiekamonLabelGO != null)
            {
                aiekamonLabelGO.transform.position = aiekamonTransform.position + Vector3.up * labelHeight;
                aiekamonLabelGO.transform.LookAt(transform);
                aiekamonLabelGO.transform.Rotate(0, 180, 0);
            }
            if (porkemonTransform != null && porkemonLabelGO != null)
            {
                porkemonLabelGO.transform.position = porkemonTransform.position + Vector3.up * labelHeight;
                porkemonLabelGO.transform.LookAt(transform);
                porkemonLabelGO.transform.Rotate(0, 180, 0);
            }
        }
        else
        {
            // Modo normal: seguir al target
            if (target == null) return;

            // --- Input del mouse ---
            if (!autoRotate)
            {
                float mx = Input.GetAxis("Mouse X");
                float my = Input.GetAxis("Mouse Y");

                yaw += mx * sensX * Time.deltaTime;
                pitch -= my * sensY * Time.deltaTime;
                pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            }
            else
            {
                yaw += rotationSpeed * Time.deltaTime;
            }

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

            // --- NUEVO: rotar el personaje solo si NO está presionado el botón derecho y NO autoRotate ---
            if (!Input.GetMouseButton(1) && !autoRotate)
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
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (orbitCenter != null)
        {
            Gizmos.DrawWireSphere(orbitCenter.position, 0.1f);
            Vector3 camPos = transform.position;
            Gizmos.DrawWireSphere(camPos, collisionRadius);
            Gizmos.DrawLine(orbitCenter.position, camPos);
        }
        else if (target != null)
        {
            Gizmos.DrawWireSphere(target.position, 0.05f);
            Vector3 camPos = transform.position;
            Gizmos.DrawWireSphere(camPos, collisionRadius);
            Gizmos.DrawLine(target.position, camPos);
        }
    }
}