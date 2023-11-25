using UnityEngine;

public static class TransformExtension
{

    public static T GetOrAddComponent<T>(this Transform origin) where T : Component
    {
        T component = origin.GetComponent<T>();
        if (component == null)
        {
            component = origin.gameObject.AddComponent<T>();
        }

        return component;
    }

    public static T GetOrAddComponent<T>(this GameObject origin) where T : Component
    {
        T component = origin.GetComponent<T>();
        if (component == null)
        {
            component = origin.AddComponent<T>();
        }

        return component;
    }
}
