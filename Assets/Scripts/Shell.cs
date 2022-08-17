using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoblinFramework.General;
using GoblinFramework.Gameplay;
using GoblinFramework.Client.Common;

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
        Engine.Destroy();
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

    private void OnDrawGizmos()
    {
        DrawPhysicsCollisionLine();
    }

#if UNITY_EDITOR
    private void DrawPhysicsCollisionLine()
    {
        if (null == Engine) return;
        if (null == Engine.GameStage.State) return;
        var gamePlayingState = Engine.GameStage.State as GoblinFramework.Client.GameStages.GamePlayingState;
        if (null == gamePlayingState) return;

        List<GoblinFramework.Gameplay.Physics.Comps.Collider> colliders = null;
        gamePlayingState.PGEngine.World.GetColliders(ref colliders);

        Gizmos.color = Color.yellow;
        foreach (var collider in colliders)
        {
            if (collider is GoblinFramework.Gameplay.Physics.Comps.BoxCollider) 
            {
                var boxCollider = collider as GoblinFramework.Gameplay.Physics.Comps.BoxCollider;
                //var pos = boxCollider.Actor.ActorBehavior.Info.pos.ToU3DVector3();
                //var size = boxCollider.Actor.ActorBehavior.Info.size.ToU3DVector3();
                //pos.y += size.y / 2;

                Gizmos.DrawWireCube(boxCollider.colliderPos.ToU3DVector3(), boxCollider.colliderSize.ToU3DVector3());
            }
        }
    }
#endif
}
