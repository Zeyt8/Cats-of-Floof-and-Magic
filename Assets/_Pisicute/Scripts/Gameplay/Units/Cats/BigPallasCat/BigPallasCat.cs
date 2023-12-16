public class BigPallasCat : Cat
{
    public override void OnEncounterStart()
    {
        PlayerObject.Instance.AddStatusEffectToUnitServerRpc(new BigPallasPassive(-1), BattleManager.GetBattleMapIndex(battleMap), Location.coordinates, owner);
        base.OnEncounterStart();
    }
}
