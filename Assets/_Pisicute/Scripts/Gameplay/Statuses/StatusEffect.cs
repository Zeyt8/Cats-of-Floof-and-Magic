using Unity.Netcode;

public class StatusEffect : INetworkSerializable
{
    public int duration;
    public bool isInfinite;
    protected int level;
    protected int amount;
    public virtual string Name => "";
    public virtual string Description => "";

    public StatusEffect()
    {
    }

    public StatusEffect(int duration, int level = 0, int amount = 0)
    {
        this.duration = duration;
        isInfinite = (duration == -1);
        this.level = level;
        this.amount = amount;
    }

    public virtual void OnTurnBegin(UnitObject unit) { }
    public virtual void OnEncounterStart(UnitObject unit) { }
    public virtual int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell) { return 0; }
    public virtual void OnSpellCast() { }
    public virtual void OnDealDamage(UnitObject self, UnitObject target, ref int damage) { }
    public virtual void OnTakeDamage(UnitObject self, ref int damage) { }
    public virtual void StatModifier(ref CatData data) { }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref duration);
        serializer.SerializeValue(ref isInfinite);
        serializer.SerializeValue(ref level);
        serializer.SerializeValue(ref amount);
    }
}
