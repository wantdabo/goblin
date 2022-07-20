using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public static Shell Instance;
    public static CGEngineComp Engine;

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
        Engine.TickEngine.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        Engine.TickEngine.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Engine.TickEngine.FixedUpdate(Time.fixedDeltaTime);
    }
}
