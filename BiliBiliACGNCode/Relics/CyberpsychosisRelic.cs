//****************** 代码文件申明 ***********************
//* 文件：CyberpsychosisRelic(赛博精神病)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每场战斗开始时获得{Strength:diff()}点力量，回合结束时失去等量力量。
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

[Pool(typeof(EventRelicPool))]
public sealed class CyberpsychosisRelic : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Strength", 4m)
    ];

    // TODO: 与 OdmGear 对称的临时力量
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            CombatState combatState = player.Creature.CombatState;
            if(combatState.RoundNumber == 1)
            {
                Flash();
                await PowerCmd.Apply<FlexPotionPower>(base.Owner.Creature, base.DynamicVars["Strength"].BaseValue, base.Owner.Creature, null);
            }
        }
    }
}
