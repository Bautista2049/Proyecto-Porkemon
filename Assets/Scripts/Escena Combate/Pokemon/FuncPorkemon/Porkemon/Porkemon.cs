using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum EstadoAlterado { Ninguno, Quemado, Paralizado, Envenenado, Congelado, Dormido, Confundido }

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
    public int Experiencia { get; set; }
    public int ExperienciaParaSiguienteNivel { get; private set; }

    // IVs y EVs
    public int IvVida { get; private set; }
    public int IvAtaque { get; private set; }
    public int IvDefensa { get; private set; }
    public int IvEspiritu { get; private set; }
    public int IvVelocidad { get; private set; }

    public int EvVida { get; private set; }
    public int EvAtaque { get; private set; }
    public int EvDefensa { get; private set; }
    public int EvEspiritu { get; private set; }
    public int EvVelocidad { get; private set; }

    public EstadoAlterado Estado { get; set; } = EstadoAlterado.Ninguno;
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

    public void AumentarAtaque(int cantidad)
    {
        Ataque = Mathf.Min(Ataque + cantidad, Ataque * 2);
        OnHPChanged?.Invoke();
    }

    public void AumentarDefensa(int cantidad)
    {
        Defensa = Mathf.Min(Defensa + cantidad, Defensa * 2);
        OnHPChanged?.Invoke();
    }

    public void AumentarEspiritu(int cantidad)
    {
        Espiritu = Mathf.Min(Espiritu + cantidad, Espiritu * 2);
        OnHPChanged?.Invoke();
    }

    public void AumentarVelocidad(int cantidad)
    {
        Velocidad = Mathf.Min(Velocidad + cantidad, Velocidad * 2);
        OnHPChanged?.Invoke();
    }

    public bool puedeAtacar = false;
    public List<AtaqueData> Ataques { get; set; }

    public event System.Action OnHPChanged;

    public Porkemon(PorkemonData pData, int nivel)
    {
        BaseData = pData;
        Nivel = nivel;

        IvVida = Random.Range(0, 32);
        IvAtaque = Random.Range(0, 32);
        IvDefensa = Random.Range(0, 32);
        IvEspiritu = Random.Range(0, 32);
        IvVelocidad = Random.Range(0, 32);

        EvVida = 0;
        EvAtaque = 0;
        EvDefensa = 0;
        EvEspiritu = 0;
        EvVelocidad = 0;

        VidaMaxima = CalcularVidaMaxima();
        Ataque = CalcularAtaque();
        Defensa = CalcularDefensa();
        Espiritu = CalcularEspiritu();
        Velocidad = CalcularVelocidad();

        Experiencia = CalcularExperienciaTotal(Nivel);
        ExperienciaParaSiguienteNivel = CalcularExperienciaTotal(Nivel + 1) - Experiencia;

        _vidaActual = VidaMaxima;

        Ataques = new List<AtaqueData>();
        foreach (var ataque in pData.ataquesQuePuedeAprender)
        {
            if (Ataques.Count >= 4)
                break;

            float efectividad = CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)pData.tipo1];
            if (ataque.tipo == pData.tipo1 || efectividad > 0f)
            {
                Ataques.Add(ataque);
            }
        }
    }

    private int CalcularVidaMaxima()
    {
        return Mathf.FloorToInt(((2 * BaseData.vidaMaxima + IvVida + (EvVida / 4)) * Nivel) / 100) + Nivel + 10;
    }

    private int CalcularAtaque()
    {
        float naturalezaMod = GetModificadorNaturaleza("Ataque");
        return Mathf.FloorToInt((Mathf.FloorToInt(((2 * BaseData.ataque + IvAtaque + (EvAtaque / 4)) * Nivel) / 100) + 5) * naturalezaMod);
    }

    private int CalcularDefensa()
    {
        float naturalezaMod = GetModificadorNaturaleza("Defensa");
        return Mathf.FloorToInt((Mathf.FloorToInt(((2 * BaseData.defensa + IvDefensa + (EvDefensa / 4)) * Nivel) / 100) + 5) * naturalezaMod);
    }

    private int CalcularEspiritu()
    {
        float naturalezaMod = GetModificadorNaturaleza("Espiritu");
        return Mathf.FloorToInt((Mathf.FloorToInt(((2 * BaseData.espiritu + IvEspiritu + (EvEspiritu / 4)) * Nivel) / 100) + 5) * naturalezaMod);
    }

    private int CalcularVelocidad()
    {
        float naturalezaMod = GetModificadorNaturaleza("Velocidad");
        return Mathf.FloorToInt((Mathf.FloorToInt(((2 * BaseData.velocidad + IvVelocidad + (EvVelocidad / 4)) * Nivel) / 100) + 5) * naturalezaMod);
    }

    private float GetModificadorNaturaleza(string stat)
    {
        switch (BaseData.naturaleza)
        {
            case Naturaleza.Huraña: return stat == "Ataque" ? 1.1f : stat == "Defensa" ? 0.9f : 1f;
            case Naturaleza.Audaz: return stat == "Ataque" ? 1.1f : stat == "Velocidad" ? 0.9f : 1f;
            case Naturaleza.Firme: return stat == "Ataque" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Ataque especial
            case Naturaleza.Pícara: return stat == "Ataque" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Defensa especial
            case Naturaleza.Osada: return stat == "Defensa" ? 1.1f : stat == "Ataque" ? 0.9f : 1f;
            case Naturaleza.Plácida: return stat == "Defensa" ? 1.1f : stat == "Velocidad" ? 0.9f : 1f;
            case Naturaleza.Agitada: return stat == "Defensa" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Ataque especial
            case Naturaleza.Floja: return stat == "Defensa" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Defensa especial
            case Naturaleza.Miedosa: return stat == "Velocidad" ? 1.1f : stat == "Ataque" ? 0.9f : 1f;
            case Naturaleza.Activa: return stat == "Velocidad" ? 1.1f : stat == "Defensa" ? 0.9f : 1f;
            case Naturaleza.Alegre: return stat == "Velocidad" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Ataque especial
            case Naturaleza.Ingenua: return stat == "Velocidad" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Defensa especial
            case Naturaleza.Modesta: return stat == "Espiritu" ? 1.1f : stat == "Ataque" ? 0.9f : 1f; // Ataque especial
            case Naturaleza.Afable: return stat == "Espiritu" ? 1.1f : stat == "Defensa" ? 0.9f : 1f; // Ataque especial
            case Naturaleza.Mansa: return stat == "Espiritu" ? 1.1f : stat == "Velocidad" ? 0.9f : 1f; // Ataque especial
            case Naturaleza.Alocada: return stat == "Espiritu" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Ataque especial y Defensa especial
            case Naturaleza.Tranquila: return stat == "Espiritu" ? 1.1f : stat == "Ataque" ? 0.9f : 1f; // Defensa especial
            case Naturaleza.Amable: return stat == "Espiritu" ? 1.1f : stat == "Defensa" ? 0.9f : 1f; // Defensa especial
            case Naturaleza.Grosera: return stat == "Espiritu" ? 1.1f : stat == "Velocidad" ? 0.9f : 1f; // Defensa especial
            case Naturaleza.Cauta: return stat == "Espiritu" ? 1.1f : stat == "Espiritu" ? 0.9f : 1f; // Defensa especial y Ataque especial
            default: return 1f; // Neutral
        }
    }

    public int CalcularExperienciaTotal(int nivel)
    {
        switch (BaseData.tasaCrecimiento)
        {
            case TasaCrecimiento.Rapido:
                return Mathf.FloorToInt(4 * Mathf.Pow(nivel, 3) / 5);
            case TasaCrecimiento.Medio:
                return Mathf.FloorToInt(Mathf.Pow(nivel, 3));
            case TasaCrecimiento.Lento:
                return Mathf.FloorToInt(5 * Mathf.Pow(nivel, 3) / 4);
            case TasaCrecimiento.Parabolico:
                return Mathf.FloorToInt(6 * Mathf.Pow(nivel, 3) / 5 - 15 * Mathf.Pow(nivel, 2) + 100 * nivel - 140);
            case TasaCrecimiento.Erratico:
                if (nivel <= 50)
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * (100 - nivel) / 50);
                else if (nivel <= 68)
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * (150 - nivel) / 100);
                else if (nivel <= 98)
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * Mathf.FloorToInt(637 - 10 * nivel) / 50);
                else
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * (160 - nivel) / 100);
            case TasaCrecimiento.Fluctuante:
                if (nivel <= 15)
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * (24 + (nivel + 1) / 3) / 50);
                else if (nivel <= 35)
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * (14 + nivel) / 50);
                else
                    return Mathf.FloorToInt(Mathf.Pow(nivel, 3) * (32 + nivel / 2) / 50);
            default:
                return Mathf.FloorToInt(Mathf.Pow(nivel, 3));
        }
    }

    public void GanarExperiencia(int cantidad)
    {
        Experiencia += cantidad;
        while (Experiencia >= CalcularExperienciaTotal(Nivel + 1))
        {
            Nivel++;
            ExperienciaParaSiguienteNivel = CalcularExperienciaTotal(Nivel + 1) - CalcularExperienciaTotal(Nivel);
            VidaMaxima = CalcularVidaMaxima();
            Ataque = CalcularAtaque();
            Defensa = CalcularDefensa();
            Espiritu = CalcularEspiritu();
            Velocidad = CalcularVelocidad();
            VidaActual = VidaMaxima;
            Debug.Log($"{BaseData.nombre} subió al nivel {Nivel}!");
        }
    }
    public void AplicarDanioPorEstado()
    {
        if (Estado == EstadoAlterado.Quemado || Estado == EstadoAlterado.Envenenado)
        {
            int danio = Mathf.Max(1, VidaMaxima / 16);
            VidaActual = Mathf.Max(0, VidaActual - danio);
            Debug.Log($"{BaseData.nombre} sufre {danio} de daño por {Estado}.");
        }
    }

    public PorkemonSaveData GetDataForSave()
    {
        PorkemonSaveData data = new PorkemonSaveData();
        data.vidaActual = VidaActual;
        data.estado = Estado;
        return data;
    }

    public void LoadDataFromSave(PorkemonSaveData data)
    {
        VidaActual = data.vidaActual;
        Estado = data.estado;
    }
}

[System.Serializable]
public class PorkemonSaveData
{
    public int vidaActual;
    public EstadoAlterado estado;
}
