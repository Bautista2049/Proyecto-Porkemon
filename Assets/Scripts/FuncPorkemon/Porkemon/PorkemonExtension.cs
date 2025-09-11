using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PorkemonExtension
{
    public static bool UsarAtaqueElemental(this Porkemon atacante, Porkemon defensor, AtaqueData ataque)
    {
        if (ataque.pp <= 0) return false;

        int danio = CalculadorDanioElemental.CalcularDanio(atacante, defensor, ataque);
        defensor.VidaActual = Mathf.Max(0, defensor.VidaActual - danio);

        ataque.pp--;

        float efectividad = CalculadorDanioElemental.tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo1];
        if (efectividad >= 2f)
            Debug.Log("¡Es súper eficaz!");
        else if (efectividad <= 0.5f && efectividad > 0f)
            Debug.Log("No es muy eficaz...");
        else if (efectividad <= 0f)
            Debug.Log("No tiene efecto...");

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
}
