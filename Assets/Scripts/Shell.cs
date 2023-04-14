using GoblinFramework.Core;
using GoblinFramework.Render;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public static Shell Instance = null;
    public static RGEngine engine = null;

    private void GameSettings()
    {
        Instance = this;
        Application.runInBackground = true;
    }

    private void Start()
    {
        GameSettings();
        engine = RGEngine.CreateGameEngine();
    }

    private void OnDestroy()
    {
        engine.Destroy();
        engine = null;
    }

    private void Update()
    {
        engine.ticker.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        engine.ticker.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        engine.ticker.FixedUpdate(Time.fixedDeltaTime);
    }
}
