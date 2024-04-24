using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreenP : MonoBehaviour
{
    [SerializeField] private GameObject panel;  // Assurez-vous d'assigner le GameObject du panel dans l'Inspector
    [SerializeField] private PartyMemberUI[] memberSlots;  // Les UI slots pour chaque Pokémon
    private PokemonParty playerParty;  // Référence au script PokemonParty

    void Start()
    {
        playerParty = FindObjectOfType<PokemonParty>();  // Trouve le script PokemonParty dans la scène
        playerParty.OnPartyUpdated += UpdateUI;  // S'abonner à l'événement
        Init();
        SetPartyData(playerParty.Pokemons);  // Utilise la liste des Pokémon
        panel.SetActive(false);  // Assurez-vous que le panel est désactivé au démarrage
    }

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            panel.SetActive(!panel.activeSelf);
            if (panel.activeSelf)
            {
                SetPartyData(playerParty.Pokemons);  // Rafraîchit les données chaque fois que le panel est activé
            }
        }
    }

    void OnDestroy()
    {
        if (playerParty != null)
            playerParty.OnPartyUpdated -= UpdateUI;  // Se désabonner pour éviter les fuites de mémoire
    }

    private void UpdateUI()
    {
        SetPartyData(playerParty.Pokemons);  // Mettre à jour l'UI chaque fois que la liste de Pokémon est modifiée
    }

    public void SetPartyData(List<Pokemon> pokemons)
    {
        Debug.Log($"Updating party screen with {pokemons.Count} pokemons.");
        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].SetData(pokemons[i]);
            }
            else
            {
                memberSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateMemberSelection(int selectedMember)
    {
        for (int i = 0; i < playerParty.Pokemons.Count; i++)
        {
            memberSlots[i].SetSelected(i == selectedMember);
        }
    }
}
