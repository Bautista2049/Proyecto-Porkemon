using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameState
{
    public static bool player1Turn = true;
    
    // El ataque que el jugador ha seleccionado para usar
    public static AtaqueData ataqueSeleccionado;

    // Referencia al porkemon actual del jugador para pasarlo entre escenas
    public static Porkemon porkemonDelJugador;
    // Referencia al porkemon actual del bot para pasarlo entre escenas
    public static Porkemon porkemonDelBot;


}