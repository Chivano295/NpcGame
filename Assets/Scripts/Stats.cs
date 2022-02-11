using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    public int health = 100;
    public int currentHealth;
    public int damage = 34;
    public TextMeshProUGUI hp;


    public void Start()
    {
        currentHealth = health;
    }

    public void Update()
    {
        hp.text = "hp = " + currentHealth;
    }
    public void TakeDamage()
    {
        currentHealth = currentHealth - damage;


        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
