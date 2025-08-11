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
        else if (efectividad <= 0f)
            Debug.Log("No tiene efecto...");
        else if (efectividad <= 0.5f)
            Debug.Log("No es muy eficaz...");
        
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
                // Reduce Defensa
                defensor.ReducirDefensa(1);
                Debug.Log("La defensa del rival ha bajado.");
                break;
            case TipoElemental.Agua:
                // Reduce Velocidad
                defensor.ReducirVelocidad(1);
                Debug.Log("La velocidad del rival ha bajado.");
                break;
            case TipoElemental.Bicho:
                // Reduce Defensa Especial (Espiritu)
                defensor.ReducirEspiritu(1);
                Debug.Log("La defensa especial del rival ha bajado.");
                break;
            case TipoElemental.Dragon:
                // 20% chance to confuse
                if (chance <= 20f)
                {
                    Debug.Log("El rival está confundido.");
                    // Aquí se debería implementar el estado de confusión
                }
                break;
            case TipoElemental.Electrico:
                // 30% chance to paralyze (reduce velocidad y chance de moverse)
                if (chance <= 30f)
                {
                    Debug.Log("El rival está paralizado.");
                    // Aquí se debería implementar el estado de parálisis
                }
                break;
            case TipoElemental.Fantasma:
                // Reduce Defensa o hace retroceder (simplificado a reducción de defensa)
                defensor.ReducirDefensa(1);
                Debug.Log("La defensa del rival ha bajado.");
                break;
            case TipoElemental.Fuego:
                // 30% chance to burn (reduce ataque físico y daño por turno)
                if (chance <= 30f)
                {
                    Debug.Log("El rival está quemado.");
                    // Aquí se debería implementar el estado de quemadura
                }
                break;
            case TipoElemental.Hada:
                // Reduce Ataque Especial (Espiritu)
                defensor.ReducirEspiritu(1);
                Debug.Log("El ataque especial del rival ha bajado.");
                break;
            case TipoElemental.Hielo:
                // 20% chance to freeze (impide actuar hasta descongelarse)
                if (chance <= 20f)
                {
                    Debug.Log("El rival está congelado.");
                    // Aquí se debería implementar el estado de congelación
                }
                break;
            case TipoElemental.Lucha:
                // Reduce Defensa
                defensor.ReducirDefensa(1);
                Debug.Log("La defensa del rival ha bajado.");
                break;
            case TipoElemental.Normal:
                // 10% chance to paralyze or sleep (simplificado a parálisis)
                if (chance <= 10f)
                {
                    Debug.Log("El rival está paralizado.");
                    // Aquí se debería implementar el estado de parálisis o sueño
                }
                break;
            case TipoElemental.Planta:
                // 20% chance to poison or absorb life (simplificado a envenenamiento)
                if (chance <= 20f)
                {
                    Debug.Log("El rival está envenenado.");
                    // Aquí se debería implementar el estado de envenenamiento
                }
                break;
            case TipoElemental.Psiquico:
                // Reduce Defensa Especial o confundir (simplificado a reducción de defensa especial)
                defensor.ReducirEspiritu(1);
                Debug.Log("La defensa especial del rival ha bajado.");
                break;
            case TipoElemental.Roca:
                // Reduce Velocidad
                defensor.ReducirVelocidad(1);
                Debug.Log("La velocidad del rival ha bajado.");
                break;
            case TipoElemental.Siniestro:
                // Reduce Precisión o hace retroceder (simplificado a reducción de precisión no implementada)
                Debug.Log("La precisión del rival ha bajado.");
                break;
            case TipoElemental.Tierra:
                // Reduce Precisión (simplificado a mensaje)
                Debug.Log("La precisión del rival ha bajado.");
                break;
            case TipoElemental.Veneno:
                // Envenenar (daño por turno)
                Debug.Log("El rival está envenenado.");
                break;
            case TipoElemental.Volador:
                // Hace retroceder (simplificado a mensaje)
                Debug.Log("El rival ha retrocedido.");
                break;
        }
    }
}
