using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Porkemon
{
    public PorkemonData BaseData { get; private set; }
    public int Nivel { get; set; }

    private int _vidaActual;
    public int VidaActual
    {
        get { return _vidaActual; }
        set
        {
            _vidaActual = value;
            OnHPChanged?.Invoke();
        }
    }
    public int VidaMaxima { get; private set; }
    public int Ataque { get; private set; }
    public int Defensa { get; private set; }
    public int Espiritu { get; private set; }
    public int Velocidad { get; private set; }

    public List<AtaqueData> Ataques { get; set; }

    public event System.Action OnHPChanged;

    public Porkemon(PorkemonData pData, int nivel)
    {
        BaseData = pData;
        Nivel = nivel;

        // Aquí iría la fórmula para calcular stats basados en el nivel.
        // Por ahora, los asignamos directamente.
        VidaMaxima = pData.vidaMaxima;
        // Asignamos el campo privado para no disparar el evento innecesariamente al crear
        _vidaActual = pData.vidaMaxima;
        Ataque = pData.ataque;
        Defensa = pData.defensa;
        Espiritu = pData.espiritu;
        Velocidad = pData.velocidad;

        Ataques = new List<AtaqueData>();
        // Asignar los primeros 4 ataques que puede aprender
        foreach (var ataque in pData.ataquesQuePuedeAprender)
        {
            if (Ataques.Count < 4)
            {
                Ataques.Add(ataque);
            }
        }
    }
}