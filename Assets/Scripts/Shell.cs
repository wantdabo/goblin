using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoblinFramework.General;

public class Shell : MonoBehaviour
{
    public static Shell Instance;
    public static CGEngine Engine = null;

    private void Start()
    {
        Instance = this;
        Engine = GameEngine<CGEngine>.CreateGameEngine();
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
