//****************** 代码文件申明 ***********************
//* 文件：BlueTamagoPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 蓝色团子
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BlueTamagoPower : PowerBaseModel
{
    protected override string customIconPath => "bluetamago";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 回合开始时额外抽 Amount 张牌
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner.Player){
            await CardPileCmd.Draw(choiceContext, base.Amount, player);
        }
    }

}
