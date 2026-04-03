//****************** 代码文件申明 ***********************
//* 文件：HonestPickUp(这是俺拾嘞)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗。获得{Gold:diff()}点金币，回复{Energy:diff()}点能量。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class HonestPickUp : CardBaseModel
{
    #region 卡牌关键词与悬停
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Gold", 3m),
        new DynamicVar("Energy", 1m)
    ];

    public HonestPickUp() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 金币 + 能量
        await PlayerCmd.GainGold(base.DynamicVars["Gold"].BaseValue, base.Owner);
        await PlayerCmd.GainEnergy(base.DynamicVars["Energy"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Gold"].UpgradeValueBy(2m);
    }
}
