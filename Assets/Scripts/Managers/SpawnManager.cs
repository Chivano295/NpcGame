using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SimpleBehaviorTree.Examples;

public class SpawnManager : MonoBehaviour
{
    [Header("Unit Stats")]
    [SerializeField] private int hp = 0;
    [SerializeField] private int defense = 0;
    [SerializeField] private int moveSpeed = 0;
    [SerializeField] private int attackDamage = 0;
    [SerializeField] private int attackSpeed = 0;
    [SerializeField] private int viewRange = 0;

    [Header("Prefabs")]
    [SerializeField] private GameObject unit;

    [Header("Private")]
    private List<GameObject> units = new List<GameObject>();

    public void ChangeHP(int newHP)
    {
        hp = newHP;
    }

    IEnumerator WaveSpawner()
    {
        yield return new WaitForSeconds(10);

        List<GameObject> newUnits = new List<GameObject>();

        for (int i = 0; i < 10; i++)
        {
            GameObject newUnit = Instantiate(unit);

            newUnits.Add(newUnit);
            units.Add(newUnit);
        }

        foreach (GameObject unit in newUnits)
        {
            AgentBrain brain = unit.GetComponent<AgentBrain>();

            brain.hp = hp;
            brain.defense = defense;
            brain.moveSpeed = moveSpeed;
            brain.attackDamage = attackDamage;
            brain.attackSpeed = attackSpeed;
            brain.viewRange = viewRange;
        }

        StartCoroutine(WaveSpawner());
    }


    private void Start()
    {
        StartCoroutine(WaveSpawner());
    }
}
