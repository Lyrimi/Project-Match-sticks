using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class Player : MonoBehaviour
{
    public float Speed;
    public float SprintSpeed;

    Rigidbody2D rb;
    InputAction move;
    InputAction sprint;
    bool sprinting = false;

    // Start is called before the first frame update
    void Start()
    {
        move = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
        rb = GetComponent<Rigidbody2D>();
        sprint.started += OnSprintStart;
        sprint.canceled += OnSprintStop;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        Vector2 MoveDir = move.ReadValue<Vector2>();
        if (sprinting)
        {
            rb.velocity = MoveDir * SprintSpeed;
        }
        else
        {
            rb.velocity = MoveDir * Speed;
        }
    }

    void OnSprintStart(InputAction.CallbackContext context)
    {
       sprinting = true;
    }

    void OnSprintStop(InputAction.CallbackContext context) 
    {
        sprinting = false;
    }
}
