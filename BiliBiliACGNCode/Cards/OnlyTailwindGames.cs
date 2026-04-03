//****************** 代码文件申明 ***********************
//* 文件：OnlyTailwindGames(只打顺风局)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：获得{SwallowPride:diff()}层忍气吞声；进入红怒时获得{RageEnergy:diff()}点能量；获得{Block:diff()}点格挡。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class OnlyTailwindGames : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<RagePower>(),
        HoverTipFactory.FromPower<SwallowPridePower>()
    ];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Powers", 2m),
        new EnergyVar(2),
        new BlockVar(4m, ValueProp.Move)
    ];

    public OnlyTailwindGames() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得 SwallowPride 层忍气吞声
        await PowerCmd.Apply<SwallowPridePower>(base.Owner.Creature, base.DynamicVars["Powers"].BaseValue, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
