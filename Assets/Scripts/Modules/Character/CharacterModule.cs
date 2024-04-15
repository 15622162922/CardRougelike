using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModule : BaseModule<CharacterModule>
{
    GameObject CharacterRoot;
    GameObject Test_Player;
    CharacterController controller;
    PlayerCamera playerCamera;
    public override void Register()
    {
        base.Register();

        LoadCharacterRoot();
        CreateTestPlayer();
    }

    public override void UnRegister()
    {
        base.UnRegister();
    }

    void LoadCharacterRoot()
    {
        CharacterRoot = GameManager.Instance.WorldRoot.GetProp("ObjectRoot");
    }

    public void CreateTestPlayer()
    {
        Debug.Log("´´½¨²âÊÔ½ÇÉ«");
        Test_Player = LoadManager.Instance.LoadPrefab("Prefab/Character/Test_Player.prefab", CharacterRoot.transform);
        controller = new CharacterController();
        controller.BindingPlayer(Test_Player);
        playerCamera = new PlayerCamera(Camera.main, Test_Player);
    }
}
