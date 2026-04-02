//****************** 代码文件申明 ***********************
//* 文件：EiHeiMask
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：欸嘿酱的面具 对精英造成伤害+20%
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class EiHeiMask : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 20m)];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        // 进入精英房间时，激活遗物
        if(room.RoomType == RoomType.Elite)
        {
            base.Status = RelicStatus.Active;
            Flash();
        }else{
            base.Status = RelicStatus.Normal;
        }
    }
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource?.Owner == base.Owner && base.Status == RelicStatus.Active)
        {
            return base.DynamicVars["Amount"].BaseValue / 100m * amount;
        }

        return 0m;
    }
}
