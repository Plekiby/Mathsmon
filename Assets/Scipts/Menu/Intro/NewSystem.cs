using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewSystem : MonoBehaviour
{
    [SerializeField] DialogBoxNew dialogBox;
    [SerializeField] private TMP_InputField _inputField;
    private string _name;
    void Start()
    {
        StartCoroutine(Introduction());
    }

    IEnumerator Introduction()
    {
        dialogBox.EnableDialogText(true);
        yield return dialogBox.TypeDialog($"Bien le bonjour! Bienvenue dans le monde magique des Mathsmon!");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"Mon nom est Chen! Les gens m'appellent souvent le Prof Mathsmon!");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"Ce monde est peuplé de créatures du nom de Mathsmon! Pour certains, les Mathsmon sont des animaux domestiques, pour d'autres, ils sont un moyen de combattre.");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"Pour ma part... L'étude des Mathsmon est ma profession.");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"Ta quête des Mathsmon est sur le point de commencer! Un tout nouveau monde de rêves, d'aventures et de Mathsmon t'attend!");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"C'est dingue non ?");
        yield return new WaitForSeconds(1f);
        yield return dialogBox.TypeDialog($"Mais tout d'abord, quel est ton nom?");
        yield return new WaitForSeconds(1f);

        dialogBox.EnableDialogText(false);
        dialogBox.EnableNameBar(true);

        // Attendre que l'utilisateur appuie sur la barre d'espace
        bool done = false;
        while (!done)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                done = true;
                this._name = _inputField.text;
                dialogBox.EnableDialogText(true);
                dialogBox.EnableNameBar(false);
                yield return dialogBox.TypeDialog($"OK! Ton nom est donc {_name}");
                yield return new WaitForSeconds(1f);
                yield return dialogBox.TypeDialog($"Va !! Combat et capture de nombreux Mathsmon, {_name}");
                yield return new WaitForSeconds(1f);
                yield return dialogBox.TypeDialog($"N'oublie pas de vaincre les mathématiciens, ils sont des maîtres de leur matières et t'apprendrons beaucoup de chose concernant les calculs mathématiques !!");
                yield return new WaitForSeconds(1f); 
                yield return dialogBox.TypeDialog($"Mais d'abord rejoins-moi vite dans mon laboratoire, j'ai une suprise pour toi ! À tout de suite !!");
                yield return new WaitForSeconds(1f);
             
                SceneManager.LoadScene("Chambre_PP");
            }
            yield return null;
        }
    }
}
