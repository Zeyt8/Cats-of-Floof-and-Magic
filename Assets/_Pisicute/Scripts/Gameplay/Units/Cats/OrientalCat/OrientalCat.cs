public class OrientalCat : Cat
{
    public override void OnEncounterStart()
    {
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new OrientalPassive(-1), BattleManager.GetBattleMapIndex(battleMap), Location.coordinates, owner);
        base.OnEncounterStart();
    }
}
