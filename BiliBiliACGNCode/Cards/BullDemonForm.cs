//****************** 代码文件申明 ***********************
//* 文件：BullDemonForm(牛魔形态)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：你每打出一张带[gold]有一说一[/gold]的牌，获得{Stacks:diff()}层[gold]唐氏[/gold]。有一说一，保留。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BullDemonForm : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.YYSY)];
    #endregion

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CustomKeyWords.YYSY, CardKeyword.Retain];

    #region 卡牌属性配置
    private const int energyCost = 3;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Stacks", 1m)
    ];

    public BullDemonForm() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // TODO: 施加 BullDemonPower（Amount=Stacks）；Power 内监听打出 YYSY 时叠唐氏（GetTangPower 等）
        await Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
