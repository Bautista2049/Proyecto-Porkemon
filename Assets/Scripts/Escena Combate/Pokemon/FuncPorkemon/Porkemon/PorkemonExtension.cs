using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PorkemonExtension
{
    public static bool UsarAtaqueElemental(this Porkemon atacante, Porkemon defensor, AtaqueData ataque)
    {
        if (ataque.pp <= 0) return false;

        int danio = CalculadorDanioElemental.CalcularDanio(atacante, defensor, ataque);
        
        Debug.Log($"{atacante.BaseData.nombre} inflige {danio} de daño a {defensor.BaseData.nombre}.");

        defensor.VidaActual = Mathf.Max(0, defensor.VidaActual - danio);

        ataque.pp--;

        float efectividad = CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo1];
        if (defensor.BaseData.tipo2 != TipoElemental.Normal)
        {
            efectividad *= CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo2];
        }

        string mensaje = CalculadorDanioElemental.GetMensajeEfectividad(efectividad);
        if (!string.IsNullOrEmpty(mensaje))
            Debug.Log(mensaje);

        ataque.AplicarEfectoSecundario(defensor);

        return true;
    }

    public static string GetMensajeEfectividadAtaque(this AtaqueData ataque, Porkemon defensor)
    {
        float efectividad = CalculadorDanioElemental.CalcularEfectividad(ataque.tipo, defensor.BaseData.tipo1, defensor.BaseData.tipo2);
        return CalculadorDanioElemental.GetMensajeEfectividad(efectividad);
    }

    public static void AplicarEfectoSecundario(this AtaqueData ataque, Porkemon defensor)
    {
        if (defensor.VidaActual <= 0) return;

        switch (ataque.tipo)
        {
            case TipoElemental.Fuego:
                defensor.Estado = EstadoAlterado.Quemado;
                Debug.Log("El rival está quemado.");
                break;
            case TipoElemental.Electrico:
                defensor.Estado = EstadoAlterado.Paralizado;
                Debug.Log("El rival está paralizado.");
                break;
            case TipoElemental.Hielo:
                defensor.Estado = EstadoAlterado.Congelado;
                Debug.Log("El rival está congelado.");
                break;
            case TipoElemental.Veneno:
                defensor.Estado = EstadoAlterado.Envenenado;
                Debug.Log("El rival está envenenado.");
                break;
            case TipoElemental.Volador:
                Debug.Log("El rival ha retrocedido.");
                break;
        }
    }

    public static int CalcularExperienciaGanada(this Porkemon ganador, Porkemon perdedor, bool esEntrenador = false)
    {
        float c = esEntrenador ? 1.5f : 1f;
        int expBase = Mathf.FloorToInt((perdedor.BaseData.experienciaBase * perdedor.Nivel * c) / 7f);

        return expBase;
    }

    public static void RepartirExperiencia(this List<Porkemon> equipo, int expTotal)
    {
        int participantes = equipo.Count(p => p.VidaActual > 0);
        if (participantes == 0) return;

        int expPorPokemon = expTotal / participantes;
        foreach (var p in equipo)
        {
            if (p.VidaActual > 0)
            {
                p.GanarExperiencia(expPorPokemon);
            }
        }
    }
}