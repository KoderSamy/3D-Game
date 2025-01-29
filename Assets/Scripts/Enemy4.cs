using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy4 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float moveDistance = 2f;
    private Vector3 startPosition;
    private bool movingRight = true;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (movingRight)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime, Space.Self); // Space.Self!
            if (transform.localPosition.x > moveDistance)
            {
                movingRight = false;
            }
        }
        else
        {
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime, Space.Self); // Space.Self!
            if (transform.localPosition.x < -moveDistance)
            {
                movingRight = true;
            }
        }
    }
}
