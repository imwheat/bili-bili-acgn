//****************** 代码文件申明 ***********************
//* 文件：MagicalPoweredArmor(魔导铠)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：第{Turn:diff()}回合开始时，获得{Strength:diff()}层力量与{Block:diff()}点格挡。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MagicalPoweredArmor : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Turn", 4m),
        new DynamicVar("Strength", 4m),
        new BlockVar(22m, ValueProp.Move)
    ];
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner){
            var combatState = player.Creature.CombatState;
            if(combatState.RoundNumber == base.DynamicVars["Turn"].BaseValue){
                await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["Strength"].BaseValue, base.Owner.Creature, null);
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["Block"].BaseValue, base.DynamicVars.Block.Props, null);
            }
        }
    }

}
