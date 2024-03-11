using Unity.Collections;
using Unity.Netcode;

public class StatusEffect : INetworkSerializable
{
    public FixedString32Bytes type;
    public int duration;
    public bool isInfinite;
    public int level;
    public int amount;
    public virtual string Name => "";
    public virtual string Description => "";

    public StatusEffect()
    {
    }

    public StatusEffect(int duration, int level = 0, int amount = 0)
    {
        type = GetType().ToString();
        this.duration = duration;
        isInfinite = (duration == -1);
        this.level = level;
        this.amount = amount;
    }

    public virtual void OnAdd(UnitObject unit) { }
    public virtual void OnRemove(UnitObject unit) { }
    public virtual void OnTurnBegin(UnitObject unit) { }
    public virtual void OnEncounterStart(UnitObject unit) { }
    public virtual int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell) { return 0; }
    public virtual void OnSpellCast(Leader caster, Spell spell) { }
    public virtual void OnDealDamage(UnitObject self, UnitObject target, ref int damage) { }
    public virtual void OnTakeDamage(UnitObject self, ref int damage) { }
    public virtual void StatModifier(ref CatData data) { }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref type);
        serializer.SerializeValue(ref duration);
        serializer.SerializeValue(ref isInfinite);
        serializer.SerializeValue(ref level);
        serializer.SerializeValue(ref amount);
    }
}
