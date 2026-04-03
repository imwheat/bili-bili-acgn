//****************** 代码文件申明 ***********************
//* 文件：RagePower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 红怒
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Extensions;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class RagePower : PowerBaseModel
{
    protected override string customIconPath => "rage";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Single;
    public override bool IsInstanced => true;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("DamageMultiplier", 50m), new EnergyVar(3), new CardsVar(2)];

    public override async Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        // 回复能量，抽牌
        if(base.Owner.Player != null && base.Owner == applier){
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.BaseValue, base.Owner.Player);
            // 抽牌
            PlayerChoiceContext? choiceContext = null;
            GameAction? running = RunManager.Instance.ActionExecutor.CurrentlyRunningAction;
            if (running is PlayCardAction playCard && playCard.PlayerChoiceContext != null)
            {
                choiceContext = playCard.PlayerChoiceContext;
            }
            else if (running is GenericHookGameAction hookAction && hookAction.ChoiceContext != null)
            {
                // 若叠层来自「带 HookPlayerChoiceContext 的 hook 动作」（不是普通出牌）
                choiceContext = hookAction.ChoiceContext;
            }
            if(choiceContext != null){
                await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner.Player);
            }
        }
    }
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
		if (dealer != base.Owner)
		{
			return 1m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 1m;
		}
		if (target == null)
		{
			return 1m;
		}

        return 1m + base.DynamicVars["DamageMultiplier"].BaseValue/100m;
    }

}