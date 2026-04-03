//****************** 代码文件申明 ***********************
//* 文件：OxCall(牛叫)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：消耗。对所有敌人在本回合施加{Tang:diff()}点变唐。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class OxCall : CardBaseModel
{
    #region 卡牌关键词与悬停
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<GetTangPower>()];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Tang", 7m)
    ];

    public OxCall() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人施加本回合变唐效果
        if(base.CombatState != null){
        foreach(var enemy in base.CombatState.HittableEnemies){
                await PowerCmd.Apply<GetTangPower>(enemy, base.DynamicVars["Tang"].BaseValue, base.Owner.Creature, null);
            }
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Tang"].UpgradeValueBy(2m);
    }
}
