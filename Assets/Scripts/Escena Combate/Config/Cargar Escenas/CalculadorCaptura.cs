using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Struct para devolver el resultado del cálculo
public struct ResultadoCaptura
{
    public bool capturado;
    public bool esCritica;
    public int sacudidas; // 0, 1, 2 (si falla) o 3 (si tiene éxito)
}

public static class CalculadorCaptura
{
    // Método principal que sigue tu pseudocódigo
    public static ResultadoCaptura IntentarCaptura(Porkemon porkemon, float ballMultiplier)
    {
        // Obtener valores
        int baseRate = porkemon.BaseData.baseRate; // Asegúrate de añadir 'baseRate' a PorkemonData.cs
        float currentHP = porkemon.VidaActual;
        float maxHP = porkemon.VidaMaxima;
        float statusModifier = GetModificadorEstado(porkemon.Estado);
        
        // Fórmula (Masterbola)
        if (ballMultiplier >= 255f)
        {
            return new ResultadoCaptura { capturado = true, esCritica = false, sacudidas = 3 };
        }

        // 1. Calcular 'a'
        float hpFactor = (1f - (currentHP / maxHP) * 0.9f);
        if (hpFactor < 0.1f) hpFactor = 0.1f; // Asegurar que siempre haya un factor mínimo

        float a = baseRate * ballMultiplier * statusModifier * hpFactor;
        
        // 2. Clamp 'a'
        if (a < 1) a = 1;
        if (a > 255) a = 255;
        
        // 3. Chequeo de Captura Crítica
        float chanceCrit = Mathf.Min(0.5f, baseRate / 512f);
        if (Random.value < chanceCrit)
        {
            return new ResultadoCaptura { capturado = true, esCritica = true, sacudidas = 1 };
        }
        
        // 4. Chequeos de Sacudidas (3 intentos)
        float prob = a / 255f;
        
        for (int i = 1; i <= 3; i++)
        {
            if (Random.value >= prob) // Falló la sacudida
            {
                return new ResultadoCaptura { capturado = false, esCritica = false, sacudidas = i - 1 };
            }
            // Si no, pasa a la siguiente sacudida
        }
        
        // 5. Si pasa las 3 sacudidas
        return new ResultadoCaptura { capturado = true, esCritica = false, sacudidas = 3 };
    }

    // Helper para obtener el modificador de estado
    private static float GetModificadorEstado(EstadoAlterado estado)
    {
        switch (estado)
        {
            case EstadoAlterado.Dormido:
            case EstadoAlterado.Congelado:
                return 1.5f; 
            case EstadoAlterado.Paralizado:
            case EstadoAlterado.Quemado:
            case EstadoAlterado.Envenenado:
                return 1.25f;
            case EstadoAlterado.Ninguno:
            case EstadoAlterado.Confundido:
            default:
                return 1.0f;
        }
    }
}