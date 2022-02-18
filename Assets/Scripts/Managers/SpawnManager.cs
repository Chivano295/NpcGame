using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleBehaviorTree.Examples;

public class SpawnManager : MonoBehaviour
{
    //enviremont variables
    [Header("Enviorment")]
    [SerializeField] private Transform unitFather;
    [SerializeField] private Transform spawnPosition;

    //team and unit prefab
    [Header("Prefabs")]
    [SerializeField] private GameObject unit;
    [SerializeField] private Color color;

    //statistics variables
    [Header("Unit Stats")]
    [SerializeField] private bool team;
    [SerializeField] private int hp = 0;
    [SerializeField] private int defense = 0;
    [SerializeField] private int moveSpeed = 0;
    [SerializeField] private int attackDamage = 0;
    [SerializeField] private int attackSpeed = 0;
    [SerializeField] private int viewRange = 0;

    [SerializeField] private int totalPoints = 100;

    [Header("Private")]
    private List<GameObject> units = new List<GameObject>();

    //text for stats assigning
    [Header("Text")]
    public TextMeshProUGUI HPpts;
    public TextMeshProUGUI DEFpts;
    public TextMeshProUGUI DMGpts;
    public TextMeshProUGUI SPDpts;
    public TextMeshProUGUI ATSPDpts;
    public TextMeshProUGUI VRpts;
    public TextMeshProUGUI TotalPTS;

    
    public void ChangeHP(int newHP)
    {
        hp = newHP;
    }

    
    private Vector3 RandomVector()
    {
        Vector3 position = Vector3.zero;

        position.x = Random.Range(-100, 100) / 100;
        position.z = Random.Range(-100, 100) / 100;

        return position.normalized;
    }

    //spawns an wave 
    IEnumerator WaveSpawner()
    {
        yield return new WaitForSeconds(10);

        List<GameObject> newUnits = new List<GameObject>();

        GameObject group = new GameObject("Unit Group");
        group.transform.parent = unitFather;

        //spawns the unit
        for (int i = 0; i < 10; i++)
        {
            GameObject newUnit = Instantiate(unit, spawnPosition.position + RandomVector(), Quaternion.identity, group.transform);

            newUnit.GetComponent<Renderer>().material.color = color;

            newUnits.Add(newUnit);
            units.Add(newUnit);
        }
        //sets the statistics onto the units
        foreach (GameObject unit in newUnits)
        {
            AgentBrain brain = unit.GetComponent<AgentBrain>();
            MyTeam myTeam = unit.GetComponent<MyTeam>();

            myTeam.team = team;
            brain.team = team;

            brain.hp           += hp;
            brain.defense      += defense;
            brain.moveSpeed    += moveSpeed;
            brain.attackDamage += attackDamage;
            brain.attackSpeed  += attackSpeed;
            brain.viewRange    += viewRange;
        }

        StartCoroutine(WaveSpawner());
    }

    //updates the text while choosing statistics
    public void Update()
    {
        HPpts.text = "" + hp;
        DEFpts.text = "" + defense;
        DMGpts.text = "" + attackDamage;
        SPDpts.text = "" + moveSpeed;
        ATSPDpts.text = "" + attackSpeed;
        VRpts.text = "" + viewRange;
        TotalPTS.text = "Total points = " + totalPoints;

    }
    //starts the wave spawning
    private void Start()
    {
        StartCoroutine(WaveSpawner());
    }

    //All these functions below are assigned to buttons for selecting each statistic
    public void HP1()
    {
        if (totalPoints > 0)
        {
            hp = hp + 5;
        }
    }
    public void HP2()
    {
        if (hp > 0)
        {
            hp = hp - 5;
            MaxPTS1();
        }

    }
    public void DEF1()
    {
        if (totalPoints > 5)
        {
            defense = defense + 5;
        }
    }
    public void DEF2()
    {
        if (defense > 0)
        {
            defense = defense - 5;
            MaxPTS1();
        }
    }
    public void DMG1()
    {
        if (totalPoints > 0)
        {
            attackDamage = attackDamage + 5;
        }
    }
    public void DMG2()
    {
        if (attackDamage > 0)
        {
            attackDamage = attackDamage - 5;
            MaxPTS1();
        }
    }
    public void SPD1()
    {
        if (totalPoints > 0)
        {
            moveSpeed = moveSpeed + 5;
        }
    }
    public void SPD2()
    {
        if (moveSpeed > 0)
        {
            moveSpeed = moveSpeed - 5;
            MaxPTS1();
        }
    }
    public void ATSPD1()
    {
        if (totalPoints > 0)
        {
            attackSpeed = attackSpeed + 5;
        }
    }
    public void ATSPD2()
    {
        if (attackSpeed > 0)
        {
            attackSpeed = attackSpeed - 5;
            MaxPTS1();
        }
    }
    public void VR1()
    {
        if (totalPoints > 0)
        {
            viewRange = viewRange + 5;
        }
    }
    public void VR2()
    {
        if (viewRange > 0)
        {
            viewRange = viewRange - 5;
            MaxPTS1();
        }
    }
    public void MaxPTS1()
    {
        totalPoints += 5;
    }
    public void MaxPTS2()
    {
        if (totalPoints >= 0)
        {
            totalPoints -= 5;

            if (totalPoints <= 0)
                totalPoints = 0;
        }

    }
}
