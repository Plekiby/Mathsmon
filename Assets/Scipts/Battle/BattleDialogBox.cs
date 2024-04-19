using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int lettersPerSecond;
    [SerializeField] Color highlightedColor;

    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject choiceBox;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;


    [SerializeField] Text yesText;
    [SerializeField] Text noText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public void EnableCalculBar(bool enabled)
    {
        calculBar.SetActive(enabled);
    }
    public void EnableCalculBarMoyen(bool enabled)
    {
        calculBarMoyen.SetActive(enabled);
    }
    public void EnableCalculBarDifficile(bool enabled)
    {
        calculBarDifficile.SetActive(enabled);
    }
    ///////////////////////////Soustraction//////////////////////////////
    public void EnableCalculBarSoustra(bool enabled)
    {
        calculBarSoutraction.SetActive(enabled);
    }
    public void EnableCalculBarSoustraMoyen(bool enabled)
    {
        calculBarSoutraMoyen.SetActive(enabled);
    }
    public void EnableCalculBarSoustraDifficile(bool enabled)
    {
        calculBarSoutraDifficile.SetActive(enabled);
    }
    ///////////////////////////Multiplication//////////////////////////////
    public void EnableCalculBarMulti(bool enabled)
    {
        calculBarMulti.SetActive(enabled);
    }
    public void EnableCalculBarMultiMoyen(bool enabled)
    {
        calculBarMultiMoyen.SetActive(enabled);
    }
    public void EnableCalculBarMultiDifficile(bool enabled)
    {
        calculBarMultiDifficile.SetActive(enabled);
    }
    ///////////////////////////Division//////////////////////////////
    public void EnableCalculBarDivi(bool enabled)
    {
        calculBarDivi.SetActive(enabled);
    }
    public void EnableCalculBarDiviMoyen(bool enabled)
    {
        calculBarDiviMoyen.SetActive(enabled);
    }
    public void EnableCalculBarDiviDifficile(bool enabled)
    {
        calculBarDiviDifficile.SetActive(enabled);
    }
    //////////////////////////////////////////////////////////////////

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableChoiceBox(bool enabled)
    {
        choiceBox.SetActive(enabled);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectedMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "-";
        }
    }

    public void UpdateChoiceBox(bool yesSelected)
    {
        if (yesSelected)
        {
            yesText.color = highlightedColor;
            noText.color = Color.black;
        }
        else
        {
            yesText.color = Color.black;
            noText.color = highlightedColor;
        }
    }
}
