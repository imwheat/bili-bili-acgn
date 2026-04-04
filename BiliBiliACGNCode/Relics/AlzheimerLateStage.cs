//****************** 代码文件申明 ***********************
//* 文件：AlzheimerLateStage
//* 作者：wheat
//* 创建时间：2026/04/04 10:25:00 星期六
//* 描述：阿尔茨海默症 晚期
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class AlzheimerLateStage : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner)
		{
			return false;
		}
        if(options.Any(option => option is AlzheimerRestSiteOption))
        {
            return false;
        }
		options.Add(new AlzheimerRestSiteOption(player));
		return true;
	}
	public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		// 每场战斗开始时获得2点红温，每回合获得1点红温
		if (player == base.Owner)
		{
			Flash();
			CombatState combatState = player.Creature.CombatState;
			if (combatState.RoundNumber == 1)
			{
				await PowerCmd.Apply<AngerPower>(base.Owner.Creature, 2, base.Owner.Creature, null);
			}
            await PowerCmd.Apply<AngerPower>(base.Owner.Creature, 1, base.Owner.Creature, null);
		}
	}
}