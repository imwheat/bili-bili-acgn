//****************** 代码文件申明 ***********************
//* 文件：AnimeSwordPower
//* 作者：wheat
//* 创建时间：2026/04/03 18:24:00 星期五
//* 描述：能力 动漫区的剑 每当你获得[gold]红温值[/gold]，对随机敌人造成{Damage:diff()}点伤害。
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AnimeSwordPower : PowerBaseModel
{
    protected override string customIconPath => "animesword";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power is AngerPower && amount > 0 && applier == base.Owner){
            var enemies = base.CombatState.HittableEnemies;
            var enemy = base.CombatState.RunState.Rng.CombatTargets.NextItem(enemies);
            if(enemy != null){
                PlayerChoiceContext? choiceContext = null;
                GameAction? running = RunManager.Instance.ActionExecutor.CurrentlyRunningAction;
                if (running is PlayCardAction playCard && playCard.PlayerChoiceContext != null)
                {
                    choiceContext = playCard.PlayerChoiceContext;
                }
                else if (running is GenericHookGameAction hookAction && hookAction.ChoiceContext != null)
                {
                    choiceContext = hookAction.ChoiceContext;
                }
                if(choiceContext != null){
                    await CreatureCmd.Damage(choiceContext, enemy, base.Amount, ValueProp.Unpowered, base.Owner, null);
                }
            }
        }
    }
}
