//****************** 代码文件申明 ***********************
//* 文件：WindIsRatherNoisy(风儿甚是喧嚣)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：从你的消耗牌堆选择{Cards:diff()}张牌加入手牌。消耗。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class WindIsRatherNoisy : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public WindIsRatherNoisy() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var cards = (from c in PileType.Exhaust.GetPile(base.Owner).Cards
				orderby c.Rarity, c.Id
                select c).ToList();
        if(cards.Count > 0){
            // 多选 UI 从消耗堆取牌入手牌
            await CardPileCmd.Add(await CardSelectCmd.FromSimpleGrid(choiceContext, cards, base.Owner, new CardSelectorPrefs(base.SelectionScreenPrompt, 0, (int)base.DynamicVars.Cards.BaseValue)), PileType.Hand);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
