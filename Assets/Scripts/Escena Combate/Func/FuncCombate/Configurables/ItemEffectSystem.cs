using UnityEngine;

/// <summary>
/// Sistema centralizado para aplicar los efectos de los BattleItems.
/// Reemplaza las 3 copias duplicadas que existían en ControladorCambio, ControladorMochila y FuncTurnos.
/// </summary>
public static class ItemEffectSystem
{
    /// <summary>
    /// Aplica el efecto de un item sobre un Porkemon. Retorna true si el efecto fue aplicado correctamente.
    /// </summary>
    public static bool AplicarEfecto(BattleItem item, Porkemon porkemon)
    {
        if (item == null || porkemon == null) return false;

        switch (item.type)
        {
            // --- Pociones ---
            case BattleItemType.Pocion:
                CurarVida(porkemon, 20, item.nombre);
                break;
            case BattleItemType.Superpocion:
                CurarVida(porkemon, 50, item.nombre);
                break;
            case BattleItemType.Hiperpocion:
                CurarVida(porkemon, 200, item.nombre);
                break;
            case BattleItemType.Pocionmaxima:
                int curacionMax = porkemon.VidaMaxima - porkemon.VidaActual;
                porkemon.VidaActual = porkemon.VidaMaxima;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Recuperó {curacionMax} PS!");
                break;

            // --- Estadísticas X ---
            case BattleItemType.AtaqueX:
                porkemon.AumentarAtaque(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Ataque aumentado!");
                ActivarBuffVisual();
                break;
            case BattleItemType.DefensaX:
                porkemon.AumentarDefensa(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa aumentada!");
                ActivarBuffVisual();
                break;
            case BattleItemType.AtaqueEspecialX:
                porkemon.AumentarEspiritu(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Ataque Especial aumentado!");
                ActivarBuffVisual();
                break;
            case BattleItemType.DefensaEspecialX:
                porkemon.AumentarEspiritu(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Defensa Especial aumentada!");
                ActivarBuffVisual();
                break;
            case BattleItemType.VelocidadX:
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Velocidad aumentada!");
                ActivarBuffVisual();
                break;
            case BattleItemType.PrecisionX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Precisión aumentada!");
                break;
            case BattleItemType.CriticoX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Índice crítico aumentado!");
                break;
            case BattleItemType.ProteccionX:
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. Protección activada!");
                break;

            // --- Objetos Roto ---
            case BattleItemType.RotoPremio:
                GameState.multiplicadorDinero = 3f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Las recompensas de dinero aumentarán esta batalla!");
                break;
            case BattleItemType.RotoExp:
                GameState.multiplicadorExp = 1.5f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡La experiencia ganada aumentará esta batalla!");
                break;
            case BattleItemType.RotoBoost:
                porkemon.AumentarAtaque(2);
                porkemon.AumentarDefensa(2);
                porkemon.AumentarEspiritu(2);
                porkemon.AumentarVelocidad(2);
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Todas sus estadísticas han aumentado!");
                ActivarBuffVisual();
                break;
            case BattleItemType.RotoCatch:
                GameState.multiplicadorCaptura = 2f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡La probabilidad de captura ha aumentado esta batalla!");
                break;
            case BattleItemType.RotoOferta:
                GameState.multiplicadorPreciosTienda = 0.5f;
                Debug.Log($"{porkemon.BaseData.nombre} usó {item.nombre}. ¡Los precios de la tienda se han reducido temporalmente!");
                break;

            default:
                Debug.LogWarning($"Tipo de item no manejado: {item.type}");
                return false;
        }

        return true;
    }

    /// <summary>
    /// Consume un item del inventario y sincroniza con el inventario completo.
    /// </summary>
    public static void ConsumirItem(BattleItem item)
    {
        if (item == null) return;

        item.cantidad--;

        if (GestorDeBatalla.instance != null)
            GestorDeBatalla.instance.SincronizarInventarioCompleto(item);

        if (item.cantidad <= 0 && GestorDeBatalla.instance != null)
            GestorDeBatalla.instance.inventarioBattleItems.Remove(item);
    }

    /// <summary>
    /// Verifica si un tipo de item es una Pokébola.
    /// </summary>
    public static bool EsPorkebola(BattleItemType type)
    {
        return type == BattleItemType.Porkebola ||
               type == BattleItemType.Superbola ||
               type == BattleItemType.Ultrabola ||
               type == BattleItemType.Masterbola;
    }

    private static void CurarVida(Porkemon porkemon, int cantidad, string nombreItem)
    {
        int curacion = Mathf.Min(cantidad, porkemon.VidaMaxima - porkemon.VidaActual);
        porkemon.VidaActual += curacion;
        Debug.Log($"{porkemon.BaseData.nombre} usó {nombreItem}. Recuperó {curacion} PS!");
    }

    private static void ActivarBuffVisual()
    {
        if (GestorDeBatalla.instance != null)
            GestorDeBatalla.instance.ActivarBuffVisualJugador(-1f);
    }
}
