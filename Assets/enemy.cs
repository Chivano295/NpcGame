using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class enemy : MonoBehaviour
{

    public int health = 100;
    public int currentHealth;
    public int damage = 34;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI dmg;

    public void Start()
    {
        currentHealth = health;
    }

    public void Update()
    {
        hp.text = "hp =" + currentHealth;
        dmg.text = "dmg =" + damage;
    }
    public void TakeDamage()
    {
        currentHealth = currentHealth - damage;

       
        if(currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }

}
