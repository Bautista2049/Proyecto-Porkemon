using UnityEngine;

public class PokemonAttackByName : MonoBehaviour
{
    private PokemonAnimator pokemonAnimator;

    void Awake()
    {
        pokemonAnimator = GetComponent<PokemonAnimator>();
        if (pokemonAnimator == null)
            pokemonAnimator = GetComponentInChildren<PokemonAnimator>();
    }

    public void PlayAttackAnimation(Porkemon porkemon, AtaqueData ataque)
    {
        if (pokemonAnimator == null || porkemon == null || ataque == null)
            return;

        if (porkemon.BaseData == null)
            return;

        string pokemonName = porkemon.BaseData.nombre;
        string attackName = ataque.nombreAtaque;

        if (string.IsNullOrEmpty(pokemonName) || string.IsNullOrEmpty(attackName))
            return;

        switch (pokemonName)
        {
            case "Bellsprout":
                PlayBellsproutAttack(attackName);
                break;
            case "KittyZap":
                PlayKittyZapAttack(attackName);
                break;
            case "Charmander":
                PlayCharmanderAttack(attackName);
                break;
            case "PunchBug":
                PlayPunchBugAttack(attackName);
                break;
            case "Draguscular":
                PlayDraguscularAttack(attackName);
                break;
            default:
                pokemonAnimator.PlayAttack(1);
                break;
        }
    }

    void PlayKittyZapAttack(string attackName)
    {
        switch (attackName)
        {
            case "Zappys":
                pokemonAnimator.PlayAttack(1);
                break;
            case "Wizarding":
                pokemonAnimator.PlayAttack(2);
                break;
            case "Placaje":
                pokemonAnimator.PlayAttack(3);
                break;
            default:
                pokemonAnimator.PlayAttack(1);
                break;
        }
    }

    void PlayCharmanderAttack(string attackName)
    {
        switch (attackName)
        {
            case "Placaje":
                pokemonAnimator.PlayAttack(1);
                break;
            case "Manchar":
                pokemonAnimator.PlayAttack(2);
                break;
            case "Golpe Terraqueo":
                pokemonAnimator.PlayAttack(3);
                break;
            default:
                pokemonAnimator.PlayAttack(1);
                break;
        }
    }

    void PlayPunchBugAttack(string attackName)
    {
        switch (attackName)
        {
            case "Adrenaline":
                pokemonAnimator.PlayAttack(1);
                break;
            case "Ataque Sismico":
                pokemonAnimator.PlayAttack(2);
                break;
            case "Golpe Terraqueo":
                pokemonAnimator.PlayAttack(3);
                break;
            default:
                pokemonAnimator.PlayAttack(1);
                break;
        }
    }

    void PlayDraguscularAttack(string attackName)
    {
        switch (attackName)
        {
            case "Ataque Sismico":
                pokemonAnimator.PlayAttack(1);
                break;
            case "Placaje":
                pokemonAnimator.PlayAttack(2);
                break;
            case "Estaca":
                pokemonAnimator.PlayAttack(3);
                break;
            case "Golpe Terraqueo":
                pokemonAnimator.PlayAttack(4);
                break;
            default:
                pokemonAnimator.PlayAttack(1);
                break;
        }
    }

    void PlayBellsproutAttack(string attackName)
    {
        switch (attackName)
        {
            case "Placaje":
                pokemonAnimator.PlayAttack(1);
                break;
            case "Golpe Terraqueo":
                pokemonAnimator.PlayAttack(2);
                break;
            default:
                pokemonAnimator.PlayAttack(1);
                break;
        }
    }
}
