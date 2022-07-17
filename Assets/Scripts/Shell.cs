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
