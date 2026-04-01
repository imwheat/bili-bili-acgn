//****************** 代码文件申明 ***********************
//* 文件：HairGrowth
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:08 星期二
//* 描述：抽取[gold]有一说一[/gold]张牌{CalculatedAmount}。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Creatures;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class HairGrowth : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.YYSY)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(0m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedCard").WithMultiplier((CardModel card, Creature? _)=>
		{
			return card.Owner.Creature?.GetPowerAmount<YYSYPower>() ?? 0;
		})
    ];

    public HairGrowth() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        #endregion
        await CardPileCmd.Draw(choiceContext, ((CalculatedVar)base.DynamicVars["CalculatedCard"]).Calculate(base.Owner.Creature), base.Owner);
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        // 无升级效果（动态变量 upgradeValue 均为 0，且未配置 upgradeEffects）

        #endregion
    }
}
