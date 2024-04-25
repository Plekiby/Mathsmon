using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, ActionSelection, MoveSelection, RunningTurn, Busy, PartyScreen, AboutToUse, BattleOver }
public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    // Déclarations des composants et variables nécessaires
    // Des composants sérialisés pour permettre une configuration facile via l'éditeur Unity
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;

    // Jeux de mathématiques pour différents niveaux de difficulté
    [SerializeField] private AdditionSimple additionSimple;
    [SerializeField] private AdditionMoyen additionMoyen;
    [SerializeField] private AdditionDifficile additionDifficile;
    [SerializeField] private SoustractionSimple soustractionSimple;
    [SerializeField] private SoustractionMoyen soustractionMoyen;
    [SerializeField] private SoustractionDifficile soustractionDifficile;
    [SerializeField] private MultiplicationSimple multiplicationSimple;
    [SerializeField] private MultiplicationMoyen multiplicationMoyen;
    [SerializeField] private MultiplicationDifficile multiplicationDifficile;
    [SerializeField] private DivisionSimple divisionSimple;
    [SerializeField] private DivisionMoyen divisionMoyen;
    [SerializeField] private DivisionDifficile divisionDifficile;

    public event Action<bool> OnBattleOver;

    private BattleState state;
    private BattleState? prevState;
    private int currentAction;
    private int currentMove;
    private int currentMember;
    private bool aboutToUseChoice = true;
    private MonoBehaviour activeGame;
    private bool lastAnswerWasCorrect = false;

    private PokemonParty playerParty;
    private PokemonParty trainerParty;
    private Pokemon wildPokemon;

    private bool isTrainerBattle = false;
    private PlayerControllers player;
    private TrainerController trainer;

    private int escapeAttempts;

    // Commence une bataille contre un Pokémon sauvage
    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerControllers>();
        isTrainerBattle = false;

        StartCoroutine(SetupBattle());
    }

    // Commence une bataille contre un dresseur
    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;
        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerControllers>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    // Configuration initiale de la bataille
    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"Un {enemyUnit.Pokemon.Base.Name} sauvage apparaît.");
        }
        else
        {
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogBox.TypeDialog($"{trainer.Name} veut combattre.");

            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} envoie {enemyPokemon.Base.Name}.");

            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Vas-y {playerPokemon.Base.Name} !");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        escapeAttempts = 0;
        partyScreen.Init();
        yield return new WaitForSeconds(1);  // Assurez-vous que tous les dialogues sont terminés
        ActionSelection();  // Initie correctement la sélection d'action
    }

    // Définit le jeu actif basé sur le type de jeu choisi
    public void SetActiveGame(string gameType)
    {
        // Association du type de jeu aux différentes instances de jeux de mathématiques
        switch (gameType)
        {
            case "Simple":
                activeGame = additionSimple;
                break;
            case "Moyen":
                activeGame = additionMoyen;
                break;
            case "Difficile":
                activeGame = additionDifficile;
                break;
            case "soustractionSimple":
                activeGame = soustractionSimple;
                break;
            case "soustractionMoyen":
                activeGame = soustractionMoyen;
                break;
            case "soustractionDifficile":
                activeGame = soustractionDifficile;
                break;
            case "multiplicationSimple":
                activeGame = multiplicationSimple;
                break;
            case "multiplicationMoyen":
                activeGame = multiplicationMoyen;
                break;
            case "multiplicationDifficile":
                activeGame = multiplicationDifficile;
                break;
            case "divisionSimple":
                activeGame = divisionSimple;
                break;
            case "divisionMoyen":
                activeGame = divisionMoyen;
                break;
            case "divisionDifficile":
                activeGame = divisionDifficile;
                break;
        }
    }

    // Traite la réponse du joueur lorsqu'il soumet une réponse à un jeu de mathématiques
    public void OnAnswerSubmitted()
    {
        if (activeGame == additionSimple)
            lastAnswerWasCorrect = additionSimple.AnswerQuestion();
        else if (activeGame == additionMoyen)
            lastAnswerWasCorrect = additionMoyen.AnswerQuestion();
        else if (activeGame == additionDifficile)
            lastAnswerWasCorrect = additionDifficile.AnswerQuestion();
        else if (activeGame == soustractionSimple)
            lastAnswerWasCorrect = soustractionSimple.AnswerQuestion();
        else if (activeGame == soustractionMoyen)
            lastAnswerWasCorrect = soustractionMoyen.AnswerQuestion();
        else if (activeGame == soustractionDifficile)
            lastAnswerWasCorrect = soustractionDifficile.AnswerQuestion();
        else if (activeGame == multiplicationSimple)
            lastAnswerWasCorrect = multiplicationSimple.AnswerQuestion();
        else if (activeGame == multiplicationMoyen)
            lastAnswerWasCorrect = multiplicationMoyen.AnswerQuestion();
        else if (activeGame == multiplicationDifficile)
            lastAnswerWasCorrect = multiplicationDifficile.AnswerQuestion();
        else if (activeGame == divisionSimple)
            lastAnswerWasCorrect = divisionSimple.AnswerQuestion();
        else if (activeGame == divisionMoyen)
            lastAnswerWasCorrect = divisionMoyen.AnswerQuestion();
        else if (activeGame == divisionDifficile)
            lastAnswerWasCorrect = divisionDifficile.AnswerQuestion();
    }

    // Termine la bataille et notifie si le joueur a gagné ou perdu
    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver?.Invoke(won);
    }

    // Initie la sélection d'action
    void ActionSelection()
    {
        if (state != BattleState.AboutToUse && state != BattleState.PartyScreen && state != BattleState.MoveSelection)
        {
            Debug.Log("Entrée dans l'état de sélection d'action");
            state = BattleState.ActionSelection;
            dialogBox.SetDialog("Choisissez une action");
            dialogBox.EnableActionSelector(true);
        }
        else
        {
            Debug.LogError("Tentative incorrecte d'entrer dans la sélection d'action depuis " + state.ToString());
        }
    }

    // Ouvre l'écran de sélection du groupe de Pokémon
    void OpenPartyScreen()
    {
        prevState = state;  // Mémorise l'état actuel pour y retourner après la sélection
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    // Initie la sélection de mouvement
    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    // Gère le moment où le dresseur adverse s'apprête à utiliser un nouveau Pokémon
    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        yield return dialogBox.TypeDialog($"{trainer.Name} s'apprête à utiliser {newPokemon.Base.Name}. Voulez-vous changer de Pokémon ?");
        dialogBox.EnableChoiceBox(true);

        state = BattleState.AboutToUse;
        while (state == BattleState.AboutToUse)
        {
            yield return null; // Attend ici jusqu'à ce qu'un choix soit fait
        }

        dialogBox.EnableChoiceBox(false);
    }

    // Exécute les tours en fonction de l'action choisie par le joueur
    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            // Vérifie qui commence
            bool playerGoesFirst = true;
            playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            // Premier tour
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Deuxième tour
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                if (state == BattleState.BattleOver) yield break;
            }
        }
        else
        {
            if (playerAction == BattleAction.SwitchPokemon)
            {
                var selectedPokemon = playerParty.Pokemons[currentMember];
                state = BattleState.Busy;
                yield return SwitchPokemon(selectedPokemon);
            }
            else if (playerAction == BattleAction.UseItem)
            {
                dialogBox.EnableActionSelector(false);
                if (player.getNbMathsball() > 0)
                {
                    player.NbMathsballm();
                    yield return ThrowPokeball();
                }
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            // Tour de l'ennemi
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    // Exécute un mouvement durant le tour
    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        // Réinitialise toutes les barres de calcul
        test();

        // Vérifie si c'est le tour du joueur et configure le jeu correct
        if (sourceUnit.IsPlayerUnit)
        {
            ActivateCalculBar(playerUnit.Pokemon.Base.Name, currentMove);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); // Attendez que le joueur appuie sur Entrée

            OnAnswerSubmitted(); // Vérifie la réponse après soumission

            test();

            if (!lastAnswerWasCorrect) // Si la réponse est incorrecte, sautez l'attaque
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} a essayé d'utiliser {move.Base.Name} mais s'est trompé dans le calcul !");
                yield break; // Sautez l'attaque
            }
        }

        // Si la réponse est correcte ou si c'est le tour de l'ennemi, effectuez l'attaque
        dialogBox.SetDialog($"{sourceUnit.Pokemon.Base.Name} utilise {move.Base.Name} !");
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateHP();

        if (targetUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(targetUnit);
        }
    }

    // Active la barre de calcul en fonction du Pokémon et de l'indice du mouvement
    void ActivateCalculBar(string pokemonName, int moveIndex)
    {
        switch (pokemonName)
        {
            case "Additix":
                if (moveIndex == 0) { dialogBox.EnableCalculBar(true); SetActiveGame("Simple"); }
                else if (moveIndex == 1) { dialogBox.EnableCalculBarMoyen(true); SetActiveGame("Moyen"); }
                else { dialogBox.EnableCalculBarDifficile(true); SetActiveGame("Difficile"); }
                additionSimple.NextQuestion();
                break;
            case "Soustrix":
                if (moveIndex == 0) { dialogBox.EnableCalculBarSoustra(true); SetActiveGame("soustractionSimple"); }
                else if (moveIndex == 1) { dialogBox.EnableCalculBarSoustraMoyen(true); SetActiveGame("soustractionMoyen"); }
                else { dialogBox.EnableCalculBarSoustraDifficile(true); SetActiveGame("soustractionDifficile"); }
                soustractionSimple.NextQuestion();
                break;
            case "Multiplix":
                if (moveIndex == 0) { dialogBox.EnableCalculBarMulti(true); SetActiveGame("multiplicationSimple"); }
                else if (moveIndex == 1) { dialogBox.EnableCalculBarMultiMoyen(true); SetActiveGame("multiplicationMoyen"); }
                else { dialogBox.EnableCalculBarMultiDifficile(true); SetActiveGame("multiplicationDifficile"); }
                multiplicationSimple.NextQuestion();
                break;
            case "Dividix":
                if (moveIndex == 0) { dialogBox.EnableCalculBarDivi(true); SetActiveGame("divisionSimple"); }
                else if (moveIndex == 1) { dialogBox.EnableCalculBarDiviMoyen(true); SetActiveGame("divisionMoyen"); }
                else { dialogBox.EnableCalculBarDiviDifficile(true); SetActiveGame("divisionDifficile"); }
                divisionSimple.NextQuestion();
                break;
        }
    }

    // Gère quand un Pokémon est mis K.O.
    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} est K.O.");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (faintedUnit.IsPlayerUnit)
        {
            // Vérifiez s'il y a un autre Pokémon du joueur pour continuer à combattre
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();  // Demande au joueur de sélectionner un autre Pokémon
            else
                BattleOver(false);  // Plus de Pokémon en bonne santé, le joueur perd
        }
        else
        {
            // Gain d'expérience
            int expGain = 1;
            playerUnit.Pokemon.GainWin();
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} a gagné {expGain} d'exp");
            yield return playerUnit.Hud.SetExpSmooth();

            // Vérification de la montée de niveau
            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} monte au niveau {playerUnit.Pokemon.Level}");

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);

            // Le Pokémon ennemi est K.O., vérifiez s'il y en a un autre en ligne
            if (isTrainerBattle)
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));  // Demande de changement de Pokémon
                else
                    BattleOver(true);  // Plus de Pokémon, le joueur gagne
            }
            else
            {
                BattleOver(true);  // Combat contre un Pokémon sauvage, victoire automatique
            }
        }
    }

    // Vérifie si la bataille est terminée après qu'un Pokémon soit mis K.O.
    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        if (faintedUnit.IsPlayerUnit)
        {
            // Le Pokémon du joueur est K.O.
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();  // Changer pour un autre Pokémon
            else
                BattleOver(false);  // Plus de Pokémon en bonne santé, le joueur perd
        }
        else
        {
            // Le Pokémon ennemi est K.O.
            if (!isTrainerBattle)
            {
                BattleOver(true);  // Combat contre un Pokémon sauvage gagné
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));  // Demande au joueur s'il veut changer de Pokémon
                else
                    BattleOver(true);  // Plus de Pokémon pour le dresseur, le joueur gagne
            }
        }
    }

    // Gère la mise à jour en fonction de l'état de la bataille
    public void HandleUpdate()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.PartyScreen)
        {
            HandlePartySelection();
        }
        else if (state == BattleState.AboutToUse)
        {
            HandleAboutToUse();
        }
    }

    // Gère la sélection d'action par le joueur
    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentAction;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentAction;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentAction += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentAction -= 2;

        currentAction = Mathf.Clamp(currentAction, 0, 3);

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentAction == 0)
            {
                // Combat
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Sac
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                // Pokémon
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Fuite
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

    // Gère la sélection de mouvement par le joueur
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMove;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMove;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMove += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMove -= 2;

        currentMove = Mathf.Clamp(currentMove, 0, playerUnit.Pokemon.Moves.Count - 1);

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var move = playerUnit.Pokemon.Moves[currentMove];

            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);

            if (playerUnit.Pokemon.Base.Name == "Additix")
            {
                if (currentMove == 0)
                {
                    dialogBox.EnableCalculBar(true);
                    SetActiveGame("Simple");
                    additionSimple.NextQuestion();
                }
                else if (currentMove == 1)
                {
                    dialogBox.EnableCalculBarMoyen(true);
                    SetActiveGame("Moyen");
                    additionMoyen.NextQuestion();
                }
                else
                {
                    dialogBox.EnableCalculBarDifficile(true);
                    SetActiveGame("Difficile");
                    additionDifficile.NextQuestion();
                }
            }
            else if (playerUnit.Pokemon.Base.Name == "Soustrix")
            {
                if (currentMove == 0)
                {
                    dialogBox.EnableCalculBarSoustra(true);
                    SetActiveGame("soustractionSimple");
                    soustractionSimple.NextQuestion();
                }
                else if (currentMove == 1)
                {
                    dialogBox.EnableCalculBarSoustraMoyen(true);
                    SetActiveGame("soustractionMoyen");
                    soustractionMoyen.NextQuestion();
                }
                else
                {
                    dialogBox.EnableCalculBarSoustraDifficile(true);
                    SetActiveGame("soustractionDifficile");
                    soustractionDifficile.NextQuestion();
                }
            }
            else if (playerUnit.Pokemon.Base.Name == "Multiplix")
            {
                if (currentMove == 0)
                {
                    dialogBox.EnableCalculBarMulti(true);
                    SetActiveGame("multiplicationSimple");
                    multiplicationSimple.NextQuestion();
                }
                else if (currentMove == 1)
                {
                    dialogBox.EnableCalculBarMultiMoyen(true);
                    SetActiveGame("multiplicationMoyen");
                    multiplicationMoyen.NextQuestion();
                }
                else
                {
                    dialogBox.EnableCalculBarMultiDifficile(true);
                    SetActiveGame("multiplicationDifficile");
                    multiplicationDifficile.NextQuestion();
                }
            }
            else if (playerUnit.Pokemon.Base.Name == "Dividix")
            {
                if (currentMove == 0)
                {
                    dialogBox.EnableCalculBarDivi(true);
                    SetActiveGame("divisionSimple");
                    divisionSimple.NextQuestion();
                }
                else if (currentMove == 1)
                {
                    dialogBox.EnableCalculBarDiviMoyen(true);
                    SetActiveGame("divisionMoyen");
                    divisionMoyen.NextQuestion();
                }
                else
                {
                    dialogBox.EnableCalculBarDiviDifficile(true);
                    SetActiveGame("divisionDifficile");
                    divisionDifficile.NextQuestion();
                }
            }
            StartCoroutine(RunTurns(BattleAction.Move));
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            ActionSelection();
        }
    }

    // Gère la sélection de Pokémon par le joueur
    void HandlePartySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++currentMember;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --currentMember;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            currentMember += 2;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            currentMember -= 2;

        currentMember = Mathf.Clamp(currentMember, 0, playerParty.Pokemons.Count - 1);

        partyScreen.UpdateMemberSelection(currentMember);

        if (Input.GetKeyDown(KeyCode.E))
        {
            var selectedMember = playerParty.Pokemons[currentMember];
            if (selectedMember.HP <= 0)
            {
                partyScreen.SetMessageText("Vous ne pouvez pas envoyer un Pokémon évanoui.");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("Vous ne pouvez pas échanger avec le même Pokémon.");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.ActionSelection)
            {
                prevState = null;
                StartCoroutine(RunTurns(BattleAction.SwitchPokemon));
            }
            else
            {
                state = BattleState.Busy;
                StartCoroutine(SwitchPokemon(selectedMember));
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (playerUnit.Pokemon.HP <= 0)
            {
                partyScreen.SetMessageText("Vous devez choisir un Pokémon pour continuer.");
                return;
            }

            partyScreen.gameObject.SetActive(false);

            if (prevState == BattleState.AboutToUse)
            {
                prevState = null;
                StartCoroutine(SendNextTrainerPokemon());
            }
            else
                ActionSelection();
        }
    }

    // Gère le moment où le dresseur adverse s'apprête à utiliser un nouveau Pokémon
    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
            dialogBox.UpdateChoiceBox(aboutToUseChoice);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            dialogBox.EnableChoiceBox(false);
            state = BattleState.Busy; // Bloque d'autres entrées
            if (aboutToUseChoice)
            {
                Debug.Log("Le joueur a choisi de changer de Pokémon.");
                prevState = state; // Mémorise l'état actuel
                OpenPartyScreen();
            }
            else
            {
                Debug.Log("Le joueur a choisi de ne pas changer de Pokémon.");
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
    }

    // Échange de Pokémon
    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Reviens {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Vas-y {newPokemon.Base.Name} !");

        if (prevState == null)
        {
            state = BattleState.RunningTurn;
        }
        else if (prevState == BattleState.AboutToUse)
        {
            prevState = null;
            StartCoroutine(SendNextTrainerPokemon());
        }
    }

    // Envoie le prochain Pokémon du dresseur
    IEnumerator SendNextTrainerPokemon()
    {
        Debug.Log("Envoi du prochain Pokémon du dresseur.");
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} envoie {nextPokemon.Base.Name} !");

        Debug.Log("Prochain Pokémon du dresseur envoyé. Reprise du combat.");
        state = BattleState.RunningTurn;
        yield return new WaitForSeconds(1); // Attendre la fin de l'animation du texte
        ActionSelection();  // Passez maintenant à la sélection d'action
    }

    // Lance une Pokéball
    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"Vous ne pouvez pas voler les Pokémon des dresseurs !");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name()} utilise une Pokéball !");

        var pokeballObj = Instantiate(pokeballSprite, playerUnit.transform.position - new Vector3(2, 0), Quaternion.identity);
        var pokeball = pokeballObj.GetComponent<SpriteRenderer>();

        // Animations
        yield return pokeball.transform.DOJump(enemyUnit.transform.position + new Vector3(0, 2), 2f, 1, 1f).WaitForCompletion();
        yield return enemyUnit.PlayCaptureAnimation();
        yield return pokeball.transform.DOMoveY(enemyUnit.transform.position.y - 1.3f, 0.5f).WaitForCompletion();

        int shakeCount = TryToCatchPokemon(enemyUnit.Pokemon);

        for (int i = 0; i < Mathf.Min(shakeCount, 3); ++i)
        {
            yield return new WaitForSeconds(0.5f);
            yield return pokeball.transform.DOPunchRotation(new Vector3(0, 0, 10f), 0.8f).WaitForCompletion();
        }

        if (shakeCount == 4)
        {
            // Pokémon est capturé
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} a été capturé");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} a été ajouté à votre équipe");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            // Pokémon s'est libéré
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} s'est libéré");
            else
                yield return dialogBox.TypeDialog($"Presque capturé");

            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }
    }

    // Tentative de capturer un Pokémon
    int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * 2 / (3 * pokemon.MaxHp);

        if (a >= 255)
            return 4;

        float b = 1048560 / Mathf.Sqrt(Mathf.Sqrt(16711680 / a));

        int shakeCount = 0;
        while (shakeCount < 4)
        {
            if (UnityEngine.Random.Range(0, 65535) >= b)
                break;

            ++shakeCount;
        }

        return shakeCount;
    }

    // Tentative de fuite du combat
    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"Vous ne pouvez pas fuir les combats contre les dresseurs !");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Vous avez fui en toute sécurité !");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Vous avez fui en toute sécurité !");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Impossible de fuir !");
                state = BattleState.RunningTurn;
            }
        }
    }

    // Réinitialise toutes les barres de calcul
    void test()
    {
        dialogBox.EnableCalculBar(false);
        dialogBox.EnableCalculBarMoyen(false);
        dialogBox.EnableCalculBarDifficile(false);

        dialogBox.EnableCalculBarSoustra(false);
        dialogBox.EnableCalculBarSoustraMoyen(false);
        dialogBox.EnableCalculBarSoustraDifficile(false);

        dialogBox.EnableCalculBarMulti(false);
        dialogBox.EnableCalculBarMultiMoyen(false);
        dialogBox.EnableCalculBarMultiDifficile(false);

        dialogBox.EnableCalculBarDivi(false);
        dialogBox.EnableCalculBarDiviMoyen(false);
        dialogBox.EnableCalculBarDiviDifficile(false);
    }
}
