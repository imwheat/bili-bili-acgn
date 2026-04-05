//****************** 代码文件申明 ***********************
//* 文件：RocketHeavyRam(火箭冲击重压)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：对所有敌人造成{Damage:diff()}点伤害，抽取{Cards:diff()}张牌。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class RocketHeavyRam : CardBaseModel
{
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(24m, ValueProp.Move),
        new CardsVar(2)
    ];

    public RocketHeavyRam() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成伤害；抽取{Cards:diff()}张牌
        await DamageCmd.Attack(base.DynamicVars["Damage"].BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars["Cards"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(8m);
    }
}
