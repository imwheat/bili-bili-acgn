//****************** 代码文件申明 ***********************
//* 文件：ShowbizEffect(节目效果)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗一张手牌。获得{Anger:diff()}点[gold]红温值[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class ShowbizEffect : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.Anger)];
    #endregion

    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Anger", 1m)
    ];

    public ShowbizEffect() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 消耗1张手牌。获得{Anger:diff()}点[gold]红温值[/gold]。
        // 选择1张手牌并消耗
        CardModel cardModel = (await CardSelectCmd.FromHand(prefs: new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1), context: choiceContext, player: base.Owner, filter: null, source: this)).FirstOrDefault();
        if (cardModel != null)
        {
            await CardCmd.Exhaust(choiceContext, cardModel);
        }
        // 获得{Anger:diff()}点[gold]红温[/gold]。
        await PowerCmd.Apply<AngerPower>(base.Owner.Creature, base.DynamicVars["Anger"].BaseValue, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Anger"].UpgradeValueBy(1m);
    }
}
