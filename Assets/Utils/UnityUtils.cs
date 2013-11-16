using UnityEngine;
using System.Collections;

public class UnityUtils {

    public static void SetLayerRecursivlyOn(Transform t, int layer)
    {
        t.gameObject.layer = layer;
        foreach (Transform tc in t)
        {
            SetLayerRecursivlyOn(tc, layer);
        }
    }

}
