//****************** 代码文件申明 ***********************
//* HaKaSeSneezingMachine
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：博士的喷嚏机 每场战斗开始时全体敌人获得3层虚弱，你有20%几率获得1层脆弱
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class HaKaSeSneezingMachine : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    // 用于UI文案占位符，不在此处实现具体战斗逻辑
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("WeakAmount", 3m),
        new DynamicVar("FragileChance", 20m),
        new DynamicVar("FragileAmount", 1m),
    ];
    // 虚弱和脆弱Hover
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<WeakPower>(), HoverTipFactory.FromPower<FrailPower>()];
    
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == base.Owner)
		{
			CombatState combatState = player.Creature.CombatState;
			if (combatState.RoundNumber == 1)
			{
                Flash();
                // 给所有敌人添加3层虚弱
                foreach(var enemy in combatState.HittableEnemies)
                {
                    await PowerCmd.Remove<ArtifactPower>(enemy);
                    await PowerCmd.Apply<WeakPower>(enemy, (int)base.DynamicVars["WeakAmount"].BaseValue, enemy, null);
                }
                // 有20%几率获得1层脆弱
                if(combatState.RunState.Rng.CombatPotionGeneration.NextInt(0, 100) < (int)base.DynamicVars["FragileChance"].BaseValue)
                {
                    await PowerCmd.Apply<FrailPower>(base.Owner.Creature, (int)base.DynamicVars["FragileAmount"].BaseValue, base.Owner.Creature, null);
                }

			}
		}
	}
}

