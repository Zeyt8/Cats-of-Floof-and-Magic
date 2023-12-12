public class WildFactionStatusEffect : StatusEffect
{
    private int level;
    public WildFactionStatusEffect(int level, int duration) : base(duration)
    {
        this.level = level;
    }

    public override int OnMovementModifier(UnitObject unit, HexCell fromCell, HexCell toCell)
    {
        base.OnMovementModifier(unit, fromCell, toCell);
        if (level >= 2)
        {
            if (fromCell.TerrainTypeIndex == 1)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
        else
        {
            return 0;
        }
    }
}
