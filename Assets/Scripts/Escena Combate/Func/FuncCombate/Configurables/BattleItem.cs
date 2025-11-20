using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleItemType
{
    AtaqueX,
    DefensaX,
    AtaqueEspecialX,
    DefensaEspecialX,
    VelocidadX,
    PrecisionX,
    CriticoX,
    ProteccionX,
    Porkebola,
    Superbola,
    Ultrabola,
    Masterbola,
    Pocion,
    Superpocion,
    Hiperpocion,
    Pocionmaxima,
    Revivir,
    RevivirMax,

    // Mentas (cambian la naturaleza del Porkemon)
    MentaTimida,
    MentaSeria,
    MentaAtrevida,
    MentaRelajante,
    MentaErupcion,
    MentaTranquila,
    MentaTraviesa,
    MentaIngenua,
    MentaModesta,
    MentaSuave,
    MentaSolitaria,
    MentaLaxa,
    MentaAlegre,
    MentaRapida,
    MentaCuidadosa,
    MentaCalmante,
    MentaValiente,
    MentaAudaz,
    MentaAdamant,

    // Objetos Roto / de apoyo
    RotoPremio,
    RotoExp,
    RotoBoost,
    RotoCatch,
    RotoOferta
}

[System.Serializable]
public class BattleItem
{
    public BattleItemType type;
    public string nombre;
    public string descripcion;
    public int cantidad;

    public BattleItem(BattleItemType type, string nombre, string descripcion, int cantidad = 1)
    {
        this.type = type;
        this.nombre = nombre;
        this.descripcion = descripcion;
        this.cantidad = cantidad;
    }
}
