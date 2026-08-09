using UnityEngine;

public class CharacterModule : BaseModule<CharacterModule>
{
    private GameObject CharacterRoot;
    private CharacterController controller;
    private PlayerCamera playerCamera;
    private GameObject Test_Player;

    protected override void OnInit()
    {
        base.OnInit();

        LoadCharacterRoot();
        CreateTestPlayer();
    }

    protected override void OnRelease()
    {
        base.OnRelease();
    }

    private void LoadCharacterRoot()
    {
        CharacterRoot = GameManager.Instance.WorldRoot.GetProp("ObjectRoot");
    }

    public void CreateTestPlayer()
    {
        Debug.Log("�������Խ�ɫ");
        Test_Player = LoadManager.Instance.LoadPrefab("Prefab/Character/Test_Player.prefab", CharacterRoot.transform);
        controller = new CharacterController();
        controller.BindingPlayer(Test_Player);
        playerCamera = new PlayerCamera(Camera.main, Test_Player);
    }
}