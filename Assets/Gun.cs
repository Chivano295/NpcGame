
using UnityEngine;

public class Gun : MonoBehaviour
{
   
    public float range = 100f;

    public Camera fpsCam;

    // Update is called once per frame
    void Update()
    {
        //when you press mousebutton it calls the shoot function
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }

    }
    public void Shoot()
    {       
        RaycastHit hit;
        //checks if you hit something with the raycast and puts it in debug
       if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
           enemy enemy =  hit.transform.GetComponent<enemy>();
            if(enemy != null)
            {
                enemy.TakeDamage();
            }
        }
    }
}
