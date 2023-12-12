public class DesertFactionStatusEffect : StatusEffect
{
    public DesertFactionStatusEffect(int duration) : base(duration)
    {
    }

    public override int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell)
    {
        base.OnMovementModifier(unit, fromCell, toCell);
        if (fromCell.TerrainTypeIndex == 0)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }
}
