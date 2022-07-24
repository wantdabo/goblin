using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoblinFramework.General;

public class Shell : MonoBehaviour
{
    public static Shell Instance;
    public static CGEngineComp Engine = null;

    private void Start()
    {
        Instance = this;
        Engine = GameEngineComp<CGEngineComp>.CreateGameEngine();
    }

    private void OnDestroy()
    {
        Engine?.Destroy();
        Engine = null;
    }

    private void Update()
    {
        Engine.CTickEngine.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        Engine.CTickEngine.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Engine.CTickEngine.FixedUpdate(Time.fixedDeltaTime);
    }
}
