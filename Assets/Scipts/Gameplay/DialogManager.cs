using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;  
    [SerializeField] Text dialogText;       
    [SerializeField] int lettersPerSecond; 

    public event Action OnShowDialog;    
    public event Action OnCloseDialog;    

    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this; 
    }

    Dialog dialog;
    Action onDialogFinished;  

    int currentLine = 0; 
    bool isTyping; 

    public bool IsShowing { get; private set; } 

    // Coroutine pour afficher un dialogue
    public IEnumerator ShowDialog(Dialog dialog, Action onFinished = null)
    {
        yield return new WaitForEndOfFrame(); 

        OnShowDialog?.Invoke();  

        IsShowing = true;
        this.dialog = dialog;
        onDialogFinished = onFinished;

        dialogBox.SetActive(true); 
        StartCoroutine(TypeDialog(dialog.Lines[0])); 
    }

    // Gestion des entrées utilisateur pour passer les lignes de dialogue
    public void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.E) && !isTyping)  
        {
            ++currentLine;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine])); 
            }
            else
            {
                currentLine = 0;
                IsShowing = false;
                dialogBox.SetActive(false); 
                onDialogFinished?.Invoke(); 
                OnCloseDialog?.Invoke(); 
            }
        }
    }

    // Coroutine pour taper une ligne de dialogue
    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";  
        foreach (var letter in line.ToCharArray()) 
        {
            dialogText.text += letter;  
            yield return new WaitForSeconds(1f / lettersPerSecond); 
        }
        isTyping = false;  
    }

    // Afficher du texte de dialogue avec gestion de l'attente d'entrée utilisateur et fermeture automatique
    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true)
    {
        OnShowDialog?.Invoke(); 
        IsShowing = true;
        dialogBox.SetActive(true); 

        yield return TypeDialog(text);  
        if (waitForInput)
        {
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.E));  
        }

        if (autoClose)
        {
            CloseDialog();  
        }
    }

    // Fermer la boîte de dialogue
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        IsShowing = false;
        OnCloseDialog?.Invoke();  
    }
}