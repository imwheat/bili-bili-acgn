//****************** 代码文件申明 ***********************
//* 文件：IScream(我在呐喊)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：同一回合打出{DefCount:diff()}张技能牌后，对随机敌人造成{Damage:diff()}点伤害。
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

[Pool(typeof(SharedRelicPool))]
public sealed class IScream : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(3),
        new DamageVar(6m, ValueProp.Unpowered)
    ];

    private bool _isActivating;

	private int _skillCountThisTurn;

	public override bool ShowCounter => CombatManager.Instance.IsInProgress;

	public override int DisplayAmount
	{
		get
		{
			if (!IsActivating)
			{
				return SkillCountThisTurn % base.DynamicVars.Cards.IntValue;
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

	private int SkillCountThisTurn
	{
		get
		{
			return _skillCountThisTurn;
		}
		set
		{
			AssertMutable();
			_skillCountThisTurn = value;
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
			base.Status = ((SkillCountThisTurn % intValue == intValue - 1) ? RelicStatus.Active : RelicStatus.Normal);
		}
		InvokeDisplayAmountChanged();
	}

	public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
	{
		if (side != base.Owner.Creature.Side)
		{
			return Task.CompletedTask;
		}
		SkillCountThisTurn = 0;
		base.Status = RelicStatus.Normal;
		return Task.CompletedTask;
	}

	public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner == base.Owner && CombatManager.Instance.IsInProgress && cardPlay.Card.Type == CardType.Skill)
		{
			SkillCountThisTurn++;
			int intValue = base.DynamicVars.Cards.IntValue;
			if (SkillCountThisTurn % intValue == 0)
			{
				TaskHelper.RunSafely(DoActivateVisuals());
                var combatState = base.Owner.Creature.CombatState;
                if(combatState != null){
                    var randomEnemy = combatState.RunState.Rng.CombatTargets.NextItem(combatState.HittableEnemies);
                    if(randomEnemy != null){
                        await CreatureCmd.Damage(context, randomEnemy, base.DynamicVars.Damage, base.Owner.Creature);
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
		SkillCountThisTurn = 0;
		IsActivating = false;
		return Task.CompletedTask;
	}
}
