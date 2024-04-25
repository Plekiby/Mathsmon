using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjects : MonoBehaviour
{
    //public static EssentialObjects Instance { get; private set; }
    private void Awake()
    {
 
     DontDestroyOnLoad(gameObject);
          
    }
 }
