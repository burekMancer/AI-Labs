using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class playerInputController : MonoBehaviour
{
    CharacterController characterController;

    InputAction moveAction;
    InputAction lookAction;
    Camera playerCamera;
    float moveSpeed = 4f; //change this shi to 5 dont forget
    float rotSpeed = 31f;
    float yaw = 0f;
    float botch = 0f;

   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = Camera.main;
    }

    void Start()
    {
        Cursor.visible=false;
        moveAction = InputSystem.actions.FindAction("Move");
        lookAction = InputSystem.actions.FindAction("Look");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 moveValue = moveAction.ReadValue<Vector2>();
        Vector3 playerMovement = transform.forward * (moveValue.y * moveSpeed) + transform.right * (moveValue.x * moveSpeed);

        characterController.SimpleMove(playerMovement);

        Vector2 lookValue = lookAction.ReadValue<Vector2>();

        yaw += lookValue.x * rotSpeed * Time.deltaTime;
        botch -= lookValue.y * rotSpeed * Time.deltaTime;
        botch = Mathf.Clamp(botch, -89f, 89f);

        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        playerCamera.transform.rotation = Quaternion.Euler(botch, yaw, 0f);
    }
}
