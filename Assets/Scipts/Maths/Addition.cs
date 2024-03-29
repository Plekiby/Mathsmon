using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Addition : MonoBehaviour
{
    private int number1;
    private int number2;
    private int numberAnswer;



    private int getnumber1()
    {
        return this.number1;
    }

    private void setnumber1(int number1) { 
        this.number1 = number1;
    }




    private int getnumber2()
    {
        return this.number2;
    }
    private void setnumber2(int number2) { 
        this.number2 = number2;
    }



    private int getAnswer()
    {
        return this.numberAnswer;
    }
    
    private void setAnswer(int number1)
    {
        this.numberAnswer = number1;
    }




    private void CalculateAnswer()
    {
        numberAnswer = number2 * number1;  
    }

   
    public void SimpleQuestion()
    {
        if (int.TryParse(_inputField.text, out int userAnswer) && userAnswer == numberAnswer)
        {
            _textFields[2].text = "Correct!";
        }
        else
        {
            _textFields[2].text = "Wrong, the correct answer is " + numberAnswer;
        }
    }




}
