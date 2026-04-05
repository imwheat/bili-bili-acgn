//****************** 代码文件申明 ***********************
//* 文件：OdmGear(立体机动装置)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每场战斗开始时获得{Dexterity:diff()}点敏捷，回合结束时失去等量敏捷。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class OdmGear : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Dexterity", 4m)
    ];

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            CombatState combatState = player.Creature.CombatState;
            if(combatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<SpeedPotionPower>(base.Owner.Creature, base.DynamicVars["Dexterity"].BaseValue, base.Owner.Creature, null);
            }
        }
    }
}
