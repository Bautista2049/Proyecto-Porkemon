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
    ProteccionX
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
