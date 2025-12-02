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

// --- NUEVAS ESTRUCTURAS PARA EFECTOS SECUNDARIOS ---

/// <summary>
/// Define las estadísticas que un ataque puede modificar.
/// </summary>
public enum Estadistica
{
    Ninguna,
    Ataque,
    Defensa,
    Espiritu, // Usado para Ataque Especial y Defensa Especial
    Velocidad,
    Precision,
    Evasion
}

/// <summary>
/// Clase para definir una modificación de estadística.
/// </summary>
[System.Serializable]
public class ModificadorEstadistica
{
    public Estadistica estadistica;
    [Tooltip("Número de niveles que sube o baja la estadística (ej: 1 para +1, -1 para -1).")]
    public int etapas;
    [Tooltip("Marcar si el efecto es para el oponente. Desmarcar si es para el usuario del ataque.")]
    public bool esParaOponente = true;
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

    [Header("Mecánicas Especiales")]
    [Tooltip("Si se marca, este ataque copiará el último movimiento usado por el oponente.")]
    public bool esMimetizacion = false;

    [Header("Efectos Secundarios")]
    [Tooltip("El estado alterado que este ataque puede infligir.")]
    public EstadoAlterado estadoQueAplica = EstadoAlterado.Ninguno;

    [Tooltip("La probabilidad (en %) de que el estado alterado se aplique (0 si nunca ocurre).")]
    [Range(0, 100)]
    public int probabilidadEstado = 0;

    [Space(10)]
    [Tooltip("Lista de modificaciones de estadísticas que puede causar el ataque.")]
    public List<ModificadorEstadistica> modificadoresDeStats;

    [Tooltip("La probabilidad (en %) de que se apliquen las modificaciones de estadísticas (0 si nunca ocurre).")]
    [Range(0, 100)]
    public int probabilidadModificacionStats = 0;
}