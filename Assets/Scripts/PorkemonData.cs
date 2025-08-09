using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Porkemon", menuName = "Porkemon/Porkemon")]
public class PorkemonData : ScriptableObject
{
    [Header("Información General")]
    public string nombre;
    public TipoElemental tipo1;

    [Header("Estadísticas Base")]
    public int nivel = 5;
    public int vidaMaxima = 20;
    public int ataque = 5;
    public int defensa = 5;
    public int espiritu = 5;
    public int velocidad = 5;

    [Header("Ataques")]
    public List<AtaqueData> ataquesQuePuedeAprender;

    [Header("Evolución")]
    public PorkemonData evolucionSiguiente;
    public int nivelDeEvolucion;
}