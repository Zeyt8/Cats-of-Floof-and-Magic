public class LevitatingBlobCat : Cat
{
    public override bool IsValidDestination(HexCell cell)
    {
        return cell.units.Count == 0;
    }

    protected override bool IsValidCrossing(HexCell fromCell, HexCell toCell)
    {
        return true;
    }
}
