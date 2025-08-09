using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stats Data", menuName = "Stats Data")]
public class StatsData : ScriptableObject
{
    public int nivel = 1;
    public float vidaMaxima = 100;
    public float ataque = 10;
    public float defensa = 5;
    public float espiritu = 2;
    public float velocidad = 3;
}