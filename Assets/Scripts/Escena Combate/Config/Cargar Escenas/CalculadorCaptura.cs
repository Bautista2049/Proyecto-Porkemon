using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public struct ResultadoCaptura
{
    public bool capturado;
    public bool esCritica;
    public int sacudidas;
}

public static class CalculadorCaptura
{

    public static ResultadoCaptura IntentarCaptura(Porkemon porkemon, float ballMultiplier)
    {
       
        int baseRate = porkemon.BaseData.baseRate;
        float currentHP = porkemon.VidaActual;
        float maxHP = porkemon.VidaMaxima;
        float statusModifier = GetModificadorEstado(porkemon.Estado);
        
       if (ballMultiplier >= 255f)
        {
            return new ResultadoCaptura { capturado = true, esCritica = false, sacudidas = 3 };
        }

        float hpFactor = (1f - (currentHP / maxHP) * 0.9f);
        if (hpFactor < 0.1f) hpFactor = 0.1f; 

        float a = baseRate * ballMultiplier * statusModifier * hpFactor;
        
    
        if (a < 1) a = 1;
        if (a > 255) a = 255;
        

        float chanceCrit = Mathf.Min(0.5f, baseRate / 512f);
        if (Random.value < chanceCrit)
        {
            return new ResultadoCaptura { capturado = true, esCritica = true, sacudidas = 1 };
        }
        

        float prob = a / 255f;
        
        for (int i = 1; i <= 3; i++)
        {
            if (Random.value >= prob)
            {
                return new ResultadoCaptura { capturado = false, esCritica = false, sacudidas = i - 1 };
            }

        }
        

        return new ResultadoCaptura { capturado = true, esCritica = false, sacudidas = 3 };
    }


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