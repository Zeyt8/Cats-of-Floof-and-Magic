using UnityEngine.Events;

public static class GameEvents
{
    public static UnityEvent<int> OnTurnStart = new UnityEvent<int>();
    public static UnityEvent<int> OnTurnEnd = new UnityEvent<int>();
    public static UnityEvent OnRoundEnd = new UnityEvent();

    public static UnityEvent<int> OnLeaderRecruited = new UnityEvent<int>();
}
