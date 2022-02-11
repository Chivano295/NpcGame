using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    [SerializeField]
    int StarterHp = 100;
    [SerializeField]
    int Health = 100;
    [SerializeField]
    int Damage = 30;


    private void Start()
    {
        StarterHp = Health;
    }

}
