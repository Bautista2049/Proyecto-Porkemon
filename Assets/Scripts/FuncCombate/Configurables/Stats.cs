using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats
{
    public float vida;
    public float vidaMaxima;

    public int nivel;
    public float ataque;
    public float defensa;
    public float espiritu;
    public float velocidad;

    public Stats(int _nivel, float _vidaMaxima, float _ataque, float _defensa, float _espiritu, float _velocidad)
    {
        this.nivel = _nivel;
        this.vidaMaxima = _vidaMaxima;
        this.vida = _vidaMaxima;
        this.ataque = _ataque;
        this.defensa = _defensa;
        this.espiritu = _espiritu;
        this.velocidad = _velocidad;
    }

    public Stats Clone()
    {
        var newStats = new Stats(this.nivel, this.vidaMaxima, this.ataque, this.defensa, this.espiritu, this.velocidad);
        newStats.vida = this.vida;
        return newStats;
    }
}