using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoblinFramework.Common;

public class Shell : MonoBehaviour
{
    public static Shell Instance;
    public static CGEngineComp CEngine = null;

    private void Start()
    {
        Instance = this;
        CEngine = GameEngineComp<CGEngineComp>.CreateGameEngine();
    }

    private void OnDestroy()
    {
        CEngine?.Destroy();
        CEngine = null;
    }

    private void Update()
    {
        CEngine.TickEngine.Update(Time.deltaTime);
    }

    private void LateUpdate()
    {
        CEngine.TickEngine.LateUpdate(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        CEngine.TickEngine.FixedUpdate(Time.fixedDeltaTime);
    }
}
