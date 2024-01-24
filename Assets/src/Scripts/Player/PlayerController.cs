using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 200.0f;
    public float jumpForce = 2.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        
    }
    private void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 rotation = new Vector3(0.0f, moveHorizontal, 0.0f);
        Vector3 movement = transform.forward * moveVertical * speed * Time.deltaTime;

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation * rotationSpeed * Time.deltaTime));
        rb.MovePosition(rb.position + movement);

        if (Input.GetButtonDown("Jump"))
        {
            rb.AddForce(new Vector3(0, jumpForce, 0), ForceMode.Impulse);
        }
    }
}
