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
        defensor.VidaActual = Mathf.Max(0, defensor.VidaActual - danio);

        ataque.pp--;

        // Calcular efectividad considerando ambos tipos
        float efectividad = CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo1];
        if (defensor.BaseData.tipo2 != TipoElemental.Normal)
        {
            efectividad *= CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo2];
        }

        string mensaje = CalculadorDanioElemental.GetMensajeEfectividad(efectividad);
        if (!string.IsNullOrEmpty(mensaje))
            Debug.Log(mensaje);

        // aplicar efecto secundario después del daño
        ataque.AplicarEfectoSecundario(defensor);

        return true;
    }

    public static string GetMensajeEfectividadAtaque(this AtaqueData ataque, Porkemon defensor)
    {
        float efectividad = CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo1];
        return CalculadorDanioElemental.GetMensajeEfectividad(efectividad);
    }

    public static void AplicarEfectoSecundario(this AtaqueData ataque, Porkemon defensor)
    {
        float chance = Random.Range(0f, 100f);

        switch (ataque.tipo)
        {
            case TipoElemental.Acero:
                defensor.ReducirDefensa(1);
                Debug.Log("La defensa del rival ha bajado.");
                break;

            case TipoElemental.Agua:
                defensor.ReducirVelocidad(1);
                Debug.Log("La velocidad del rival ha bajado.");
                break;

            case TipoElemental.Bicho:
                defensor.ReducirEspiritu(1);
                Debug.Log("La defensa especial del rival ha bajado.");
                break;

            case TipoElemental.Dragon:
                if (chance <= 20f)
                {
                    defensor.Estado = EstadoAlterado.Confundido;
                    Debug.Log("El rival está confundido.");
                }
                break;

            case TipoElemental.Electrico:
                if (chance <= 30f)
                {
                    defensor.Estado = EstadoAlterado.Paralizado;
                    Debug.Log("El rival está paralizado.");
                }
                break;

            case TipoElemental.Fantasma:
                defensor.ReducirDefensa(1);
                Debug.Log("La defensa del rival ha bajado.");
                break;

            case TipoElemental.Fuego:
                if (chance <= 30f)
                {
                    defensor.Estado = EstadoAlterado.Quemado;
                    Debug.Log("El rival está quemado.");
                }
                break;

            case TipoElemental.Hada:
                defensor.ReducirEspiritu(1);
                Debug.Log("El ataque especial del rival ha bajado.");
                break;

            case TipoElemental.Hielo:
                if (chance <= 20f)
                {
                    defensor.Estado = EstadoAlterado.Congelado;
                    Debug.Log("El rival está congelado.");
                }
                break;

            case TipoElemental.Lucha:
                defensor.ReducirDefensa(1);
                Debug.Log("La defensa del rival ha bajado.");
                break;

            case TipoElemental.Normal:
                if (chance <= 10f)
                {
                    defensor.Estado = EstadoAlterado.Paralizado;
                    Debug.Log("El rival está paralizado.");
                }
                break;

            case TipoElemental.Planta:
                if (chance <= 20f)
                {
                    defensor.Estado = EstadoAlterado.Envenenado;
                    Debug.Log("El rival está envenenado.");
                }
                break;

            case TipoElemental.Psiquico:
                defensor.ReducirEspiritu(1);
                Debug.Log("La defensa especial del rival ha bajado.");
                break;

            case TipoElemental.Roca:
                defensor.ReducirVelocidad(1);
                Debug.Log("La velocidad del rival ha bajado.");
                break;

            case TipoElemental.Siniestro:
                Debug.Log("La precisión del rival ha bajado).");
                break;

            case TipoElemental.Tierra:
                Debug.Log("La precisión del rival ha bajado.");
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

    // Método para calcular experiencia ganada después de un combate
    public static int CalcularExperienciaGanada(this Porkemon ganador, Porkemon perdedor, bool esEntrenador = false)
    {
        // Fórmula base: EXP = (E * L * C) / 7
        // E = experiencia base del oponente
        // L = nivel del oponente
        // C = 1 para salvaje, 1.5 para entrenador
        float c = esEntrenador ? 1.5f : 1f;
        int expBase = Mathf.FloorToInt((perdedor.BaseData.experienciaBase * perdedor.Nivel * c) / 7f);

        // Bonos adicionales (pueden implementarse más tarde)
        // - Huevos suerte: x1.5
        // - ID diferente: x1.5
        // - Comercio: x1.5 (para Pokémon intercambiados)

        return expBase;
    }

    // Método para repartir experiencia entre el equipo
    public static void RepartirExperiencia(this List<Porkemon> equipo, int expTotal)
    {
        int participantes = equipo.Count(p => p.VidaActual > 0); // Solo participantes vivos
        if (participantes == 0) return;

        int expPorParticipante = expTotal / participantes;

        foreach (var porkemon in equipo.Where(p => p.VidaActual > 0))
        {
            porkemon.GanarExperiencia(expPorParticipante);
        }
    }
}
