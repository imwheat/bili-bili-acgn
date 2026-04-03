//****************** 代码文件申明 ***********************
//* 文件：NeverDiePower(永远不死)
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 永远不死 每当你打出带[gold]有一说一[/gold]的牌时，获得Amount点格挡。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NeverDiePower : PowerBaseModel
{
    protected override string customIconPath => "neverdie";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if(cardPlay.Card.Owner == base.Owner.Player && cardPlay.Card.Keywords.Contains(CustomKeyWords.YYSY)){
            await CreatureCmd.GainBlock(base.Owner.Player.Creature, base.Amount, ValueProp.Move, null, true);
        }
    }
}
