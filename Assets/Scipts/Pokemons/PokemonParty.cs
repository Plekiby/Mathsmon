using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PokemonParty : MonoBehaviour
{
    [SerializeField] List<Pokemon> pokemons;  // Liste contenant les Pokémon de l'équipe

    public List<Pokemon> Pokemons
    {
        get
        {
            return pokemons;  // Retourne la liste des Pokémon de l'équipe
        }
        set { pokemons = value; }  // Permet de modifier la liste des Pokémon de l'extérieur
    }

    // Retourne le nombre de Pokémon dans l'équipe
    public int taille()
    {
        return pokemons.Count;
    }

    private void Start()
    {
        // Initialisation de chaque Pokémon au démarrage du jeu
        foreach (var pokemon in pokemons)
        {
            pokemon.Init();
        }
    }

    // Obtient le premier Pokémon en santé de l'équipe
    public Pokemon GetHealthyPokemon()
    {
        return pokemons.Where(x => x.HP > 0).FirstOrDefault();
    }

    // Ajoute un nouveau Pokémon à l'équipe
    public void AddPokemon(Pokemon newPokemon)
    {
        if (pokemons.Count < 3)
        {
            pokemons.Add(newPokemon);  // Ajoute un nouveau Pokémon si l'équipe en contient moins de trois
        }
        else
        {
            // Si l'équipe a déjà trois Pokémon, affiche un message ou gère le remplacement
        }
    }
}
