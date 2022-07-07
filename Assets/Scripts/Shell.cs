using GoblinFramework.Common;
using GoblinFramework.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    internal static GameEngineEntity GameEngine;

    private void Start()
    {
        GameEngine = new GameEngineEntity();
        GameEngine.GameEngine = GameEngine;
        GameEngine.Create(GameEngine);
    }

    private void Update()
    {
        GameEngine.Location.EngineTick.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        GameEngine.Location.EngineTick.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        GameEngine.Location.EngineTick.FixedUpdate(Time.fixedDeltaTime);
    }
}
