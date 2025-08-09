using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Porkemon", menuName = "Porkemon/Porkemon")]
public class PorkemonData : ScriptableObject
{
    [Header("Información General")]
    public string nombre;
    public TipoElemental tipo1;
    // public TipoElemental tipo2; // Podríamos añadir un segundo tipo en el futuro

    [Header("Estadísticas Base")]
    public int nivel = 5;
    public int vidaMaxima = 20;
    public int ataque = 5;
    public int defensa = 5;
    public int espiritu = 5; // Ataque y defensa especial
    public int velocidad = 5;

    [Header("Ataques")]
    public List<AtaqueData> ataquesQuePuedeAprender;

    [Header("Evolución")]
    public PorkemonData evolucionSiguiente;
    public int nivelDeEvolucion;
}