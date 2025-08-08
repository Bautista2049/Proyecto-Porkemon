using UnityEngine;

public class Ataques : MonoBehaviour
{
    private FuncTurnos funcTurnos;

    private void Start()
    {
        funcTurnos = FindObjectOfType<FuncTurnos>();
        if (funcTurnos == null)
        {
            Debug.LogError("No se encontró FuncTurnos en la escena.");
        }
    }

    public void Ataque1()
    {
        AtaqueConDanio(10);
    }

    public void Ataque2()
    {
        AtaqueConDanio(20);
    }

    public void Ataque3()
    {
        AtaqueConDanio(30);
    }

    public void Ataque4()
    {
        AtaqueConDanio(40);
    }

    private void AtaqueConDanio(int danio)
    {
        if (funcTurnos == null) return;
        funcTurnos.danioPorAtaque = danio;
        funcTurnos.Atacar();
    }
}
