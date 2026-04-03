//****************** 代码文件申明 ***********************
//* 文件：CorrectTeamPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 正确车队
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

//****************** 代码文件申明 ***********************
//* 文件：CorrectTeamPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 正确车队
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class CorrectTeamPower : PowerBaseModel
{
    protected override string customIconPath => "correctteam";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 红怒状态下额外抽 Amount 张牌
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power is RagePower && amount > 0 && applier == base.Owner){
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


}
