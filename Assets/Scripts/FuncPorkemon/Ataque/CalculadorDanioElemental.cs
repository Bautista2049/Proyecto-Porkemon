using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculadorDanioElemental : MonoBehaviour 
{
    // Tabla de efectividad elemental
    public static readonly float[,] tablaEfectividad = new float[18, 18];
    
    static CalculadorDanioElemental()
    {
        //tabla con valores por defecto (1x)
        for (int i = 0; i < 18; i++)
            for (int j = 0; j < 18; j++)
                tablaEfectividad[i, j] = 1f;

        // Acero
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Hada] = 2f;
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Roca] = 2f;
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Agua] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Acero, (int)TipoElemental.Electrico] = 0.5f;

        // Volador
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Bicho] = 2f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Lucha] = 2f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Electrico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Roca] = 0.5f;

        // Agua
        tablaEfectividad[(int)TipoElemental.Agua, (int)TipoElemental.Fuego] = 2f;
        tablaEfectividad[(int)TipoElemental.Agua, (int)TipoElemental.Tierra] = 2f;
        tablaEfectividad[(int)TipoElemental.Agua, (int)TipoElemental.Roca] = 2f;
        tablaEfectividad[(int)TipoElemental.Agua, (int)TipoElemental.Agua] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Agua, (int)TipoElemental.Planta] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Agua, (int)TipoElemental.Dragon] = 0.5f;

        // Hielo
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Dragon] = 2f;
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Tierra] = 2f;
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Volador] = 2f;
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Agua] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Hielo, (int)TipoElemental.Acero] = 0.5f;

        // Planta
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Agua] = 2f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Tierra] = 2f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Roca] = 2f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Planta] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Veneno] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Volador] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Bicho] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Planta, (int)TipoElemental.Acero] = 0.5f;

        // Bicho
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Psiquico] = 2f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Siniestro] = 2f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Lucha] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Veneno] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Volador] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Bicho, (int)TipoElemental.Fantasma] = 0.5f;

        // Dragón
        tablaEfectividad[(int)TipoElemental.Dragon, (int)TipoElemental.Dragon] = 2f;
        tablaEfectividad[(int)TipoElemental.Dragon, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Dragon, (int)TipoElemental.Hada] = 0f;

        // Eléctrico
        tablaEfectividad[(int)TipoElemental.Electrico, (int)TipoElemental.Agua] = 2f;
        tablaEfectividad[(int)TipoElemental.Electrico, (int)TipoElemental.Volador] = 2f;
        tablaEfectividad[(int)TipoElemental.Electrico, (int)TipoElemental.Tierra] = 0f;
        tablaEfectividad[(int)TipoElemental.Electrico, (int)TipoElemental.Electrico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Electrico, (int)TipoElemental.Planta] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Electrico, (int)TipoElemental.Dragon] = 0.5f;

        // Fantasma
        tablaEfectividad[(int)TipoElemental.Fantasma, (int)TipoElemental.Fantasma] = 2f;
        tablaEfectividad[(int)TipoElemental.Fantasma, (int)TipoElemental.Psiquico] = 2f;
        tablaEfectividad[(int)TipoElemental.Fantasma, (int)TipoElemental.Siniestro] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fantasma, (int)TipoElemental.Normal] = 0f;

        // Fuego
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Bicho] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Acero] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Agua] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Roca] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Dragon] = 0.5f;

        // Hada
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Lucha] = 2f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Dragon] = 2f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Siniestro] = 2f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Veneno] = 0.5f;

        // Lucha
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Normal] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Roca] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Siniestro] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Fantasma] = 0f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Psiquico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Veneno] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Volador] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Hada] = 0.5f;

        // Normal
        tablaEfectividad[(int)TipoElemental.Normal, (int)TipoElemental.Roca] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Normal, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Normal, (int)TipoElemental.Fantasma] = 0f;

        // Psíquico
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Lucha] = 2f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Veneno] = 2f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Psiquico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Siniestro] = 0f;

        // Roca
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Fuego] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Volador] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Bicho] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Lucha] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Tierra] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Acero] = 0.5f;

        // Siniestro
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Psiquico] = 2f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Fantasma] = 2f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Lucha] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Siniestro] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Hada] = 0.5f;

        // Tierra
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Fuego] = 2f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Electrico] = 2f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Veneno] = 2f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Bicho] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Planta] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Volador] = 0f;

        // Veneno
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Hada] = 2f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Veneno] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Tierra] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Acero] = 0f;

        // Volador
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Bicho] = 2f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Lucha] = 2f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Electrico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Volador, (int)TipoElemental.Roca] = 0.5f;

        // Fuego
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Bicho] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Acero] = 2f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Agua] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Roca] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Fuego, (int)TipoElemental.Dragon] = 0.5f;

        // Hada
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Lucha] = 2f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Dragon] = 2f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Siniestro] = 2f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Fuego] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Hada, (int)TipoElemental.Veneno] = 0.5f;

        // Lucha
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Normal] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Roca] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Siniestro] = 2f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Fantasma] = 0f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Psiquico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Veneno] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Volador] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Lucha, (int)TipoElemental.Hada] = 0.5f;

        // Normal
        tablaEfectividad[(int)TipoElemental.Normal, (int)TipoElemental.Roca] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Normal, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Normal, (int)TipoElemental.Fantasma] = 0f;

        // Psíquico
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Lucha] = 2f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Veneno] = 2f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Psiquico] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Acero] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Psiquico, (int)TipoElemental.Siniestro] = 0f;

        // Roca
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Fuego] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Hielo] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Volador] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Bicho] = 2f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Lucha] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Tierra] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Roca, (int)TipoElemental.Acero] = 0.5f;

        // Siniestro
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Psiquico] = 2f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Fantasma] = 2f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Lucha] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Siniestro] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Siniestro, (int)TipoElemental.Hada] = 0.5f;

        // Tierra
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Fuego] = 2f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Electrico] = 2f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Veneno] = 2f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Bicho] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Planta] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Tierra, (int)TipoElemental.Volador] = 0f;

        // Veneno
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Planta] = 2f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Hada] = 2f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Veneno] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Tierra] = 0.5f;
        tablaEfectividad[(int)TipoElemental.Veneno, (int)TipoElemental.Acero] = 0f;
    }
    
    public static int CalcularDanio(Porkemon atacante, Porkemon defensor, AtaqueData ataque)
    {
        float efectividad = tablaEfectividad[(int)ataque.tipo, (int)defensor.BaseData.tipo1];
        
        int ataqueStat = ataque.categoria == CategoriaAtaque.Especial ? atacante.Espiritu : atacante.Ataque;
        int defensaStat = ataque.categoria == CategoriaAtaque.Especial ? defensor.Espiritu : defensor.Defensa;
        
        // Fórmula de daño de Wikidek
        int danioBase = Mathf.RoundToInt(((ataque.poder * ataqueStat) / (float)defensaStat) * 0.5f);
        int danioFinal = Mathf.RoundToInt(danioBase * efectividad);
        
        return Mathf.Max(1, danioFinal);
    }
    
    public static string GetMensajeEfectividad(float efectividad)
    {
        if (efectividad >= 2f) return "¡Es súper eficaz!";
        if (efectividad <= 0f) return "No tiene efecto...";
        if (efectividad <= 0.5f) return "No es muy eficaz...";
        return "";
    }
}
