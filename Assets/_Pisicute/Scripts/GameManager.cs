using Unity.Collections;
using Unity.Netcode;

public class GameManager : NetworkPersistentSingleton<GameManager>
{
    public static NetworkVariable<FixedString128Bytes> SelectedMap = new NetworkVariable<FixedString128Bytes>();
}
