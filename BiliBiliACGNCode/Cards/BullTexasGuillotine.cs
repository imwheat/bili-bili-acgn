//****************** 代码文件申明 ***********************
//* 文件：BullTexasGuillotine(牛克萨斯断头台)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：X费。造成等同于[gold]红温值[/gold]×本次支付能量倍数的伤害；[gold]红怒[/gold]时倍率翻倍。消耗，保留。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BottleRagePower = BiliBiliACGN.BiliBiliACGNCode.Powers.RagePower;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BullTexasGuillotine : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CustomKeyWords.Anger),
        HoverTipFactory.FromPower<BottleRagePower>()
    ];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];

    #region 卡牌属性配置
    // TODO: 将 energyCost 改为 STS2 中「X 费」规定的 canonical 数值（当前为占位，避免未知 API 下编译/运行异常）
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    public BullTexasGuillotine() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 读取本次打出消耗的 X（或 CardPlay 上的能量信息）；伤害 = 红温层数 × X ×（有 BottleRagePower 则 2 否则 1）
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
    }
}
