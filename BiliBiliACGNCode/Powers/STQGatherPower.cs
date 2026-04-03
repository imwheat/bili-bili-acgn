//****************** 代码文件申明 ***********************
//* 文件：STQGatherPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 7tq集合
//*******************************************************
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class STQGatherPower : PowerBaseModel
{
    protected override string customIconPath => "stqgather";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPowerAmountChanged(PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        // 进入红怒时所有盟友获得 Amount 点力量
        if(applier == base.Owner && power is RagePower && amount > 0){
            foreach(var player in base.CombatState.Players){
                if(player.Creature == base.Owner || player.Creature.IsAlive == false)
                {
                    continue;
                }
                await PowerCmd.Apply<StrengthPower>(player.Creature, base.Amount, base.Owner, null);
            }
        }
    }

}
