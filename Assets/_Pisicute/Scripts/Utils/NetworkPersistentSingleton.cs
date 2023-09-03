using Unity.Netcode;

public class NetworkPersistentSingleton<T> : NetworkBehaviour where T : NetworkBehaviour
{
    public static T Instance { get; private set; } = null;

    public virtual void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject);
        }
    }
}
