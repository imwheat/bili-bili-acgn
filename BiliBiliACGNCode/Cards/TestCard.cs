//****************** 代码文件申明 ***********************
//* 文件：TestCard
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:00 星期四
//* 描述：测试卡牌示例，用于验证卡牌基础流程与动态变量
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

// 加入哪个卡池
[Pool(typeof(ColorlessCardPool))]
public sealed class TestCard : CardBaseModel
{
    // 基础耗能
    private const int energyCost = 1;
    // 卡牌类型
    private const CardType type = CardType.Attack;
    // 卡牌稀有度
    private const CardRarity rarity = CardRarity.Common;
    // 目标类型（AnyEnemy表示任意敌人）
    private const TargetType targetType = TargetType.AnyEnemy;
    // 是否在卡牌图鉴中显示
    private const bool shouldShowInCardLibrary = true;
    

    /// <summary>
    /// 卡牌基础动态变量：基础伤害。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(1, ValueProp.Move)];

    public TestCard() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary)
    {
    }

    /// <summary>
    /// 打出效果：对选定目标造成伤害，然后抽1张牌。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                .FromCard(this)
                .Targeting(cardPlay.Target)
                .Execute(choiceContext);
        }

        await CardPileCmd.Draw(choiceContext, 1, base.Owner);
    }

    /// <summary>
    /// 升级效果：提升基础伤害数值。
    /// </summary>
    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(1);
    }
}