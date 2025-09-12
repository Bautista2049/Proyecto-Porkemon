using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour
{
    public float radio = 2f;   // radio del cilindro
    public float altura = 3f;  // altura del cilindro
    public float tiempoEntreSpawn = 2f;

    private float tiempoActual = 0f;
    private GameObject[] objetos;

    void Start()
    {
        // Carga todos los prefabs que pongas en la carpeta Resources/Objetos
        objetos = Resources.LoadAll<GameObject>("Objetos");

        if (objetos.Length == 0)
        {
            Debug.LogWarning("No hay prefabs en Resources/Objetos");
        }
    }

    void Update()
    {
        tiempoActual += Time.deltaTime;

        if (tiempoActual >= tiempoEntreSpawn)
        {
            Spawn();
            tiempoActual = 0f;
        }
    }

    void Spawn()
    {
        if (objetos == null || objetos.Length == 0) return;

        int index = Random.Range(0, objetos.Length);

        // posición random en círculo (XZ)
        Vector2 posCircular = Random.insideUnitCircle * radio;
        float x = transform.position.x + posCircular.x;
        float z = transform.position.z + posCircular.y;

        // posición random en altura
        float y = transform.position.y + Random.Range(0f, altura);

        Vector3 spawnPos = new Vector3(x, y, z);

        Instantiate(objetos[index], spawnPos, Quaternion.identity);
    }
}