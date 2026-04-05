//****************** 代码文件申明 ***********************
//* 文件：ShortRecorder(短笛)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每场战斗中第一次打出技能牌时，获得{Block:diff()}点格挡。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class ShortRecorder : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(6m, ValueProp.Unpowered)
    ];
    private bool _wasUsedThisCombat;
	private bool WasUsedThisCombat
	{
		get
		{
			return _wasUsedThisCombat;
		}
		set
		{
			AssertMutable();
			_wasUsedThisCombat = value;
            base.Status = _wasUsedThisCombat ? RelicStatus.Disabled : RelicStatus.Normal;
		}
	}

    /// <summary>
    /// 首张 Skill 时获得 {Block:diff()} 点格挡
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="cardPlay"></param>
    /// <returns></returns>
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Type == CardType.Skill && cardPlay.Card.Owner == base.Owner && !WasUsedThisCombat)
        {
            Flash();
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["Block"].BaseValue, base.DynamicVars.Block.Props, cardPlay);
            WasUsedThisCombat = true;
        }
    }
    public override Task AfterCombatEnd(CombatRoom room)
    {
        WasUsedThisCombat = false;
        return Task.CompletedTask;
    }
}
