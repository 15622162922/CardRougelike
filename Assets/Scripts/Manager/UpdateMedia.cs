using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;

public class UpdateMedia : MonoBehaviour
{
    Action action_update;
    Action action_lateUpdate;
    Action action_fixedUpdate;

    public void Binding(Action update, Action lateUpdate, Action fixedUpdate)
    {
        this.action_update = update;
        this.action_lateUpdate = lateUpdate;
        this.action_fixedUpdate = fixedUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        action_update.Invoke();
    }

    private void LateUpdate()
    {
        action_lateUpdate.Invoke();
    }

    private void FixedUpdate()
    {
        action_fixedUpdate.Invoke();
    }
}
