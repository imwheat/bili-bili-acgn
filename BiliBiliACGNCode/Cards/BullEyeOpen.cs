//****************** 代码文件申明 ***********************
//* 文件：BullEyeOpen
//* 作者：wheat
//* 创建时间：2026/03/31 08:51:59 星期二
//* 描述：造成等同于你本回合抽到过的[gold]有一说一[/gold]次数的{CalculatedDamage:diff()}伤害。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BullEyeOpen : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CustomKeyWords.YYSY];
    #region 卡牌属性配置
    private const int energyCost = 3;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(1m),
		new ExtraDamageVar(1m),
		new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card, Creature? _) => 
        CombatManager.Instance.History.Entries.OfType<CardDrawnEntry>().Count(
            (CardDrawnEntry e) => e.Actor == card.Owner.Creature && e.Card.CanonicalKeywords.Contains(CustomKeyWords.YYSY)))
    ];

    public BullEyeOpen() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        #endregion
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        // 降费
        base.EnergyCost.UpgradeBy(-1);

        #endregion
    }
}
