//****************** 代码文件申明 ***********************
//* 文件：Day0
//* 作者：wheat
//* 创建时间：2026/03/31 10:18:00 星期二
//* 描述：获得{Block:diff()}点格挡。 抽取{Cards:diff()}张牌。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class Day0 : CardBaseModel
{
    #region 卡牌关键词与悬停
    // 未配置 CanonicalKeywords / ExtraHoverTips
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8m, ValueProp.Move),
        new CardsVar(2)
    ];

    public Day0() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, ValueProp.Move, null);

        await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
        #endregion
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Block"].UpgradeValueBy(3m);
        base.DynamicVars["Cards"].UpgradeValueBy(1m);

        #endregion
    }
}
