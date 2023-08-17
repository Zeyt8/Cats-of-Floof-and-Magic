public class MainBaseUI : BuildingUI
{
    HexCoordinates location;

    public override void Initialize(Building building)
    {
        base.Initialize(building);
        location = building.Location.coordinates;
    }

    public void RecruitLeaderSelectTile()
    {
        Player.Instance.InitiateSelectCellForEffect(
            (cell) => (cell.coordinates.DistanceTo(location) <= 1),
            ((MainCastle)currentBuilding).CreateLeader);
        Destroy(gameObject);
    }
}
