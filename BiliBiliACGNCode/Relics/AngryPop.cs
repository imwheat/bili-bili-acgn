//****************** 代码文件申明 ***********************
//* 文件：AngryPop
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：愤怒的pop子 每当你在战斗中失去生命，获得临时{Amount}力量
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class AngryPop : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 1m), new DynamicVar("StrengthApplied", 0m)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    public override int DisplayAmount => (int)base.DynamicVars["StrengthApplied"].BaseValue;
    public override bool ShowCounter => true;

    public override Task BeforeCombatStart()
    {
        base.DynamicVars["StrengthApplied"].BaseValue = 0m;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner.Creature && result.UnblockedDamage > 0)
		{
            Flash();
			await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, null);
			base.DynamicVars["StrengthApplied"].BaseValue += base.DynamicVars["Amount"].BaseValue;
            InvokeDisplayAmountChanged();
		}
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {

        if (side == base.Owner.Creature.Side)
		{
            Flash();
			await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, -base.DynamicVars["StrengthApplied"].BaseValue, base.Owner.Creature, null, silent: true);
            base.DynamicVars["StrengthApplied"].BaseValue = 0m;
            InvokeDisplayAmountChanged();
		}
    }

}
