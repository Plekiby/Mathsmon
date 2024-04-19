﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;
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






    private MonoBehaviour activeGame;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public void SetActiveGame(string gameType)
    {
        if (gameType == "Simple")
            activeGame = additionSimple;
        else if (gameType == "Moyen")
            activeGame = additionMoyen;
        else if(gameType =="Difficile")
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
        bool lastAnswerWasCorrect = false;

        if (activeGame == additionSimple)
            lastAnswerWasCorrect = additionSimple.AnswerQuestion();
        else if (activeGame == additionMoyen)
            lastAnswerWasCorrect = additionMoyen.AnswerQuestion();
        else if(activeGame == additionDifficile)
            lastAnswerWasCorrect = additionDifficile.AnswerQuestion();
        //////////////////////////////////////

        if (activeGame == soustractionSimple)
            lastAnswerWasCorrect = soustractionSimple.AnswerQuestion();
        else if (activeGame == soustractionMoyen)
            lastAnswerWasCorrect = soustractionMoyen.AnswerQuestion();
        else if(activeGame == soustractionDifficile)
            lastAnswerWasCorrect = soustractionDifficile.AnswerQuestion();
        //////////////////////////////////////

        if (activeGame == multiplicationSimple)
            lastAnswerWasCorrect = multiplicationSimple.AnswerQuestion();
        else if (activeGame == multiplicationMoyen)
            lastAnswerWasCorrect = multiplicationMoyen.AnswerQuestion();
        else if(activeGame == multiplicationDifficile)
            lastAnswerWasCorrect = multiplicationDifficile.AnswerQuestion();
        //////////////////////////////////////
        
        if (activeGame == divisionSimple)
            lastAnswerWasCorrect = divisionSimple.AnswerQuestion();
        else if (activeGame == divisionMoyen)
            lastAnswerWasCorrect = divisionMoyen.AnswerQuestion();
        else if (activeGame == divisionDifficile)
            lastAnswerWasCorrect = divisionDifficile.AnswerQuestion();

        StartCoroutine(PerformPlayerMove(lastAnswerWasCorrect));
    }



    public IEnumerator SetupBattle()
    {
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);

        dialogBox.SetMoveNames(playerUnit.Pokemon.Moves);

        yield return dialogBox.TypeDialog($"Un {enemyUnit.Pokemon.Base.Name} sauvage vous attaque.");
        yield return new WaitForSeconds(1f);

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        Masquer();
        StartCoroutine(dialogBox.TypeDialog("Choisissez une action"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    IEnumerator PerformPlayerMove(bool LastAnswerWasCorrect)
    {
        state = BattleState.Busy;
        
        var move = playerUnit.Pokemon.Moves[currentMove];
        Masquer();
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");
        bool isFainted = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);

        yield return new WaitForSeconds(1f);

        if (LastAnswerWasCorrect == true) {
            isFainted = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
            yield return enemyHud.UpdateHP();
            
        }
        else {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} a loupé {move.Base.Name}");
            StartCoroutine(EnemyMove()); 
            dialogBox.EnableCalculBar(false);
            dialogBox.EnableCalculBarMoyen(false);
        }


        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} Fainted");
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Pokemon.GetRandomMove();
        dialogBox.EnableDialogText(true);
        yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} used {move.Base.Name}");

        yield return new WaitForSeconds(1f);

        bool isFainted = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return playerHud.UpdateHP();

        if (isFainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted");
        }
        else
        {
            PlayerAction();
        }
    }

    private void Update()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentAction == 0)
            {
                // Fight
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                // Run
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            
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

            }else if (playerUnit.Pokemon.Base.Name == "Soustrix")
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

            }if (playerUnit.Pokemon.Base.Name == "Multiplix")
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

            }if (playerUnit.Pokemon.Base.Name == "Dividix")
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

        }
           
        
    }

    void Masquer()
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
