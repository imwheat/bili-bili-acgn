//****************** 代码文件申明 ***********************
//* 文件：GoddessCompanion
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：智障女神同行 开局随机对敌人造成0-20点伤害，你每走5个房间，她就会弄丢你0-10金币。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class GoddessCompanion : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 20m), new DynamicVar("Amount", 5m), new DynamicVar("Gold", 10m), new DamageVar("RandomDamage", 0m, ValueProp.Unpowered)];
    private int _roomEnter = 0;
    public override bool ShowCounter => true;
    public override int DisplayAmount => RoomEnter;
    [SavedProperty]
	public int RoomEnter
	{
		get
		{
			return _roomEnter;
		}
		private set
		{
			AssertMutable();
            _roomEnter = value;
            if(_roomEnter >= (int)base.DynamicVars["Amount"].BaseValue){
                // 随机弄丢钱
                RandomLostGold();
                _roomEnter = 0;
            }
			UpdateDisplay();
		}
	}
    private void UpdateDisplay()
	{
		InvokeDisplayAmountChanged();
	}
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == base.Owner)
		{
			CombatState combatState = player.Creature.CombatState;
			if (combatState.RoundNumber == 1)
			{
                // 随机对敌人造成0-20点伤害
                int dmg = (int)base.DynamicVars["Damage"].BaseValue;
                var damage = combatState.RunState.Rng.CombatPotionGeneration.NextInt(-dmg/2, dmg);
                // 如果伤害大于0，则造成伤害
                if(damage > 0){
                    Flash();
                    base.DynamicVars["RandomDamage"].BaseValue = damage;
				    VfxCmd.PlayOnCreatureCenters(combatState.HittableEnemies, "vfx/vfx_attack_slash");
				    await CreatureCmd.Damage(choiceContext, combatState.HittableEnemies, (DamageVar)base.DynamicVars["RandomDamage"], base.Owner.Creature);
                }

                // 失活
                base.Status = RelicStatus.Disabled;
			}
		}
	}
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        base.Status = RelicStatus.Normal;
    }
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        RoomEnter++;
    }
    private void RandomLostGold()
    {
        int gold = (int)base.DynamicVars["Gold"].BaseValue;
        var lostGold = base.Owner.RunState.Rng.CombatPotionGeneration.NextInt(-gold/2, gold);
        if(lostGold > 0){
            Flash();
            PlayerCmd.LoseGold(lostGold, base.Owner);
        }
    }
}
