public class BoxArmorCat : Cat
{
    public override void OnEncounterStart()
    {
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new BoxArmourPassive(-1), BattleManager.GetBattleMapIndex(battleMap), Location.coordinates, owner);
        base.OnEncounterStart();
    }
}
