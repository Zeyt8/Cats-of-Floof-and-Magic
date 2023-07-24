using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; } = null;

    public virtual void Awake()
    {
        Instance = GetComponent<T>();
    }
}