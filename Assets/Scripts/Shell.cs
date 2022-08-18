using GoblinFramework.Core;
using GoblinFramework.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        DrawPhysicsCollisionLine();
    }

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
                Gizmos.DrawWireCube(boxCollider.colliderPos.ToU3DVector3(), boxCollider.colliderSize.ToU3DVector3());
            }
            else if (collider is GoblinFramework.Gameplay.Physics.Comps.CylinderCollider)
            {
                var cylinderCollider = collider as GoblinFramework.Gameplay.Physics.Comps.CylinderCollider;
                Gizmos.DrawWireCube(cylinderCollider.colliderPos.ToU3DVector3(), cylinderCollider.colliderSize.ToU3DVector3());
            }
            else if (collider is GoblinFramework.Gameplay.Physics.Comps.SphereCollider)
            {
                var sphereCollider = collider as GoblinFramework.Gameplay.Physics.Comps.SphereCollider;
                Gizmos.DrawWireSphere(sphereCollider.colliderPos.ToU3DVector3(), sphereCollider.colliderSize.ToU3DVector3().x * 0.5f);
            }
            else if (collider is GoblinFramework.Gameplay.Physics.Comps.CapsuleCollider) 
            {
                var capsuleCollider = collider as GoblinFramework.Gameplay.Physics.Comps.CapsuleCollider;
                Gizmos.DrawWireCube(capsuleCollider.colliderPos.ToU3DVector3(), capsuleCollider.colliderSize.ToU3DVector3());
            }
        }
    }
#endif
}
