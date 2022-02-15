using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleBehaviorTree.Examples;

public class SpawnManager : MonoBehaviour
{
    [Header("Enviorment")]
    [SerializeField] private Transform unitFather;

    [Header("Prefabs")]
    [SerializeField] private GameObject unit;

    [Header("Unit Stats")]
    [SerializeField] private int hp = 0;
    [SerializeField] private int defense = 0;
    [SerializeField] private int moveSpeed = 0;
    [SerializeField] private int attackDamage = 0;
    [SerializeField] private int attackSpeed = 0;
    [SerializeField] private int viewRange = 0;

    [SerializeField] private int totalPoints = 100;

    [Header("Private")]
    private List<GameObject> units = new List<GameObject>();

    [Header("Text")]
    public TextMeshProUGUI HPpts;
    public TextMeshProUGUI DEFpts;
    public TextMeshProUGUI DMGpts;
    public TextMeshProUGUI SPDpts;
    public TextMeshProUGUI ATSPDpts;
    public TextMeshProUGUI VRpts;

    public void ChangeHP(int newHP)
    {
        hp = newHP;
    }

    IEnumerator WaveSpawner()
    {
        yield return new WaitForSeconds(10);

        List<GameObject> newUnits = new List<GameObject>();

        GameObject group = new GameObject("Unit Group");
        group.transform.parent = unitFather;

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

    public void Update()
    {
        HPpts.text = "" + hp;
        DEFpts.text = "" + defense;
        DMGpts.text = "" + attackDamage;
        SPDpts.text = "" + moveSpeed;
        ATSPDpts.text = "" + attackSpeed;
        VRpts.text = "" + viewRange;
    }

    private void Start()
    {
        StartCoroutine(WaveSpawner());
    }

    public void HP1()
    {
        if( totalPoints > 0)
        {
            hp = hp + 1;
        }
    }
    public void HP2()
    {
        if (hp > 0)
        {
            hp = hp - 1;
            MaxPTS1();
        }
           
    }
    public void DEF1()
    {
        defense = defense + 1;
    }
    public void DEF2()
    {
        defense = defense - 1;
    }
    public void DMG1()
    {
        attackDamage = attackDamage + 1;
    }
    public void DMG2()
    {
        attackDamage = attackDamage - 1;
    }
    public void SPD1()
    {
        moveSpeed = moveSpeed + 1;
    }
    public void SPD2()
    {
        moveSpeed = moveSpeed - 1;
    }
    public void ATSPD1()
    {
        attackSpeed = attackSpeed + 1;
    }
    public void ATSPD2()
    {
        attackSpeed = attackSpeed - 1;
    }
    public void VR1()
    {
        viewRange = viewRange + 1;
    }
    public void VR2()
    {
        viewRange = viewRange - 1;
    }
    public void MaxPTS1()
    {
        totalPoints += 1;
    }
    public void MaxPTS2()
    {
        totalPoints -= 1;
    }

}
