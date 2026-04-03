//****************** 代码文件申明 ***********************
//* 文件：AddYYSYTempPower(临时添加有一说一)
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 临时添加有一说一 每回合随机赋予一张手牌[gold]有一说一[/gold]。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AddYYSYTempPower : PowerBaseModel
{
    protected override string customIconPath => "addyysytemp";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawnEarly(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        // 不是自己抽的牌，不处理
        if(card.Owner.Creature != base.Owner){
            return;
        }
        if(card.Keywords.Contains(CustomKeyWords.YYSY)){
            return;
        }
        card.AddKeyword(CustomKeyWords.YYSY);
		await PowerCmd.Apply<AddYYSYTempPower>(base.Owner, -1, base.Owner, null);
    }
}