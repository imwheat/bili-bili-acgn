//****************** 代码文件申明 ***********************
//* 文件：FengYouJing
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：风油精 攻击牌+2伤害
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class FengYouJing : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 2m)];
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if(cardSource?.Owner == base.Owner && props.HasFlag(ValueProp.Move))
        {
            return base.DynamicVars["Amount"].BaseValue;
        }
        return 0m;
    }
}
