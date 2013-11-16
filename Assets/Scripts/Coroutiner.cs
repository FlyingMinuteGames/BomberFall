using UnityEngine;
using System.Collections;

public class Coroutiner : MonoBehaviour
{

    // This will be null until after Awake()

    public delegate IEnumerator _coroutine();
    private static Coroutiner instance = null;
    public static Coroutiner Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        instance = this;
    }

    public Coroutine StartCoroutine(_coroutine co)
    {
        return this.StartCoroutine(co());
    }

}
