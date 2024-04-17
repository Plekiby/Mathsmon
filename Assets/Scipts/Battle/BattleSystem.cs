using System.Collections;
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
    }
    public void OnAnswerSubmitted()
    {
        bool lastAnswerWasCorrect = false;

        if (activeGame == additionSimple)
            lastAnswerWasCorrect = additionSimple.AnswerQuestion();
        else if (activeGame == additionMoyen)
            lastAnswerWasCorrect = additionMoyen.AnswerQuestion();

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
        dialogBox.EnableCalculBar(false);
        dialogBox.EnableCalculBarMoyen(false);
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
        dialogBox.EnableCalculBar(false);
        dialogBox.EnableCalculBarMoyen(false);
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
                if (currentMove == 1)
                {
                    dialogBox.EnableCalculBarMoyen(true);
                    SetActiveGame("Moyen");
                    additionMoyen.NextQuestion();

                } //else
                  //{
                  //dialogBox.EnableCalculBar(true);//Calcul Dificile
                  //}
            }//if (playerUnit.Pokemon.Base.Name == "Additix")
        }
    }
}
