using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AdditionMoyen : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI number1, number2, Answer;
    private int _hiddenAnswer;

    private void Start()
    {
        CreateNewQuestion();
    }

    public bool AnswerQuestion()
    {
        int userInput;
        bool parseSuccess = int.TryParse(_inputField.text, out userInput);

        if (parseSuccess && userInput == this._hiddenAnswer)
        {
            Answer.text = "Bonne r�ponse!";
            return true;
        }
        else
        {
            if (!parseSuccess)
            {
                Answer.text = "Vous n'avez rien rentr� !!";
            }
            else
            {
                Answer.text = "Mauvaise r�ponse, la bonne r�ponse �tait " + this._hiddenAnswer;
                return false;
            }
        }
        return false;
    }

    public void NextQuestion()
    {
        Answer.text = "";
        _inputField.text = "";
        CreateNewQuestion();
    }
    private void CreateNewQuestion()
    {
        System.Random rnd = new System.Random();
        int _number1 = rnd.Next(0, 1000);
        int _number2 = rnd.Next(0, 1000);

        number1.text = _number1.ToString();
        number2.text = (_number1 + _number2).ToString();

        this._hiddenAnswer = _number2; 
    }

    public void OnSubmitButtonPressed()
    {
        bool lastAnswerWasCorrect = AnswerQuestion();
    }

}