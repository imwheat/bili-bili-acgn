//****************** 代码文件申明 ***********************
//* 文件：EnjoyPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 开始享受
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class EnjoyPower : PowerBaseModel
{
    protected override string customIconPath => "enjoy";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 进入红怒后自动打出 Amount 张手牌中带有一说一的牌
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(power is RagePower && amount > 0 && applier == base.Owner){
            // 获取手牌中带有一说一的牌
            var cards = PileType.Hand.GetPile(base.Owner.Player).Cards.Where(c => c.Keywords.Contains(CustomKeyWords.YYSY)).ToList();
            // 实际执行操作次数为带有一说一的手牌数量和能力次数中的较小值
            int n = Mathf.Min(cards.Count, base.Amount);
            var randomCards = cards.UnstableShuffle(base.Owner.Player.RunState.Rng.CombatCardSelection).Take(n);
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
                foreach(var card in randomCards){
                    await CardCmd.AutoPlay(choiceContext, card, null);
                }
            }

        }
    }
}
