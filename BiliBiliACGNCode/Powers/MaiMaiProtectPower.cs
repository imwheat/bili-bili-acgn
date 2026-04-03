//****************** 代码文件申明 ***********************
//* 文件：MaiMaiProtectPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 脉脉庇护
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

//****************** 代码文件申明 ***********************
//* 文件：MaiMaiProtectPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 脉脉庇护
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MaiMaiProtectPower : PowerBaseModel
{
    protected override string customIconPath => "maimaiprotect";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    // 每回合失去红温并获得 Amount 点能量
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner.Player){
            if(player.Creature.HasPower<AngerPower>())
            {
                await PowerCmd.Apply<AngerPower>(base.Owner, -1m, base.Owner, null);
            }
            await PlayerCmd.GainEnergy(base.Amount, player);
        }
    }

}
