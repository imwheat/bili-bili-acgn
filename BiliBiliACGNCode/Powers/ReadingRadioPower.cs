//****************** 代码文件申明 ***********************
//* 文件：ReadingRadioPower(读书电台)
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 读书电台 每当你抽到一张带[gold]有一说一[/gold]的牌，随机对一名敌人造成{Damage:diff()}点伤害。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ReadingRadioPower : PowerBaseModel
{
    protected override string customIconPath => "readingradio";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if(card.Keywords.Contains(CustomKeyWords.YYSY)){
            await DamageCmd.Attack(base.Amount)
                .FromCard(null)
                .TargetingRandomOpponents(base.Owner.CombatState)
                .Execute(choiceContext);
        }
    }
}
