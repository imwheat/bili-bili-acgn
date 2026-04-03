//****************** 代码文件申明 ***********************
//* 文件：CorrectTeam(正确车队)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：获得{MaxEnergy:diff()}点能量上限。[gold]红怒[/gold]状态下额外抽{DrawInRage:diff()}张牌。保留。
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
public sealed class CorrectTeam : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<BottleRagePower>()];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain];

    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("MaxEnergy", 1m),
        new DynamicVar("DrawInRage", 1m)
    ];

    public CorrectTeam() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 本战斗能量上限 +MaxEnergy；施加 CorrectTeamPower（DrawInRage 用于红怒额外抽牌）
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DrawInRage"].UpgradeValueBy(1m);
    }
}
