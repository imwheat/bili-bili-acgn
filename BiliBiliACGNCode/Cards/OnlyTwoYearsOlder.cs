//****************** 代码文件申明 ***********************
//* 文件：OnlyTwoYearsOlder(他才比我大两岁)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：给予{VulnerablePower:diff()}层[gold]易伤[/gold]，并抽取{Cards:diff()}张牌。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class OnlyTwoYearsOlder : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("VulnerablePower", 2m),
        new CardsVar(1)
    ];

    public OnlyTwoYearsOlder() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 对目标施加易伤；抽 Cards 张牌
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
    }
}
