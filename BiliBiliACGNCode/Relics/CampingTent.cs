//****************** 代码文件申明 ***********************
//* 文件：CampingTent(露营帐篷)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：在精英战斗开始时，回复{Heal:diff()}点生命。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class CampingTent : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Heal", 8m)
    ];

    /// <summary>
    /// 进入精英战时，回复 {Heal:diff()} 点生命
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if(room.RoomType == RoomType.Elite){
            Flash();
            await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars["Heal"].BaseValue, true);
        }
    }

}
