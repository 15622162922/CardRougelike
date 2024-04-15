using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController
{
    Test_Player_Action player_action;
    GameObject player;
    UnityEngine.CharacterController controller;
    float moveSpeed = 6f;

    public CharacterController()
    {
        Init();
    }

    public void BindingPlayer(GameObject player)
    {
        this.player = player;
        controller = this.player.AddComponent<UnityEngine.CharacterController>();
    }

    void Init()
    {
        player_action = new Test_Player_Action();
        player_action.Player.Enable();
        player_action.Player.Test_Action.performed += Test_Action;
        //player_action.Player.Movement.performed += Movement_performed;

        UpdateManager.Instance.RegisterUpdate(UpdateAction);
    }

    void Close()
    {
        UpdateManager.Instance.RemoveUpdate(UpdateAction);
    }

    void UpdateAction()
    {
        Movement_Action();
    }

    void Test_Action(InputAction.CallbackContext context)
    {
        Debug.Log("Call Test_Action");
    }

    void Movement_performed(InputAction.CallbackContext context)
    {
        Debug.Log(context);
    }

    void Movement_Action()
    {
        Vector2 moveVector = player_action.Player.Movement.ReadValue<Vector2>();
        if (moveVector == Vector2.zero) return;
        Vector3 moveVector3 = new Vector3(moveVector.x, moveVector.y, 0);
        controller.Move(moveVector3 * moveSpeed * Time.deltaTime);
    }
}