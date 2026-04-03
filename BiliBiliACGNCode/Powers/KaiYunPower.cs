//****************** 代码文件申明 ***********************
//* 文件：KaiYunPower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 开云 本回合每打出1张牌，本回合获得1点唐氏。
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class KaiYunPower : PowerBaseModel
{
    protected override string customIconPath => "kai_yun";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner == base.Owner.Player)
        {
            await PowerCmd.Apply<TangShiPower>(base.Owner, 1m, base.Owner, null);
            await PowerCmd.Apply<TangShiLossPower>(base.Owner, 1m, base.Owner, null);
        }
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == CombatSide.Enemy)
        {
            await PowerCmd.TickDownDuration(this);
        }
    }
}