//****************** 代码文件申明 ***********************
//* 文件：SatokoCleaver(柴刀)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：已升级的攻击牌额外造成{Damage:diff()}点伤害。
//*******************************************************
using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SatokoCleaver : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6m, ValueProp.Unpowered)
    ];

    /// <summary>
    /// 已升级的攻击牌额外造成 {Damage:diff()} 点伤害
    /// </summary>
    /// <param name="target"></param>
    /// <param name="amount"></param>
    /// <param name="props"></param>
    /// <param name="dealer"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        // 只有攻击牌且已升级的牌才能触发
        if(dealer != base.Owner.Creature || cardSource == null || !cardSource.IsUpgraded || cardSource.Type != CardType.Attack || !props.IsPoweredAttack_()) return 0m;
        return base.DynamicVars.Damage.BaseValue;
    }

}
