//****************** 代码文件申明 ***********************
//* 文件：SaberScabbard(saber的剑鞘)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每回合第{AttackIndex:diff()}张攻击牌造成双倍伤害。
//*******************************************************
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SaberScabbard : RelicBaseModel
{
	private bool _isActivating;
	private int _attacksPlayedThisTurn;
    private CardModel? _attackToDouble;
	public override RelicRarity Rarity => RelicRarity.Rare;

	public override bool ShowCounter => CombatManager.Instance.IsInProgress;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return AttacksPlayedThisTurn % base.DynamicVars.Cards.IntValue;
			}
			return base.DynamicVars.Cards.IntValue;
		}
	}

	protected override IEnumerable<DynamicVar> CanonicalVars => [
		new CardsVar(5),
    ];

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
    private CardModel? AttackToDouble
	{
		get
		{
			return _attackToDouble;
		}
		set
		{
			AssertMutable();
			_attackToDouble = value;
		}
	}

	private int AttacksPlayedThisTurn
	{
		get
		{
			return _attacksPlayedThisTurn;
		}
		set
		{
			AssertMutable();
			_attacksPlayedThisTurn = value;
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
			base.Status = (AttacksPlayedThisTurn == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal;
		}
		InvokeDisplayAmountChanged();
	}
    /// <summary>
    /// 回合开始时，重置攻击次数
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="side"></param>
    /// <param name="combatState"></param>
    /// <returns></returns>
	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return Task.CompletedTask;
		}
		Reset();
		return Task.CompletedTask;
	}
    /// <summary>
    /// 通知攻击牌打出，增加攻击次数
    /// </summary>
    public void NotifyAttackPlayed()
	{
		AttacksPlayedThisTurn++;
		if (AttacksPlayedThisTurn == base.DynamicVars.Cards.IntValue)
		{
			TaskHelper.RunSafely(DoActivateVisuals());
		}
	}
    /// <summary>
    /// 攻击牌打出时，增加攻击次数
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cardPlay"></param>
    /// <returns></returns>
	public override Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner && CombatManager.Instance.IsInProgress && cardPlay.Card.Type == CardType.Attack)
		{
			NotifyAttackPlayed();
		}
        return Task.CompletedTask;
	}
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
	{
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if (cardSource == null)
		{
			return 1m;
		}
		if (dealer != base.Owner.Creature && dealer != base.Owner.Osty)
		{
			return 1m;
		}
		if (AttackToDouble == null)
		{
			CardPile? pile = cardSource.Pile;
			if ((pile == null || pile.Type != PileType.Play) && AttacksPlayedThisTurn == base.DynamicVars.Cards.IntValue - 1)
			{
				return 2m;
			}
			return 1m;
		}
		if (cardSource == AttackToDouble)
		{
			return 2m;
		}
		return 1m;
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
		Reset();
		return Task.CompletedTask;
	}
    private void Reset()
    {
        IsActivating = false;
        AttackToDouble = null;
        AttacksPlayedThisTurn = 0;
    }
}
