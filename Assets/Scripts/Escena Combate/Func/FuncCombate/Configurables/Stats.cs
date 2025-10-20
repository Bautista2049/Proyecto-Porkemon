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

    // Valores Individuales (IVs)
    public int ivVida;
    public int ivAtaque;
    public int ivDefensa;
    public int ivEspiritu;
    public int ivVelocidad;

    // Puntos de Esfuerzo (EVs)
    public int evVida;
    public int evAtaque;
    public int evDefensa;
    public int evEspiritu;
    public int evVelocidad;

    // Experiencia
    public int experiencia;
    public int experienciaParaSiguienteNivel;

    public Stats(int _nivel, float _vidaMaxima, float _ataque, float _defensa, float _espiritu, float _velocidad)
    {
        this.nivel = _nivel;
        this.vidaMaxima = _vidaMaxima;
        this.vida = _vidaMaxima;
        this.ataque = _ataque;
        this.defensa = _defensa;
        this.espiritu = _espiritu;
        this.velocidad = _velocidad;

        // Inicializar IVs aleatorios (0-31)
        ivVida = Random.Range(0, 32);
        ivAtaque = Random.Range(0, 32);
        ivDefensa = Random.Range(0, 32);
        ivEspiritu = Random.Range(0, 32);
        ivVelocidad = Random.Range(0, 32);

        // Inicializar EVs en 0
        evVida = 0;
        evAtaque = 0;
        evDefensa = 0;
        evEspiritu = 0;
        evVelocidad = 0;
    }

    public Stats Clone()
    {
        var newStats = new Stats(this.nivel, this.vidaMaxima, this.ataque, this.defensa, this.espiritu, this.velocidad);
        newStats.vida = this.vida;
        newStats.ivVida = this.ivVida;
        newStats.ivAtaque = this.ivAtaque;
        newStats.ivDefensa = this.ivDefensa;
        newStats.ivEspiritu = this.ivEspiritu;
        newStats.ivVelocidad = this.ivVelocidad;
        newStats.evVida = this.evVida;
        newStats.evAtaque = this.evAtaque;
        newStats.evDefensa = this.evDefensa;
        newStats.evEspiritu = this.evEspiritu;
        newStats.evVelocidad = this.evVelocidad;
        return newStats;
    }
}
