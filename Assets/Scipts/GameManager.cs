using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string PlayerName { get; set; }
    public PokemonParty PlayerParty { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Rend l'instance persistante à travers les scènes
        }
        else
        {
            Destroy(gameObject);  // Assure qu'il n'y a pas de doublons
        }
    }

    public void SetupPlayer(string name, PokemonParty party)
    {
        PlayerName = name;
        PlayerParty = party;
    }
}
