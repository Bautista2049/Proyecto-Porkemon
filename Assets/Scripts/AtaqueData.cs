using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Primero, definimos los tipos de ataque y de Porkemon
public enum TipoElemental
{
    Normal,
    Fuego,
    Agua,
    Planta,
    Electrico,
    Psiquico,
    Veneno
}

public enum CategoriaAtaque
{
    Fisico,
    Especial,
    Estado // Para buffs, debuffs, etc.
}

[CreateAssetMenu(fileName = "Nuevo Ataque", menuName = "Porkemon/Ataque")]
public class AtaqueData : ScriptableObject
{
    [Header("Información Básica")]
    public string nombreAtaque;
    [TextArea]
    public string descripcion;
    public TipoElemental tipo;
    public CategoriaAtaque categoria;

    [Header("Estadísticas de Combate")]
    public int poder; // El daño base del ataque
    public int precision; // La probabilidad de acertar (ej. 95)
    public int pp; // Puntos de poder (cuántas veces se puede usar)
    [Range(0, 100)]
    public float chanceCritico = 6.25f; // Probabilidad en % de hacer un crítico

    // Aquí podríamos añadir efectos secundarios en el futuro
    // public EfectoSecundario efecto;
}