using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public Vector3Serializable PlayerPosition;
    public string SceneName;
    public List<string> PokemonData; // Liste de données Pokémon sérialisées en JSON
}

[System.Serializable]
public class Vector3Serializable
{
    public float x, y, z;

    public Vector3Serializable(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector()
    {
        return new Vector3(x, y, z);
    }
}
