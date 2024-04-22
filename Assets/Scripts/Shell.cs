using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Goblin.Core;
using UnityEngine;

/// <summary>
/// Shell/游戏入口
/// </summary>
public class Shell : MonoBehaviour
{
    private void GameSettings()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 120;
    }

    private void Start()
    {
        GameSettings();
        GoblinExport.InitGoblin();
    }

    private void Update()
    {
        GoblinExport.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        GoblinExport.FixedTick(Time.fixedDeltaTime);
    }
}