using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathsmonHealer : MonoBehaviour
{

    public void HealPokemon(PlayerControllers player)
    {
        var playerParty = player.GetComponent<PokemonParty>();



        foreach (var pokemon in playerParty.Pokemons)
        {
            pokemon.Init();  // This will reset HP and reinitialize the moves
        }


    }




}
