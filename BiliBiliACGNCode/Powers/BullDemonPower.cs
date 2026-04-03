//****************** 代码文件申明 ***********************
//* 文件：BullDemonPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 牛魔形态
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BullDemonPower : PowerBaseModel
{
    protected override string customIconPath => "bulldemon";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每打出一张有一说一获得 Amount 点「唐氏」
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Keywords.Contains(CustomKeyWords.YYSY) && cardPlay.Card.Owner == base.Owner.Player){
            await PowerCmd.Apply<TangShiPower>(base.Owner, base.Amount, base.Owner, null);
        }
    }
}
