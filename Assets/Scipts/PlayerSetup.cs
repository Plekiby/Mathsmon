using UnityEngine;


public class PlayerSetup : MonoBehaviour
{
    [SerializeField] private PokemonParty startingParty;  // Configure this in the editor

    private void Start()
    {
        GameManager.Instance.SetupPlayer("Pierre", startingParty);
    }
}
