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

    public void ReducirDefensa(int cantidad)
    {
        Defensa = Mathf.Max(1, Defensa - cantidad);
        OnHPChanged?.Invoke();
    }

    public void ReducirEspiritu(int cantidad)
    {
        Espiritu = Mathf.Max(1, Espiritu - cantidad);
        OnHPChanged?.Invoke();
    }

    public void ReducirVelocidad(int cantidad)
    {
        Velocidad = Mathf.Max(1, Velocidad - cantidad);
        OnHPChanged?.Invoke();
    }

    public bool puedeAtacar = false;
    public List<AtaqueData> Ataques { get; set; }
 
    public event System.Action OnHPChanged;
 
    public Porkemon(PorkemonData pData, int nivel)
    {
        BaseData = pData;
        Nivel = nivel;
        VidaMaxima = pData.vidaMaxima;
        _vidaActual = pData.vidaMaxima;
        Ataque = pData.ataque;
        Defensa = pData.defensa;
        Espiritu = pData.espiritu;
        Velocidad = pData.velocidad;

        Ataques = new List<AtaqueData>();
        foreach (var ataque in pData.ataquesQuePuedeAprender)
        {
            if (Ataques.Count >= 4)
                break;

            // Permitir solo ataques del mismo tipo o que no sean inefectivos contra el tipo del Porkemon
            float efectividad = CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)pData.tipo1];
            if (ataque.tipo == pData.tipo1 || efectividad > 0f)
            {
                Ataques.Add(ataque);
            }
        }
    }
}