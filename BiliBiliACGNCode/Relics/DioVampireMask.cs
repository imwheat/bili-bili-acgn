//****************** 代码文件申明 ***********************
//* 文件：DioVampireMask
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：Dio的吸血鬼面具 移除所有基础打击，获得嗜血啃咬*5
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class DioVampireMask : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<BloodyBite>()];
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 5m)];
    public override async Task AfterObtained()
    {
        // 移除所有基础打击
        var cardsToRemove = base.Owner.Deck.Cards.Where(card => card.Type == CardType.Attack && card.Tags.Contains(CardTag.Strike) && card.Rarity == CardRarity.Basic).ToList();
        await CardPileCmd.RemoveFromDeck(cardsToRemove);
        // 获得嗜血啃咬*5
		CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){
            await CardPileCmd.Add(base.Owner.RunState.CreateCard<BloodyBite>(base.Owner), PileType.Deck), 
            await CardPileCmd.Add(base.Owner.RunState.CreateCard<BloodyBite>(base.Owner), PileType.Deck),
            await CardPileCmd.Add(base.Owner.RunState.CreateCard<BloodyBite>(base.Owner), PileType.Deck),
            await CardPileCmd.Add(base.Owner.RunState.CreateCard<BloodyBite>(base.Owner), PileType.Deck),
            await CardPileCmd.Add(base.Owner.RunState.CreateCard<BloodyBite>(base.Owner), PileType.Deck)}, 
            2f);
    }
}
