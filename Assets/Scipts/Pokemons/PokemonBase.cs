using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pokemon", menuName = "Pokemon/Create new pokemon")]
public class PokemonBase : ScriptableObject
{
    [SerializeField] string name;
    [SerializeField] Sprite frontSprite, backSprite;
    [SerializeField] PokemonType type1;
    [SerializeField] int maxHp, attack, defense, speed;
    [SerializeField] int catchRate = 255;
    [SerializeField] List<LearnableMove> learnableMoves;



    public string Name => name;
    public Sprite FrontSprite => frontSprite;
    public Sprite BackSprite => backSprite;
    public PokemonType Type1 => type1;
    public int MaxHp => maxHp;
    public int Attack => attack;
    public int Defense => defense;
    public int Speed => speed;
    public List<LearnableMove> LearnableMoves => learnableMoves;
    public int CatchRate => catchRate;

    public void IncreaseBaseStats()
    {
        maxHp += 1;
        attack += 1;
        defense += 1;
        speed += 1;
    }
}

[System.Serializable]
public class LearnableMove
{
    [SerializeField] MoveBase moveBase;
    [SerializeField] int level;

    public MoveBase Base
    {
        get { return moveBase; }
    }

    public int Level
    {
        get { return level; }
    }
}

public enum PokemonType
{
    None,
    Addition,
    Soustraction,
    Division,
    Multiplication,
    Equation
}

public enum Stat
{
    Attack,
    Defense,
    SpAttack,
    SpDefense,
    Speed,

    // These 2 are not actual stats, they're used to boost the moveAccuracy
    Accuracy,
    Evasion
}

