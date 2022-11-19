using GoblinFramework.Core;
using GoblinFramework.Render;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public static Shell Instance = null;
    public static RGEngine Engine = null;

    private void GameSettings()
    {
        Instance = this;
        Application.runInBackground = true;
    }

    private void Start()
    {
        GameSettings();
        Engine = GameEngine<RGEngine>.CreateGameEngine();
    }

    private void OnDestroy()
    {
        Engine.Destroy();
        Engine = null;
    }

    private void Update()
    {
        Engine.Ticker.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        Engine.Ticker.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        Engine.Ticker.FixedUpdate(Time.fixedDeltaTime);
    }
}
