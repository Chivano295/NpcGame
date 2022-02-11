using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Billboard : MonoBehaviour
{
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
