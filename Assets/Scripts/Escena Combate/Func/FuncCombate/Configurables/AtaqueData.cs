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
    Veneno,
    Hielo,
    Lucha,
    Tierra,
    Volador,
    Fantasma,
    Dragon,
    Siniestro,
    Acero,
    Hada,
    Roca,
    Bicho
}

public enum CategoriaAtaque
{
    Fisico,
    Especial,
    Estado
}

public enum Naturaleza
{
    
    Huraña, // +Atk -Def
    Audaz, // +Atk -Spe
    Firme, // +Atk -SpA
    Pícara, // +Atk -SpD
    Osada, // +Def -Atk
    Dócil, // neutral
    Plácida, // +Def -Spe
    Agitada, // +Def -SpA
    Floja, // +Def -SpD
    Miedosa, // +Spe -Atk
    Activa, // +Spe -Def
    Seria, // neutral
    Alegre, // +Spe -SpA
    Ingenua, // +Spe -SpD
    Modesta, // +SpA -Atk
    Afable, // +SpA -Def
    Mansa, // +SpA -Spe
    Tímida, // neutral
    Alocada, // +SpA -SpD
    Tranquila, // +SpD -Atk
    Amable, // +SpD -Def
    Grosera, // +SpD -Spe
    Cauta, // +SpD -SpA
    Rara // neutral
}

public enum TasaCrecimiento
{
    Rapido,
    Medio,
    Lento,
    Parabolico,
    Erratico,
    Fluctuante
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