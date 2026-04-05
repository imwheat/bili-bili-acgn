//****************** 代码文件申明 ***********************
//* 文件：WildBullTrophy(野牛奖杯)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：同一回合内打出{YYSYCount:diff()}张带有一说一的牌，获得{Tang:diff()}点唐氏。
//*******************************************************
using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class WildBullTrophy : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("YYSYCount", 3m),
        new DynamicVar("Tang", 1m)
    ];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.YYSY), HoverTipFactory.FromPower<TangShiPower>()];
    private bool _isActivating;

	private int _yysyPlayedThisTurn;

	public override bool ShowCounter => CombatManager.Instance.IsInProgress;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return YysyPlayedThisTurn % base.DynamicVars["YYSYCount"].IntValue;
			}
			return base.DynamicVars["YYSYCount"].IntValue;
		}
	}


	private bool IsActivating
	{
		get
		{
			return _isActivating;
		}
		set
		{
			AssertMutable();
			_isActivating = value;
			UpdateDisplay();
		}
	}

	private int YysyPlayedThisTurn
	{
		get
		{
			return _yysyPlayedThisTurn;
		}
		set
		{
			AssertMutable();
			_yysyPlayedThisTurn = value;
			UpdateDisplay();
		}
	}

	private void UpdateDisplay()
	{
		if (IsActivating)
		{
			base.Status = RelicStatus.Normal;
		}
		else
		{
			int intValue = base.DynamicVars["YYSYCount"].IntValue;
			base.Status = ((YysyPlayedThisTurn % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
		}
		InvokeDisplayAmountChanged();
	}

	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return Task.CompletedTask;
		}
		YysyPlayedThisTurn = 0;
		base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner && CombatManager.Instance.IsInProgress && cardPlay.Card.Keywords.Contains(CustomKeyWords.YYSY))
		{
			YysyPlayedThisTurn++;
			int intValue = base.DynamicVars["YYSYCount"].IntValue;
			if (YysyPlayedThisTurn % intValue == 0)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
				await PowerCmd.Apply<TangShiPower>(base.Owner.Creature, base.DynamicVars["Tang"].BaseValue, base.Owner.Creature, null);
			}
		}
	}

	private async Task DoActivateVisuals()
	{
		IsActivating = true;
		Flash();
		await Cmd.Wait(1f);
		IsActivating = false;
	}

	public override Task AfterCombatEnd(CombatRoom _)
	{
		base.Status = RelicStatus.Normal;
		YysyPlayedThisTurn = 0;
		IsActivating = false;
		return Task.CompletedTask;
	}
}
