//****************** 代码文件申明 ***********************
//* 文件：AngerPower
//* 作者：wheat
//* 创建时间：2026/03/29 星期日
//* 描述：能力 红温
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Entities.Creatures;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class AngerPower : PowerBaseModel
{
    protected override string customIconPath => "anger";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果施加者是玩家，则施加红温充能
        if(applier == base.Owner && power is AngerPower){
            if(amount > 0){
                await PowerCmd.Apply<AngerChargePower>(base.Owner, amount, base.Owner, cardSource);
            }
        }
    }


}
