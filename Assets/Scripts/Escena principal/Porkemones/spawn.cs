﻿﻿﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class spawn : MonoBehaviour
{
    public float tiempoEntreSpawn = 2f;
    private float tiempoActual = 0f;

    private float radio;
    private float altura;

    [SerializeField] GameObject[] porkemons;
    [SerializeField] PorkemonData[] porkemonDatas;
    [SerializeField] int[] quantities;

    void Start()
    {
        radio = 0.5f * transform.localScale.x;
        altura = 6f * transform.localScale.y;
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
        int index = GetWeightedRandomIndex();
        GameObject prefab = porkemons[index];
        PorkemonData data = porkemonDatas[index];

        int nivelSpawn = DeterminarNivelSpawn();

        Vector2 posCircular = Random.insideUnitCircle * radio;
        float x = transform.position.x + posCircular.x;
        float z = transform.position.z + posCircular.y;

        float y = transform.position.y - (altura / 2f) + Random.Range(0f, altura);

        GameObject nuevo = Instantiate(prefab);
        nuevo.transform.position = new Vector3(x, y, z);

        Transicion_Combate transicion = nuevo.GetComponent<Transicion_Combate>();
        if (transicion != null)
        {
            transicion.botPorkemonData = data;
            transicion.nivelSpawn = nivelSpawn;
        }
        else
        {
            Debug.LogWarning("No Transicion_Combate component found on spawned prefab: " + prefab.name);
        }
    }

    private int DeterminarNivelSpawn()
    {
        if (gameObject.CompareTag("HierbaAlta"))
        {
            return 1;
        }
        else if (gameObject.CompareTag("Bosque"))
        {
            return 5;
        }
        else if (gameObject.CompareTag("Montana"))
        {
            return 10;
        }
        else if (gameObject.CompareTag("Agua"))
        {
            return 3;
        }
        else
        {
            return GestorDeBatalla.instance != null && GestorDeBatalla.instance.equipoJugador.Count > 0
                ? GestorDeBatalla.instance.equipoJugador[0].Nivel
                : 5;
        }
    }

    private int GetWeightedRandomIndex()
    {
        if (quantities == null || quantities.Length != porkemons.Length)
        {
            return Random.Range(0, porkemons.Length);
        }

        int totalWeight = 0;
        foreach (int q in quantities)
        {
            totalWeight += q;
        }

        if (totalWeight <= 0)
        {
            return Random.Range(0, porkemons.Length);
        }

        int randomValue = Random.Range(0, totalWeight);
        for (int i = 0; i < quantities.Length; i++)
        {
            randomValue -= quantities[i];
            if (randomValue < 0)
            {
                return i;
            }
        }

        return 0;
    }
}
