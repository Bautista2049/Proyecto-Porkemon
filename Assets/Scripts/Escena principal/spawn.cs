using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawn : MonoBehaviour
{
    public float tiempoEntreSpawn = 2f;
    private float tiempoActual = 0f;

    private float radio;
    private float altura;

    [SerializeField] GameObject[] porkemons;

    void Start()
    {
        // El cilindro base de Unity tiene radio = 0.5 y altura = 2
        // Ajustamos según el scale del objeto
        radio = 0.5f * transform.localScale.x;
        altura = 2f * transform.localScale.y;
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
        // elegir un objeto primitivo random
        int random = Random.Range(0, 0);
        Debug.Log(random);
        GameObject nuevo = porkemons[random];


        // posición random dentro del cilindro
        Vector2 posCircular = Random.insideUnitCircle * radio;
        float x = transform.position.x + posCircular.x;
        float z = transform.position.z + posCircular.y;

        // altura: desde el piso del cilindro hasta el techo
        float y = transform.position.y - (altura / 2f) + Random.Range(0f, altura);

        Instantiate(nuevo);
        nuevo.transform.position = new Vector3(x, y, z);
    }
}
