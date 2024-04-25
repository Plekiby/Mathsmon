using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Pokemon
{
    [SerializeField] private PokemonBase _base;
    [SerializeField] private int level;
    [SerializeField] private int currentWins;  // Current wins at this level
    [SerializeField] private int winsRequiredForNextLevel;  // Wins required to level up

    public PokemonBase Base => _base;
    public int Level => level;

    public Move CurrentMove { get; set; }

    public int HP { get; set; }
    public List<Move> Moves { get; set; }

    public Pokemon()
    {
        currentWins = 0;
        winsRequiredForNextLevel = 2;  // Starting requirement for level 2
    }

    public Pokemon(PokemonBase _base, int level, int currentWins, int winsRequiredForNextLevel)
    {
        this._base = _base;
        this.level = level;
        this.currentWins = currentWins;
        this.winsRequiredForNextLevel = winsRequiredForNextLevel;  // Starting requirement for level 2
    }

    // Serialize this Pokemon object to a Json string
    public string Serialize()
    {
        return JsonUtility.ToJson(this);
    }

    // Deserialize a Json string to this Pokemon object
    public static Pokemon Deserialize(string json)
    {
        return JsonUtility.FromJson<Pokemon>(json);
    }

    public void Init()
    {
        HP = MaxHp;
        Moves = new List<Move>();
        foreach (var move in Base.LearnableMoves)
        {
            if (move.Level <= Level)
                Moves.Add(new Move(move.Base));
            if (Moves.Count >= 4)
                break;
        }
    }

    public int MaxHp => Mathf.FloorToInt((Base.MaxHp * Level) / 100f) + 10;
    public int Attack => Mathf.FloorToInt((Base.Attack * Level) / 100f) + 5;
    public int Defense => Mathf.FloorToInt((Base.Defense * Level) / 100f) + 5;
    public int Speed => Mathf.FloorToInt((Base.Speed * Level) / 100f) + 5;
    public int getcurrentWins()
    {
         return currentWins; 
    }
    public int getwinsRequiredForNextLevel()
    {
         return winsRequiredForNextLevel; 
    }
    public int GetExpForCurrentLevel()
    {
        return winsRequiredForNextLevel-currentWins;
    }
    

    public void GainWin()
    {
        currentWins++;
   
    }


    public void OnBattleOver()
    {
       
    }

    public void LevelUp()
    {
        level++;
        currentWins = 0;  // Reset wins count for next level
        winsRequiredForNextLevel = level + 1;  // Increment wins required by level number
        IncreaseStats();
    }

    private void IncreaseStats()
    {
        _base.IncreaseBaseStats();  // This will increase the base stats by 1
    }

    public bool CheckForLevelUp()
    {
        if (currentWins == winsRequiredForNextLevel)
        {
            LevelUp();
            IncreaseStats() ;
            return true;
        }

        return false;
    }

    public bool TakeDamage(Move move, Pokemon attacker)
    {
        float modifiers = Random.Range(0.85f, 1f);  // Random modifier for damage calculation
        float a = (2 * attacker.Level + 10) / 250f;
        float d = a * move.Base.Power * ((float)attacker.Attack / Defense) + 2;
        int damage = Mathf.FloorToInt(d * modifiers);

        HP -= damage;
        return HP <= 0;
    }

    public Move GetRandomMove()
    {
        return Moves[Random.Range(0, Moves.Count)];
    }
}

