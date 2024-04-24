using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathsmonGiver : MonoBehaviour
{
    [SerializeField] Pokemon pokemon; 
    [SerializeField] Dialog dialog;  

    bool used = false;  

    // Coroutine pour donner un Pokémon au joueur
    public IEnumerator GivePokemon(PlayerControllers player)
    {
        yield return DialogManager.Instance.ShowDialog(dialog);  // Affiche le dialogue initial

        yield return new WaitForSeconds(3f);

        pokemon.Init();

        // Initialisation du Pokémon et ajout à la partie du joueur
        player.GetComponent<PokemonParty>().AddPokemon(pokemon); 

        used = true;  

        // Construit et affiche un dialogue indiquant que le joueur a reçu le Pokémon
        string dialogText = $"{player.Name} a reçu un {pokemon.Base.Name}";
        yield return DialogManager.Instance.ShowDialogText(dialogText);
    }

    // Fonction pour vérifier si le Pokémon a déjà été donné
    public bool use()
    {
        return used;
    }
}
