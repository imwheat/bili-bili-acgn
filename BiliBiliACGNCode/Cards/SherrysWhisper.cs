//****************** 代码文件申明 ***********************
//* 文件：SherrysWhisper(雪莉的轻语)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：每消耗{AngerPerEnergy:diff()}点[gold]红温值[/gold]获得{Energy:diff()}点能量，至多{MaxEnergy:diff()}点。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class SherrysWhisper : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.Anger)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("AngerPerEnergy", 3m),
        new DynamicVar("Energy", 1m),
        new DynamicVar("MaxEnergy", 3m)
    ];

    public SherrysWhisper() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 按本回合/即时消耗的红温值换算能量（AngerPerEnergy、上限 MaxEnergy）
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["AngerPerEnergy"].UpgradeValueBy(-1m);
    }
}
