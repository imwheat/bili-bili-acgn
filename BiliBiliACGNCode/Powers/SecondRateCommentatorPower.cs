//****************** 代码文件申明 ***********************
//* 文件：SecondRateCommentatorPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 二路解说
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SecondRateCommentatorPower : PowerBaseModel
{
    protected override string customIconPath => "secondratecommentator";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //每回合获得 Amount 点红温
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner.Player)
        {
            await PowerCmd.Apply<AngerPower>(base.Owner, base.Amount, base.Owner, null);
        }
    }
}
