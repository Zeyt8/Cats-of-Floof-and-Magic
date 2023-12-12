public class WildFactionStatusEffect : StatusEffect
{
    public override string Name => "Wild Faction";
    public override string Description => "Gain access to the Camouflage ability." + (level >= 2 ? "\nMove faster in grasslands." : "");

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
