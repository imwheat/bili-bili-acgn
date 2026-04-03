//****************** 代码文件申明 ***********************
//* 文件：NoRightToKnightMe(无权为我授勋)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗，保留。进入[gold]红怒[/gold]。获得等同于当前[gold]红温值[/gold]的[gold]力量[/gold]。下一回合[gold]死亡[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BottleRagePower = BiliBiliACGN.BiliBiliACGNCode.Powers.RagePower;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class NoRightToKnightMe : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CustomKeyWords.Anger),
        HoverTipFactory.FromPower<BottleRagePower>()
    ];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("DeathTurnDelay", 1m)
    ];

    public NoRightToKnightMe() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 触发进入红怒（RagePower/AngerCharge 满层逻辑）；按红温层数获得力量；施加下回合结束时即死的标记 Power 或 Buff
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
    }
}
