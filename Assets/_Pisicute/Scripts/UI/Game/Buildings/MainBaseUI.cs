public class MainBaseUI : BuildingUI
{
    Building currentBuilding;
    HexCoordinates location;

    public override void Initialize(Building building)
    {
        currentBuilding = building;
        location = building.Location.coordinates;
    }

    public void RecruitLeaderSelectTile()
    {
        Player.Instance.InitiateSelectCellForEffect(
            (cell) => (cell.coordinates.DistanceTo(location) == 1),
            ((MainCastle)currentBuilding).CreateLeader);
        Destroy(gameObject);
    }
}
