//****************** 代码文件申明 ***********************
//* 文件：NoRightToKnightMe(无权为我授勋)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：进入[gold]红怒[/gold]。获得等同于当前[gold]红温[/gold]的[gold]力量[/gold]。下一回合[gold]死亡[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BottleRagePower = BiliBiliACGN.BiliBiliACGNCode.Powers.RagePower;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

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

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
    ];

    public NoRightToKnightMe() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 进入[gold]红怒[/gold]。获得等同于当前[gold]红温[/gold]的[gold]力量[/gold]。下一回合[gold]死亡[/gold]。
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<BottleRagePower>(base.Owner.Creature, base.Owner.Creature.GetPower<BottleRagePower>()?.Amount ?? 0m, base.Owner.Creature, null);
        // 添加NoRightToKnightMe BUFF
        await PowerCmd.Apply<NoRightToKnightMePower>(base.Owner.Creature, 1, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
