//****************** 代码文件申明 ***********************
//* 文件：AngelCowPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 牛天使
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;

//****************** 代码文件申明 ***********************
//* 文件：AngelCowPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 牛天使
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AngelCowPower : PowerBaseModel
{
    protected override string customIconPath => "angelcow";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 打出带有一说一的牌时抽 Amount 张牌
    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        if(cardPlay.Card.Keywords.Contains(CustomKeyWords.YYSY) && cardPlay.Card.Owner == base.Owner.Player){
            await CardPileCmd.Draw(context, base.Amount, base.Owner.Player);
        }
    }

}
