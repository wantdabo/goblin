using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Volatile;

public class VoltPhysics : MonoBehaviour
{
    private VoltWorld world;
    private VoltCircle voltCircle;
    private VoltPolygon voltPolygon;
    void Start()
    {
        world = new VoltWorld();
        voltCircle = world.CreateCircleWorldSpace(VoltVector2.zero, Fix64.Half);
        world.CreateDynamicBody(VoltVector2.zero, 0, voltCircle);

        voltPolygon = world.CreatePolygonWorldSpace(new VoltVector2[] {
            new VoltVector2(10, -2),
            new VoltVector2(10, -4),
            new VoltVector2(-10, -4),
            new VoltVector2(-10, -2),
        });
        world.CreateStaticBody(VoltVector2.zero, 0, voltPolygon);
    }

    private void OnDrawGizmos()
    {
        if (false == Application.isPlaying) return;
        var vec = new Vector3(voltCircle.Body.Position.x.AsFloat(), voltCircle.Body.Position.y.AsFloat(), 0);
        Gizmos.DrawWireSphere(vec, voltCircle.Radius.AsFloat());

        for (int i = 0; i < voltPolygon.worldVertices.Length - 1; i++)
        {
            Gizmos.DrawLine(voltPolygon.worldVertices[i].ToVector(), voltPolygon.worldVertices[i + 1].ToVector());
        }
        Gizmos.DrawLine(voltPolygon.worldVertices[0].ToVector(), voltPolygon.worldVertices[voltPolygon.worldVertices.Length - 1].ToVector());
    }

    private void FixedUpdate()
    {
        world.Update();
        var force = new VoltVector2(0, -2 * Fix64.EN1);
        voltCircle.Body.AddForce(force);
        //voltCircle.Body.Set(voltCircle.Body.Position + force, 0);
    }
}
