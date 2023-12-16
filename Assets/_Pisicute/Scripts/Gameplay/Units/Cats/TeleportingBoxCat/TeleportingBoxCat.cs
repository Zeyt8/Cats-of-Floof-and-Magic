public class TeleportingBoxCat : Cat
{
    public override void OnEncounterStart()
    {
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new TeleportingBoxPassive(-1), BattleManager.GetBattleMapIndex(battleMap), Location.coordinates, owner);
        base.OnEncounterStart();
    }
}
