//****************** 代码文件申明 ***********************
//* 文件：WitPeak(智斗顶峰)
//* 作者：wheat
//* 创建时间：2026/03/31 10:25:03 星期二
//* 描述：消耗，获得{Energy:diff()}点能量，并抽取 2 张牌，同时为这些牌赋予[gold]有一说一[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class WitPeak : CardBaseModel
{
    #region 卡牌关键词与悬停
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Energy", 1m)
    ];

    public WitPeak() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await PlayerCmd.GainEnergy(base.DynamicVars["Energy"].BaseValue, base.Owner);
        await PowerCmd.Apply<AddYYSYTempPower>(base.Owner.Creature, 2, base.Owner.Creature, null);
        var drawCards = await CardPileCmd.Draw(choiceContext, 2m, base.Owner);
        foreach(var card in drawCards){
            card.AddKeyword(CustomKeyWords.YYSY);
        }
        #endregion
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Energy"].UpgradeValueBy(1m);

        #endregion
    }
}
