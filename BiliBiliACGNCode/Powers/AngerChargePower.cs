//****************** 代码文件申明 ***********************
//* 文件：AngerChargePower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 红温充能
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers; 

public sealed class AngerChargePower : PowerBaseModel
{
    /// <summary>
    /// 最大充能值
    /// </summary>
    private const int MAXCHARGE = 10;
    protected override string customIconPath => "anger_charge";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 如果充能值大于最大充能值，则移除
        if(base.Amount >= MAXCHARGE && power is AngerChargePower){
            await PowerCmd.Apply<RagePower>(base.Owner, 1, base.Owner, cardSource);
            await PowerCmd.Apply<AngerChargePower>(base.Owner, -MAXCHARGE, base.Owner, cardSource);
        }
    }

}