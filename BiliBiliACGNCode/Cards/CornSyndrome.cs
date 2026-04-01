//****************** 代码文件申明 ***********************
//* 文件：CornSyndrome
//* 作者：wheat
//* 创建时间：2026/03/31 08:55:49 星期二
//* 描述：获得{Block:diff()}点格挡。随机/选择给你手牌中的一张牌添加[gold]有一说一[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class CornSyndrome : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromKeyword(CustomKeyWords.YYSY)];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(7m, ValueProp.Move)
    ];

    public CornSyndrome() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block.BaseValue, ValueProp.Move, null);
        #endregion

        // 获取所有手牌
        var pile = PileType.Hand.GetPile(base.Owner);
        if (pile != null && pile.Cards.Count() > 0){   
            // 如果升级了，那就选择一张没有有一说一的手牌   
            if(base.IsUpgraded){
            var card = (await CardSelectCmd.FromHand(choiceContext, base.Owner, MCardSelectorPrefs.AddYYSY, MCardSelectorPrefs.NoYYSYFilter, this)).FirstOrDefault();
                if(card != null)
                    card.AddKeyword(CustomKeyWords.YYSY);
            }else{
                // 随机给你手牌中的一张牌添加[gold]有一说一[/gold]
                var randomCard = base.Owner.RunState.Rng.CombatCardSelection.NextItem(pile.Cards.Where(MCardSelectorPrefs.NoYYSYFilter));
                if(randomCard != null)
                    randomCard.AddKeyword(CustomKeyWords.YYSY);
            }
        }   
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Block"].UpgradeValueBy(3m);

        #endregion
    }
}
