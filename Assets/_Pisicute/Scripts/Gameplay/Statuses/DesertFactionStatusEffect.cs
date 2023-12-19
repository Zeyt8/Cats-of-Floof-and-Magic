public class DesertFactionStatusEffect : StatusEffect
{
    public override string Name => "Desert Faction";
    public override string Description => "Move faster on sand.";

    public DesertFactionStatusEffect(int duration, int level = 0, int amount = 0) : base(duration, level, amount)
    {
    }

    public override int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell)
    {
        base.OnMovementModifier(unit, fromCell, toCell);
        if (fromCell.TerrainTypeIndex == 0)
        {
            return -2;
        }
        else
        {
            return 0;
        }
    }
}
