using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DivisionDifficile : MonoBehaviour
{
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private TextMeshProUGUI number1, number2, Answer;
    private int _hiddenAnswer;

    private void Start()
    {
        CreateNewQuestion();
    }

    private int CalculateAnswer(int _number1, int _number2)
    {
        return _number1 / _number2;
    }

    public bool AnswerQuestion()
    {
        int userInput;
        bool parseSuccess = int.TryParse(_inputField.text, out userInput);
        if (parseSuccess && userInput == _hiddenAnswer)
        {
            Answer.text = "Bonne réponse!";
            return true;
        }
        else
        {
            Answer.text = !parseSuccess ? "Vous n'avez rien rentré !!" :
                           "Mauvaise réponse, la bonne réponse était " + _hiddenAnswer;
            return false;
        }
    }

    public void NextQuestion()
    {
        Answer.text = "";
        _inputField.text = "";
        CreateNewQuestion();
    }

    public void CreateNewQuestion()
    {
        System.Random rnd = new System.Random();
        int _number1 = rnd.Next(10, 1000);  // Ensures a range suitable for challenging divisions
        int _number2 = rnd.Next(10, 100);   // Avoids too small divisors for added difficulty

        number1.text = _number1.ToString();
        number2.text = _number2.ToString();
        _hiddenAnswer = CalculateAnswer(_number1, _number2);
    }

    public void OnSubmitButtonPressed()
    {
        bool lastAnswerWasCorrect = AnswerQuestion();
    }
}
