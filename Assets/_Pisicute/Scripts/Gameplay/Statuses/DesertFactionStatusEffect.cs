public class DesertFactionStatusEffect : StatusEffect
{
    public override string Name => "Desert Faction";
    public override string Description => "Move faster on sand.";

    public DesertFactionStatusEffect(int duration) : base(duration)
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
