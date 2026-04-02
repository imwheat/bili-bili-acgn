//****************** 代码文件申明 ***********************
//* 文件：BloodyBite
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：嗜血啃咬
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class BloodyBite : CardBaseModel
{
    #region 卡牌关键词与悬停
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HealVar(2m),
        new DamageVar(6m, ValueProp.Move)
    ];

    public BloodyBite() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 造成伤害，然后回血
        #region 卡牌打出效果
        await DamageCmd.Attack(base.DynamicVars["Damage"].BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
        await CreatureCmd.Heal(base.Owner.Creature, base.DynamicVars["Heal"].BaseValue, true);
        #endregion

    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Heal"].UpgradeValueBy(1m);
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
        #endregion
    }
}
