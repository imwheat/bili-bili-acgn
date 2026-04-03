//****************** 代码文件申明 ***********************
//* 文件：SwallowPridePower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 忍气吞声
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SwallowPridePower : PowerBaseModel
{
    protected override string customIconPath => "swallowpride";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //进入红怒时获得能量与格挡
    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if(applier == base.Owner && power is RagePower && amount > 0){
            await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);
            await CreatureCmd.GainBlock(base.Owner, base.Amount * 2m, ValueProp.Unpowered, null);
        }
    }
}
