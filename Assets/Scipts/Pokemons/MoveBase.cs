using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move")]
public class MoveBase : ScriptableObject
{
    [SerializeField] string name;

    [TextArea]
    [SerializeField] string description;

    [SerializeField] PokemonType type;
    [SerializeField] int power;

    public string Name {
        get { return name; }
    }

    public string Description {
        get { return description; }
    }

    public PokemonType Type {
        get { return type; }
    }

    public int Power {
        get { return power; }
    }

}
