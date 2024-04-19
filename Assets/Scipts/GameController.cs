using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { FreeRoam, Battle, Dialog, Cutscene }

public class GameController : MonoBehaviour
{
    [SerializeField] PlayerControllers playerControllers;
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    GameState state;

    public static GameController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        playerControllers.OnEncountered += StartBattle;
        battleSystem.OnBattleOver += EndBattle;

        // Assurez-vous que le système de combat est désactivé au démarrage
        battleSystem.gameObject.SetActive(false);

        // Assurez-vous que la caméra du monde est active au démarrage
        worldCamera.gameObject.SetActive(true);

        // Initialiser le jeu en mode FreeRoam
        state = GameState.FreeRoam;

        playerControllers.OnEnterTrainersView += (Collider2D trainerCollider) =>
        {
            var trainer = trainerCollider.GetComponentInParent<TrainerController>();
            if (trainer != null)
            {
                state = GameState.Cutscene;
                StartCoroutine(trainer.TriggerTrainerBattle(playerControllers));
            }
        };

        DialogManager.Instance.OnShowDialog += () =>
        {
            state = GameState.Dialog;
        };

        DialogManager.Instance.OnCloseDialog += () =>
        {
            if (state == GameState.Dialog)
                state = GameState.FreeRoam;
        };
    }

    void StartBattle()
    {
        Debug.Log("Encountered wild Pokémon, starting battle.");

        state = GameState.Battle;
        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = playerControllers.GetComponent<PokemonParty>();
        var wildPokemon = FindObjectOfType<MapArea>().GetComponent<MapArea>().GetRandomWildPokemon();

        battleSystem.StartBattle(playerParty, wildPokemon);
    }

    TrainerController trainer;
    public void StartTrainerBattle(TrainerController trainer)
    {
        Debug.Log("Starting trainer battle. Disabling world camera and enabling battle system.");
        state = GameState.Battle;
        worldCamera.gameObject.SetActive(false);
        Debug.Log("World Camera active: " + worldCamera.gameObject.activeSelf);
        battleSystem.gameObject.SetActive(true);
        Debug.Log("Battle System active: " + battleSystem.gameObject.activeSelf);

        this.trainer = trainer;
        PokemonParty playerParty = playerControllers.GetComponent<PokemonParty>();
        PokemonParty trainerParty = trainer.GetComponent<PokemonParty>();

        //yield return new WaitForSeconds(1);  // Optional wait time before starting the battle for dramatic effect.
        battleSystem.StartTrainerBattle(playerParty, trainerParty);
    }

    void EndBattle(bool won)
    {
        state = GameState.FreeRoam;
        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (state == GameState.FreeRoam)
        {
            playerControllers.HandleUpdate();
        }
        else if (state == GameState.Battle)
        {
            battleSystem.HandleUpdate();
        }
        else if (state == GameState.Dialog)
        {
            DialogManager.Instance.HandleUpdate();
        }
    }
}
