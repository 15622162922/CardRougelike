using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera
{
    Camera camera;
    GameObject targetPlayer;
    float cameraDistance = 10;
    public PlayerCamera(Camera camera, GameObject gameObject)
    {
        this.camera = camera;
        targetPlayer = gameObject;
        UpdateManager.Instance.RegisterLateUpdate(LateUpdate);
        InitCamera();
    }

    public void InitCamera()
    {
        Vector3 position = this.camera.transform.position;
        position.z = targetPlayer.transform.position.z - cameraDistance;
        this.camera.transform.position = position;
    }

    public void Destroy()
    {
        UpdateManager.Instance.RemoveLateUpdate(LateUpdate);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (targetPlayer != null)
        {
            Vector3 playerPos = targetPlayer.transform.position;

            playerPos.z -= cameraDistance;
            camera.transform.position = Vector3.Lerp(camera.transform.position, playerPos, 0.01f);
        }
    }
}
