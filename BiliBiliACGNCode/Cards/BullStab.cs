//****************** 代码文件申明 ***********************
//* 文件：BullStab
//* 作者：wheat
//* 创建时间：2026/03/31 08:54:00 星期二
//* 描述：造成{Damage:diff()}点伤害。自动打出手牌中{Cards:diff()}张带[gold]有一说一[/gold]的牌。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BullStab : CardBaseModel
{
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7m, ValueProp.Move),
    ];

    public BullStab() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .Targeting(cardPlay.Target)
            .Execute(choiceContext);
       
        #endregion
        // 获取所有手牌
        var pile = PileType.Hand.GetPile(base.Owner);
        if (pile != null && pile.Cards.Count() > 0){
            // 如果升级了，那就选择一张带[gold]有一说一[/gold]的牌
            if(base.IsUpgraded){
                var card = (await CardSelectCmd.FromHand(choiceContext, base.Owner, new CardSelectorPrefs(MCardSelectorPrefs.TO_YYSY, 1), MCardSelectorPrefs.YYSYFilter, this)).FirstOrDefault();
                if(card != null)
                await CardCmd.AutoPlay(choiceContext, card, null);
            }else{
                // 随机打出手牌中一张带[gold]有一说一[/gold]的牌
                var randomCard = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards.Where(MCardSelectorPrefs.YYSYFilter));
                if(randomCard != null)
                await CardCmd.AutoPlay(choiceContext, randomCard, null);
            }
            
        }
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Damage"].UpgradeValueBy(3m);

        #endregion
    }
}
