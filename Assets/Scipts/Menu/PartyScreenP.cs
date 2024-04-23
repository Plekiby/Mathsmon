using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenP : MonoBehaviour
{
    [SerializeField] private GameObject panel;  // Assurez-vous d'assigner le GameObject du panel dans l'Inspector
    [SerializeField] private Text messageText;
    [SerializeField] private PartyMemberUI[] memberSlots;  // Les UI slots pour chaque Pokémon
    private PokemonParty playerParty;  // Référence au script PokemonParty

    void Start()
    {
        playerParty = FindObjectOfType<PokemonParty>();  // Trouve le script PokemonParty dans la scène
        SetPartyData(playerParty.Pokemons);  // Utilise la liste des Pokémon
        panel.SetActive(false);  // Assurez-vous que le panel est désactivé au démarrage
    }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))  // Ecoute pour la touche 'E'
        {
            panel.SetActive(!panel.activeSelf);  // Active ou désactive le panel
        }
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);  // Configure chaque slot
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }

        messageText.text = "Choose a Pokemon";
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < playerParty.Pokemons.Count; i++)
        {
            memberSlots[i].SetSelected(i == selectedMember);
        }
    }
}
