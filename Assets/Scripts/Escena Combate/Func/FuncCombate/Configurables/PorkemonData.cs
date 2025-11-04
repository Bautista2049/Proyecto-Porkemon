using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Nuevo Porkemon", menuName = "Porkemon/Porkemon")]
public class PorkemonData : ScriptableObject
{
    public string nombre;
    public GameObject modeloPrefab;
    public TipoElemental tipo1;
    public TipoElemental tipo2;
    public int baseRate = 120;
    public int nivel = 5;
    public int vidaMaxima = 20;
    public int ataque = 5;
    public int defensa = 5;
    public int espiritu = 5;
    public int velocidad = 5;
    public List<AtaqueData> ataquesQuePuedeAprender;
    public PorkemonData evolucionSiguiente;
    public int nivelDeEvolucion;
    public Naturaleza naturaleza = Naturaleza.Firme; 
    public TasaCrecimiento tasaCrecimiento = TasaCrecimiento.Medio; 
    public int experienciaBase = 50;
}
