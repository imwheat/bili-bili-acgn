//****************** 代码文件申明 ***********************
//* 文件：AkuasBlessing
//* 作者：wheat
//* 创建时间：2026/04/02 08:13:29 星期四
//* 描述：获得{StrengthPower:diff()}点力量和{DexterityPower:diff()}点敏捷。但有30%几率你获得1层脆弱。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class AkuasBlessing : CardBaseModel
{
    #region 卡牌关键词与悬停
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 2m),
        new DynamicVar("Probility", 30m),
    ];

    public AkuasBlessing() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        #endregion
        // 有30/20%几率你获得1层脆弱和易伤
        if (base.CombatState.RunState.Rng.CombatPotionGeneration.NextInt(0, 100) <= base.DynamicVars["Probility"].BaseValue)
        {
            await PowerCmd.Apply<FrailPower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(base.Owner.Creature, 1m, base.Owner.Creature, this);
        }
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Power"].UpgradeValueBy(1m);
        base.DynamicVars["Probility"].UpgradeValueBy(-10m);
        #endregion
    }
}
