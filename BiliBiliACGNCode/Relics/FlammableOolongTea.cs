//****************** 代码文件申明 ***********************
//* 文件：FlammableOolongTea(可燃乌龙茶)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每场战斗结束时，回复{Heal:diff()}点生命。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class FlammableOolongTea : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Heal", 3m)
    ];

    /// <summary>
    /// 每场战斗结束时，回复{Heal:diff()}点生命。
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    public override async Task AfterCombatEnd(CombatRoom room)
    {
        Flash();
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars["Heal"].BaseValue, true);
    }
}
