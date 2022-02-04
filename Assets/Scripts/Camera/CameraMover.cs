using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 offset;

    private void FixedUpdate()
    {
        Vector3 pos = target != null ? target.position : position;

        pos = pos + offset; new Vector3(-45, 67.5f, -45);

        this.transform.position = pos;
    }
}
