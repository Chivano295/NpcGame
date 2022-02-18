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
    }
    //starts the wave spawning
    private void Start()
    {
        StartCoroutine(WaveSpawner());
    }

    //All these functions below are assigned to buttons for selecting each statistic
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
        if (totalPoints > 0)
        {
            defense = defense + 1;
        }
    }
    public void DEF2()
    {
        if (defense > 0)
        {
            defense = defense - 1;
            MaxPTS1();
        }
    }
    public void DMG1()
    {
        if (totalPoints > 0)
        {
            attackDamage = attackDamage + 1;
        }
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
        if(totalPoints >= 0)
        {
            totalPoints -= 1;
            if(totalPoints <= 0)
            totalPoints = 0;
        }
        
        
    }
}
