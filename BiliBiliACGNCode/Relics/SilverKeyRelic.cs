//****************** 代码文件申明 ***********************
//* 文件：SilverKeyRelic(银之匙)
//* 作者：wheat
//* 创建时间：2026/04/06
//* 描述：当你打出消耗牌后，复制一张进入弃牌堆。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class SilverKeyRelic : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Rare;

    public override async Task AfterCardPlayed(PlayerChoiceContext context, CardPlay cardPlay)
    {
        // 如果不是自己的牌，则不执行
        if(cardPlay.Card.Owner != base.Owner){
            return;
        }
        // 如果不是消耗牌，则不执行
        if(!cardPlay.Card.Keywords.Contains(CardKeyword.Exhaust)){
            return;
        }
        // 复制一张进入弃牌堆
        var copy = cardPlay.Card.CreateClone();
        await CardPileCmd.Add(copy, PileType.Discard);
    }
}
