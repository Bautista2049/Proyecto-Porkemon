using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    Estado
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
    public int poder;
    public int precision;
    public int pp;
    [Range(0, 100)]
    public float chanceCritico = 6.25f;
}