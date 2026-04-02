//****************** 代码文件申明 ***********************
//* 文件：UltimateShitAnimeCommittee
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：究极粪动画制作委员会 每回合，随机复制一张手牌（0费，消耗）
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class UltimateShitAnimeCommittee : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;

    /// <summary>
    /// 每回合开始时，随机复制一张手牌（0费，消耗）
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override async Task AfterPlayerTurnStartLate(PlayerChoiceContext choiceContext, Player player)
    {
        // 获取玩家手牌
        var pile = PileType.Hand.GetPile(player);
        if(pile != null && pile.Cards.Count() > 0){
            // 随机复制一张手牌
            var randomCard = player.RunState.Rng.CombatCardSelection.NextItem(pile.Cards);
            if(randomCard != null){
                Flash();
                // 复制手牌，如果没有消耗，那就添加消耗
                CardModel card2 = randomCard.CreateClone();
                if(!card2.Keywords.Contains(CardKeyword.Exhaust)){
                    card2.AddKeyword(CardKeyword.Exhaust);
                }
                card2.EnergyCost.SetCustomBaseCost(0);
			    await CardPileCmd.AddGeneratedCardToCombat(card2, PileType.Hand, true);
            }
        }
    }
}
