//****************** 代码文件申明 ***********************
//* 文件：BiggestWarCriminal(最大的战犯是你啊)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗。获得{Anger:diff()}点[gold]红温值[/gold]。升级后同时获得{Strength:diff()}点[gold]力量[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BiggestWarCriminal : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.Anger), HoverTipFactory.FromPower<StrengthPower>()];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Anger", 8m)
    ];

    public BiggestWarCriminal() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得 Anger 层红温；若 CardUpgradeLevel>0 则额外获得 2 点力量
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<AngerPower>(base.Owner.Creature, base.DynamicVars["Anger"].BaseValue, base.Owner.Creature, null);
        if(base.IsUpgraded){
            await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, 2m, base.Owner.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Anger"].UpgradeValueBy(2m);
    }
}
