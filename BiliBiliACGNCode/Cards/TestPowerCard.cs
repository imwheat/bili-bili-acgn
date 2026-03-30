//****************** 代码文件申明 ***********************
//* 文件：TestPowerCard
//* 作者：wheat
//* 创建时间：2026/03/26 13:39:00 星期四
//* 描述：测试能力卡牌
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class TestPowerCard : CardBaseModel
{
    // 基础耗能
    private const int energyCost = 0;
    // 卡牌类型
    private const CardType type = CardType.Power;
    
    /// <summary>
    /// 卡牌基础动态变量：施加能力的层数。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Power", 3m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];


    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型
    private const TargetType targetType = TargetType.Self;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;

    public TestPowerCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }
    /// <summary>
    /// 打出效果：触发施法动作并为自己施加 RagePower。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<YYSYPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
    }

    /// <summary>
    /// 升级效果：提升施加能力层数。
    /// </summary>
    protected override void OnUpgrade()
    {
        base.DynamicVars["Power"].UpgradeValueBy(1m);
    }
}