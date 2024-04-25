using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogBoxNew : MonoBehaviour
{

    [SerializeField] int lettersPerSecond;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject NameBar;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableNameBar(bool enabled)
    {
        NameBar.SetActive(enabled);
    }


}
