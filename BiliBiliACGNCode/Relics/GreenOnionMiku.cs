//****************** 代码文件申明 ***********************
//* 文件：GreenOnionMiku(葱)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每场战斗开始时，所有玩家获得{Amount:diff()}层力量与敏捷。
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
public sealed class GreenOnionMiku : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Amount", 1m)
    ];
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            CombatState combatState = player.Creature.CombatState;
            if(combatState.RoundNumber == 1){
                Flash();
                foreach(var p in combatState.Players)
                {
                    await PowerCmd.Apply<StrengthPower>(p.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, null);
                    await PowerCmd.Apply<DexterityPower>(p.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, null);
                }
            }
        }
    }

}
