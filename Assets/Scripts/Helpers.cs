using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    static Matrix4x4 _isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 45, 0));
    public static Vector3 ToIso(this Vector3 input) => _isoMatrix.MultiplyPoint3x4(input);
}
public static class ToIso
{
    public static float targetAngle { get; private set; } = 45f;

    public static void RotateTarget(float angleDelta)
    {
        targetAngle += angleDelta;

        targetAngle = Mathf.Repeat(targetAngle, 360f);

        Debug.Log($"Target Angle: {targetAngle}");
    }

    public static Vector3 SetToIso(this Vector3 input)
    {
        var isoMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, targetAngle, 0));
        return isoMatrix.MultiplyPoint3x4(input);
    }
}
