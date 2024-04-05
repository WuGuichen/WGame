using UnityEngine;

public class PhysicUtils
{
    public static void GetHitPoint(Vector3 pA, Vector3 pB, Vector3 pC, Vector3 pD, out float s, out float t, out Vector3 c1, out Vector3 c2)
    {
        var d1 = pB - pA;
        var d2 = pD - pC;
        var r = pA - pC;

        var a = Vector3.Dot(d1, d1);
        var e = Vector3.Dot(d2, d2);

        var f = Vector3.Dot(d2, r);

        if (a <= Mathf.Epsilon && e <= Mathf.Epsilon)
        {
            s = t = 0.0f;
            c1 = pA;
            c2 = pC;
            return;
        }

        if (a <= Mathf.Epsilon)
        {
            s = 0.0f;
            t = f / e;
            t = Mathf.Clamp01(t);
        }
        else
        {
            var c = Vector3.Dot(d1, r);
            if (e <= Mathf.Epsilon)
            {
                t = 0.0f;
                s = Mathf.Clamp01(-c / a);
            }
            else
            {
                var b = Vector3.Dot(d1, d2);
                var denom = a * e - b * b;

                if (denom != 0.0f)
                    s = Mathf.Clamp01((b * f - c * e) / denom);
                else
                    s = 0.0f;

                t = (b * s + f) / e;

                if (t < 0.0f)
                {
                    t = 0.0f;
                    s = Mathf.Clamp01(-c / a);
                }
                else if (t > 1.0f)
                {
                    t = 1.0f;
                    s = Mathf.Clamp01((b - c) / a);
                }
            }
        }

        c1 = pA + d1 * s;
        c2 = pC + d2 * t;
    }
}
