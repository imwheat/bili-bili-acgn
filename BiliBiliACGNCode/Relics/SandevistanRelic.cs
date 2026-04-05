//****************** 代码文件申明 ***********************
//* 文件：SandevistanRelic(斯安威斯坦)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：同一回合打出{CardsPlayed:diff()}张牌后，对所有敌人造成{Damage:diff()}点伤害。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class SandevistanRelic : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(6),
        new DamageVar(14m, ValueProp.Unpowered)
    ];

    private bool _isActivating;

	private int _cardsPlayedThisTurn;

	public override bool ShowCounter => CombatManager.Instance.IsInProgress;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return CardsPlayedThisTurn % base.DynamicVars.Cards.IntValue;
			}
			return base.DynamicVars.Cards.IntValue;
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

	private int CardsPlayedThisTurn
	{
		get
		{
			return _cardsPlayedThisTurn;
		}
		set
		{
			AssertMutable();
			_cardsPlayedThisTurn = value;
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
			int intValue = base.DynamicVars.Cards.IntValue;
			base.Status = ((CardsPlayedThisTurn % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
		}
		InvokeDisplayAmountChanged();
	}

	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return Task.CompletedTask;
		}
		CardsPlayedThisTurn = 0;
		base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner && CombatManager.Instance.IsInProgress)
		{
			CardsPlayedThisTurn++;
			int intValue = base.DynamicVars.Cards.IntValue;
			if (CardsPlayedThisTurn % intValue == 0)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
                var combatState = base.Owner.Creature.CombatState;
                if(combatState != null){
                    foreach(var enemy in combatState.HittableEnemies)
                    {
                        await CreatureCmd.Damage(context, enemy, base.DynamicVars.Damage, base.Owner.Creature);
                    }
                }

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
		CardsPlayedThisTurn = 0;
		IsActivating = false;
		return Task.CompletedTask;
	}
}
