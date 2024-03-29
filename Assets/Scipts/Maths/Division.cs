using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Division : MonoBehaviour
{
    private int number1;
    private int number2;
    private int numberAnswer;



    private int getnumber1()
    {
        return this.number1;
    }

    private void setnumber1(int number1)
    {
        this.number1 = number1;
    }


    private int getnumber2()
    {
        return this.number2;
    }
    private void setnumber2(int number2)
    {
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


    private void Start()
    {
        CreateNewQuestion();
    }



    private int CalculateAnswerFacile()
    {
        if (number2 > number1)
        {
            numberAnswer = number2 / number1;
        }
        else if (number2 < number1)
        {
            numberAnswer = number1 / number2;

        }
        return numberAnswer;
    }


    private List<int> CalculateAnswerMoyen()
    {
        numberAnswer = CalculateAnswerFacile();
        List<int> res = new List<int>();
        res.Add(numberAnswer);
        res.Add(number1);
        return res;
    }

    private int CalculateAnswerDifficile()
    {
        if (number2 > number1)
        {
            numberAnswer = - number2 / number1;
        }
        else if (number2 < number1)
        {
            numberAnswer = - number1 / number2;

        }
        List<int> res = new List<int>();
        res.Add(numberAnswer);
        res.Add(number1);
        return res;
    }


    public void CreateNewQuestion()
    {
        var rand = new Random();
        number2 = rand.Next(2000);
        number1 = rand.Next(2000);

        /// if () {
        ///  int reponse = CalculateAnswerFacile();
        /// }
        ///else if ()
        /// {
        ///int reponse = CalculateAnswerMoyen();
        ///}else if ()
        /// {
        /// int reponse = CalculateAnswerDifficile();
        ///}
    }
}
