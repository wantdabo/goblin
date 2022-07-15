using GoblinFramework.Common;
using GoblinFramework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public static Shell Instance;
    public static GameEngineComp Engine;

    private void Start()
    {
        Instance = this;

        Engine = new GameEngineComp();
        Engine.Engine = Engine;
        Engine.Create(Engine);
    }

    private void Update()
    {
        Engine.EngineTick.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        Engine.EngineTick.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Engine.EngineTick.FixedUpdate(Time.fixedDeltaTime);
    }
}
