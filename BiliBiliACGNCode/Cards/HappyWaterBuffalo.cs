//****************** 代码文件申明 ***********************
//* 文件：HappyWaterBuffalo(欢乐水牛)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗{AngerCost:diff()}点[gold]红温值[/gold]，获得[gold]受到的伤害减半[/gold]（本回合或能力，规则待定）。
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
public sealed class HappyWaterBuffalo : CardBaseModel
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
        new DynamicVar("AngerCost", 4m)
    ];

    public HappyWaterBuffalo() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 消耗 AngerCost 层红温；施加伤害减半（可用临时 Power / PlayerBuff）
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["AngerCost"].UpgradeValueBy(-1m);
    }
}
