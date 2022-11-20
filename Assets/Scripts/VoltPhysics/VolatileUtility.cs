using System;
using System.Collections.Generic;
using FixMath.NET;
using UnityEngine;

using Volatile;

public static class VolatileUtility
{
    public static Vector2 ToVector(this VoltVector2 vec)
    {
        return new Vector2((float)vec.x, (float)vec.y);
    }

    public static VoltVector2 ToFixed(this Vector2 vec)
    {
        return new VoltVector2((Fix64)vec.x, (Fix64)vec.y);
    }

    public static VoltVector2 ToFixed(this Vector3 vec)
    {
        return ToFixed((Vector2)vec);
    }

    public static Fix64 ToFixed(this float f)
    {
        return (Fix64)f;
    }
}
