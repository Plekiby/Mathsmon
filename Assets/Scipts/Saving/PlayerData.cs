using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.SceneManagement;


[System.Serializable]
public class PlayerData 
{
    public float speed;
    double v = 0.09040244;
    public float[] position;
    public UnityEngine.Vector3 pos;
    public List<string> layer;
    public List<Pokemon> pokemonTeam;
    public string currentScene;

    public PlayerData(PlayerControllers player, List<global::Pokemon> globalPokemons)
    {
        Debug.Log($"Entrée");
        speed = player.moveSpeed;
        layer = new List<string>
        {
            "ObjetsSolidesLayer",
            "Nothing",
            "Everything",
            "Nothing"
        };

        if (player != null)
        {
            Debug.Log($"Entrée");
            position = new float[3];
            this.position[0] = player.transform.position.x;
            this.position[1] = player.transform.position.y;
            this.position[2] = ConvertDoubleToFloat(v);
            currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        }
        this.pokemonTeam = ConvertGlobalPokemonsToPlayerDataPokemons(globalPokemons);
    }



    public GameObject Player { get; }
    public List<global::Pokemon> PokemonTeam { get; }
    public PlayerControllers Player1 { get; }
    public List<global::Pokemon> PokemonTeam1 { get; }

    [System.Serializable]
    public class Pokemon
    {
        public PokemonBase Base;
        public int Level;
        public int CurrentWins;
        public int WinsRequiredForNextLevel;
    }

    public static float ConvertDoubleToFloat(double value)
    {
        return (float)value;
    }

    private List<PlayerData.Pokemon> ConvertGlobalPokemonsToPlayerDataPokemons(List<global::Pokemon> globalPokemons)
    {
        List<PlayerData.Pokemon> convertedList = new List<PlayerData.Pokemon>();
        foreach (var globalPokemon in globalPokemons)
        {
            PlayerData.Pokemon localPokemon = new PlayerData.Pokemon
            {
                Base = globalPokemon.Base, 
                Level = globalPokemon.Level, 
                CurrentWins = globalPokemon.getcurrentWins(),
                WinsRequiredForNextLevel = globalPokemon.getwinsRequiredForNextLevel(),
            };
            convertedList.Add(localPokemon);
        }
        return convertedList;
    }


}
