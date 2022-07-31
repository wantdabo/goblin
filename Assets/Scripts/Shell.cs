using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoblinFramework.General;
using GoblinFramework.Gameplay;

public class Shell : MonoBehaviour
{
    public static Shell Instance = null;
    public static CGEngine Engine = null;

    private void GameSettings()
    {
        Instance = this;
        Application.runInBackground = true;
    }

    private void Start()
    {
        GameSettings();
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
