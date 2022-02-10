using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public GameObject openShop;
    public GameObject openbutton;
    public int currency = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void BuyEnemy()
    {
        currency = currency - 10;

    }
    public void OpenShop()
    {
        openShop.SetActive(true);
        openbutton.SetActive(false);
    }
    public void CloseShop()
    {
        openShop.SetActive(false);
        openbutton.SetActive(true);
    }


}
