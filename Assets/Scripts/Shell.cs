using GoblinFramework.Client.Common;
using GoblinFramework.Core;
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

    private void Update()
    {
        Engine.CTEngine.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        Engine.CTEngine.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Engine.CTEngine.FixedUpdate(Time.fixedDeltaTime);
    }
}
