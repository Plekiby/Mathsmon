using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vendeur : MonoBehaviour
{

    public void AddMathsball(PlayerControllers player)
    {
        player.NbMathsballp();
    }

    public void AddMathsball(PlayerControllers player, int count)
    {
        player.setNbMathsball(count);
    }




}
