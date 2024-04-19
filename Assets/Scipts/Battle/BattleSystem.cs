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
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;

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

    BattleState state;
    BattleState? prevState;
    int currentAction;
    int currentMove;
    int currentMember;
    bool aboutToUseChoice = true;
    private MonoBehaviour activeGame;
    bool lastAnswerWasCorrect = false;



    PokemonParty playerParty;
    PokemonParty trainerParty;
    Pokemon wildPokemon;

    bool isTrainerBattle = false;
    PlayerControllers player;
    TrainerController trainer;

    int escapeAttempts;

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.playerParty = playerParty;
        this.wildPokemon = wildPokemon;
        player = playerParty.GetComponent<PlayerControllers>();
        isTrainerBattle = false;

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.playerParty = playerParty;
        this.trainerParty = trainerParty;

        isTrainerBattle = true;
        player = playerParty.GetComponent<PlayerControllers>();
        trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        playerUnit.Clear();
        enemyUnit.Clear();

        if (!isTrainerBattle)
        {
            playerUnit.Setup(playerParty.GetHealthyPokemon());
            enemyUnit.Setup(wildPokemon);
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        }
        else
        {
            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = player.Sprite;
            trainerImage.sprite = trainer.Sprite;
            yield return dialogBox.TypeDialog($"{trainer.Name} wants to battle");

            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = trainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{trainer.Name} send out {enemyPokemon.Base.Name}");

            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = playerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        escapeAttempts = 0;
        partyScreen.Init();
        yield return new WaitForSeconds(1);  // Ensure all dialogues are completed
        ActionSelection();  // Properly initiate action selection
    }

    public void SetActiveGame(string gameType)
    {
        if (gameType == "Simple")
            activeGame = additionSimple;
        else if (gameType == "Moyen")
            activeGame = additionMoyen;
        else if (gameType == "Difficile")
            activeGame = additionDifficile;
        ///////////////////////////////////
        else if (gameType == "soustractionSimple")
            activeGame = soustractionSimple;
        else if (gameType == "soustractionMoyen")
            activeGame = soustractionMoyen;
        else if (gameType == "soustractionDifficile")
            activeGame = soustractionDifficile;
        ///////////////////////////////////
        else if (gameType == "multiplicationSimple")
            activeGame = multiplicationSimple;
        else if (gameType == "multiplicationMoyen")
            activeGame = multiplicationMoyen;
        else if (gameType == "multiplicationDifficile")
            activeGame = multiplicationDifficile;
        //////////////////////////////////////
        else if (gameType == "divisionSimple")
            activeGame = divisionSimple;
        else if (gameType == "divisionMoyen")
            activeGame = divisionMoyen;
        else if (gameType == "divisionDifficile")
            activeGame = divisionDifficile;
    }


    public void OnAnswerSubmitted()
    {


        if (activeGame == additionSimple)
            lastAnswerWasCorrect = additionSimple.AnswerQuestion();
        else if (activeGame == additionMoyen)
            lastAnswerWasCorrect = additionMoyen.AnswerQuestion();
        else if (activeGame == additionDifficile)
            lastAnswerWasCorrect = additionDifficile.AnswerQuestion();
        //////////////////////////////////////

        if (activeGame == soustractionSimple)
            lastAnswerWasCorrect = soustractionSimple.AnswerQuestion();
        else if (activeGame == soustractionMoyen)
            lastAnswerWasCorrect = soustractionMoyen.AnswerQuestion();
        else if (activeGame == soustractionDifficile)
            lastAnswerWasCorrect = soustractionDifficile.AnswerQuestion();
        //////////////////////////////////////

        if (activeGame == multiplicationSimple)
            lastAnswerWasCorrect = multiplicationSimple.AnswerQuestion();
        else if (activeGame == multiplicationMoyen)
            lastAnswerWasCorrect = multiplicationMoyen.AnswerQuestion();
        else if (activeGame == multiplicationDifficile)
            lastAnswerWasCorrect = multiplicationDifficile.AnswerQuestion();
        //////////////////////////////////////

        if (activeGame == divisionSimple)
            lastAnswerWasCorrect = divisionSimple.AnswerQuestion();
        else if (activeGame == divisionMoyen)
            lastAnswerWasCorrect = divisionMoyen.AnswerQuestion();
        else if (activeGame == divisionDifficile)
            lastAnswerWasCorrect = divisionDifficile.AnswerQuestion();

    }


    void BattleOver(bool won)
    {
        state = BattleState.BattleOver;
        playerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    void ActionSelection()
    {
        if (state != BattleState.AboutToUse && state != BattleState.PartyScreen && state != BattleState.MoveSelection)
        {
            Debug.Log("Entering ActionSelection State");
            state = BattleState.ActionSelection;
            dialogBox.SetDialog("Choose an action");
            dialogBox.EnableActionSelector(true);
        }
        else
        {
            Debug.LogError("Attempted to enter ActionSelection incorrectly from " + state.ToString());
        }
    }

    void OpenPartyScreen()
    {
        prevState = state;  // Store the current state to return after party selection
        state = BattleState.PartyScreen;
        partyScreen.SetPartyData(playerParty.Pokemons);
        partyScreen.gameObject.SetActive(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator AboutToUse(Pokemon newPokemon)
    {
        yield return dialogBox.TypeDialog($"{trainer.Name} is about to use {newPokemon.Base.Name}. Do you want to change pokemon?");
        dialogBox.EnableChoiceBox(true);

        state = BattleState.AboutToUse;
        while (state == BattleState.AboutToUse)
        {
            yield return null; // Wait here until a choice is made
        }

        dialogBox.EnableChoiceBox(false);
    }

    IEnumerator RunTurns(BattleAction playerAction)
    {
        state = BattleState.RunningTurn;

        if (playerAction == BattleAction.Move)
        {
            playerUnit.Pokemon.CurrentMove = playerUnit.Pokemon.Moves[currentMove];
            enemyUnit.Pokemon.CurrentMove = enemyUnit.Pokemon.GetRandomMove();

            // Check who goes first
            bool playerGoesFirst = true;
            playerGoesFirst = playerUnit.Pokemon.Speed >= enemyUnit.Pokemon.Speed;

            var firstUnit = (playerGoesFirst) ? playerUnit : enemyUnit;
            var secondUnit = (playerGoesFirst) ? enemyUnit : playerUnit;

            var secondPokemon = secondUnit.Pokemon;

            // First Turn
            yield return RunMove(firstUnit, secondUnit, firstUnit.Pokemon.CurrentMove);
            //yield return RunAfterTurn(firstUnit);
            if (state == BattleState.BattleOver) yield break;

            if (secondPokemon.HP > 0)
            {
                // Second Turn
                yield return RunMove(secondUnit, firstUnit, secondUnit.Pokemon.CurrentMove);
                //yield return RunAfterTurn(secondUnit);
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
                yield return ThrowPokeball();
            }
            else if (playerAction == BattleAction.Run)
            {
                yield return TryToEscape();
            }

            // Enemy Turn
            var enemyMove = enemyUnit.Pokemon.GetRandomMove();
            yield return RunMove(enemyUnit, playerUnit, enemyMove);
            // yield return RunAfterTurn(enemyUnit);
            if (state == BattleState.BattleOver) yield break;
        }

        if (state != BattleState.BattleOver)
            ActionSelection();
    }

    

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        // Reset all calcul bars visibility
        test();

        // Check if it's the player's turn and set up the correct game
        if (sourceUnit.IsPlayerUnit)
        {
            ActivateCalculBar(playerUnit.Pokemon.Base.Name, currentMove);
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return)); // Wait for player to press Enter

            OnAnswerSubmitted(); // Check the answer after submission

            test();

            if (!lastAnswerWasCorrect) // If the answer is wrong, skip the attack
            {
                yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} tried to use {move.Base.Name} but got the math wrong!");
                yield break; // Skip the attack
            }
        }

        // If the answer is right or if it's the enemy's turn, perform the attack
        dialogBox.SetDialog($"{sourceUnit.Pokemon.Base.Name} uses {move.Base.Name}!");
        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return targetUnit.Hud.UpdateHP();

        if (targetUnit.Pokemon.HP <= 0)
        {
            yield return HandlePokemonFainted(targetUnit);
        }
    }

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






    IEnumerator HandlePokemonFainted(BattleUnit faintedUnit)
    {
        yield return dialogBox.TypeDialog($"{faintedUnit.Pokemon.Base.Name} Fainted");
        faintedUnit.PlayFaintAnimation();
        yield return new WaitForSeconds(2f);

        if (faintedUnit.IsPlayerUnit)
        {
            // Check for another player Pokémon to continue fighting
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();  // prompt player to select another Pokémon
            else
                BattleOver(false);  // no healthy Pokémon left
        }
        else
        {
            // Exp Gain
            int expGain = 1;
            playerUnit.Pokemon.GainWin();
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} gained {expGain} exp");
            yield return playerUnit.Hud.SetExpSmooth();

            // Check Level Up
            while (playerUnit.Pokemon.CheckForLevelUp())
            {
                playerUnit.Hud.SetLevel();
                yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} grew to level {playerUnit.Pokemon.Level}");

                yield return playerUnit.Hud.SetExpSmooth(true);
            }

            yield return new WaitForSeconds(1f);


            // Enemy Pokémon fainted, check for next in line
            if (isTrainerBattle)
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));  // prompt about switching Pokémon
                else
                    BattleOver(true);  // no Pokémon left, player wins
            }
            else
            {
                BattleOver(true);  // wild Pokémon battle, automatically win
            }
        }
    }


    void CheckForBattleOver(BattleUnit faintedUnit)
    {
        /*if (faintedUnit.IsPlayerUnit)
        {
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();
            else
                BattleOver(false);
        }
        else
        {
            if (!isTrainerBattle)
            {
                BattleOver(true);
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));
                else
                    BattleOver(true);
            }
        }*/

        if (faintedUnit.IsPlayerUnit)
        {
            // Player's pokemon fainted
            var nextPokemon = playerParty.GetHealthyPokemon();
            if (nextPokemon != null)
                OpenPartyScreen();  // Switch to another pokemon
            else
                BattleOver(false);  // No healthy pokemon left, player loses
        }
        else
        {
            // Enemy's pokemon fainted
            if (!isTrainerBattle)
            {
                BattleOver(true);  // Wild pokemon battle won
            }
            else
            {
                var nextPokemon = trainerParty.GetHealthyPokemon();
                if (nextPokemon != null)
                    StartCoroutine(AboutToUse(nextPokemon));  // Ask player if they want to switch
                else
                    BattleOver(true);  // Trainer has no more pokemons, player wins
            }
        }
    }


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
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Bag
                StartCoroutine(RunTurns(BattleAction.UseItem));
            }
            else if (currentAction == 2)
            {
                // Pokemon
                prevState = state;
                OpenPartyScreen();
            }
            else if (currentAction == 3)
            {
                // Run
                StartCoroutine(RunTurns(BattleAction.Run));
            }
        }
    }

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
            if (playerUnit.Pokemon.Base.Name == "Multiplix")
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
            if (playerUnit.Pokemon.Base.Name == "Dividix")
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
                partyScreen.SetMessageText("You can't send out a fainted pokemon");
                return;
            }
            if (selectedMember == playerUnit.Pokemon)
            {
                partyScreen.SetMessageText("You can't switch with the same pokemon");
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
                partyScreen.SetMessageText("You have to choose a pokemon to continue");
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

    void HandleAboutToUse()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            aboutToUseChoice = !aboutToUseChoice;
            dialogBox.UpdateChoiceBox(aboutToUseChoice);
        }

        if (Input.GetKeyDown(KeyCode.E))
        { // Assume Z is the confirm button
            dialogBox.EnableChoiceBox(false);
            state = BattleState.Busy; // Block other inputs
            if (aboutToUseChoice)
            {
                Debug.Log("Player chose to switch Pokémon.");
                prevState = state; // Remember the current state
                OpenPartyScreen();
            }
            else
            {
                Debug.Log("Player chose not to switch Pokémon.");
                StartCoroutine(SendNextTrainerPokemon());
            }
        }
    }



    IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        dialogBox.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");

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

    IEnumerator SendNextTrainerPokemon()
    {
        Debug.Log("Sending out next trainer Pokémon.");
        state = BattleState.Busy;

        var nextPokemon = trainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{trainer.Name} send out {nextPokemon.Base.Name}!");

        Debug.Log("Next trainer Pokémon sent out. Resuming battle.");
        state = BattleState.RunningTurn;
        yield return new WaitForSeconds(1); // Wait for the text animation to complete
        ActionSelection();  // Now move to action selection
    }



    IEnumerator ThrowPokeball()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't steal the trainers pokemon!");
            state = BattleState.RunningTurn;
            yield break;
        }

        yield return dialogBox.TypeDialog($"{player.Name} used POKEBALL!");

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
            // Pokemon is caught
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} was caught");
            yield return pokeball.DOFade(0, 1.5f).WaitForCompletion();

            playerParty.AddPokemon(enemyUnit.Pokemon);
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} has been added to your party");

            Destroy(pokeball);
            BattleOver(true);
        }
        else
        {
            // Pokemon broke out
            yield return new WaitForSeconds(1f);
            pokeball.DOFade(0, 0.2f);
            yield return enemyUnit.PlayBreakOutAnimation();

            if (shakeCount < 2)
                yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} broke free");
            else
                yield return dialogBox.TypeDialog($"Almost caught it");

            Destroy(pokeball);
            state = BattleState.RunningTurn;
        }
    }

    int TryToCatchPokemon(Pokemon pokemon)
    {
        float a = (3 * pokemon.MaxHp - 2 * pokemon.HP) * pokemon.Base.CatchRate * 1 / (3 * pokemon.MaxHp);

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

    IEnumerator TryToEscape()
    {
        state = BattleState.Busy;

        if (isTrainerBattle)
        {
            yield return dialogBox.TypeDialog($"You can't run from trainer battles!");
            state = BattleState.RunningTurn;
            yield break;
        }

        ++escapeAttempts;

        int playerSpeed = playerUnit.Pokemon.Speed;
        int enemySpeed = enemyUnit.Pokemon.Speed;

        if (enemySpeed < playerSpeed)
        {
            yield return dialogBox.TypeDialog($"Ran away safely!");
            BattleOver(true);
        }
        else
        {
            float f = (playerSpeed * 128) / enemySpeed + 30 * escapeAttempts;
            f = f % 256;

            if (UnityEngine.Random.Range(0, 256) < f)
            {
                yield return dialogBox.TypeDialog($"Ran away safely!");
                BattleOver(true);
            }
            else
            {
                yield return dialogBox.TypeDialog($"Can't escape!");
                state = BattleState.RunningTurn;
            }
        }



    }

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