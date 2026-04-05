//****************** 代码文件申明 ***********************
//* 文件：FuwaFuwaTime(滑滑蛋)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：第{Turn:diff()}回合额外抽{Cards:diff()}张牌。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class FuwaFuwaTime : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Turn", 2m),
        new CardsVar(2)
    ];

    /// <summary>
    /// 第二回合开始时额外抽 Cards 张牌
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player == base.Owner)
        {
            CombatState combatState = player.Creature.CombatState;
            if(combatState.RoundNumber == base.DynamicVars["Turn"].BaseValue)
            {
                Flash();
                await CardPileCmd.Draw(choiceContext, base.DynamicVars["Cards"].BaseValue, player);
            }
        }
    }
}
