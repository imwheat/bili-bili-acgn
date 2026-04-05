//****************** 代码文件申明 ***********************
//* 文件：NotebookOfDeath(死亡笔记)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：每场战斗开始时在手牌加入一张死亡笔记D（DeathNoteD）。
//*******************************************************
using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class DeathNote : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public override async Task AfterPlayerTurnStartEarly(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            CombatState combatState = player.Creature.CombatState;
            if (combatState.RoundNumber == 1){
                Flash();
                await CardPileCmd.AddGeneratedCardToCombat(ModelDb.Card<DeathNoteD>(), PileType.Hand, true);
            }
        }
    }

}
