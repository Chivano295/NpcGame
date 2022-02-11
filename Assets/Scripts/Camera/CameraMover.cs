using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private float CameraSpeed = 1;

    [SerializeField] private Transform target;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 offset;

    private void Update()
    {
        Vector3 moveDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
            moveDirection += new Vector3(1, 0, 1);
        if (Input.GetKey(KeyCode.A))
            moveDirection += new Vector3(-1, 0, 1);
        if (Input.GetKey(KeyCode.S))
            moveDirection += new Vector3(-1, 0, -1);
        if (Input.GetKey(KeyCode.D))
            moveDirection += new Vector3(1, 0, -1);

        moveDirection = moveDirection * Time.deltaTime * CameraSpeed;

        position += moveDirection;
    }

    private void FixedUpdate()
    {
        Vector3 pos = target != null ? target.position : position;

        pos += offset;

        this.transform.position = Vector3.Lerp(this.transform.position, pos, Time.deltaTime * CameraSpeed);
    }
}
