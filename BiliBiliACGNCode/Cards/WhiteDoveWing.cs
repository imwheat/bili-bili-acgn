//****************** 代码文件申明 ***********************
//* 文件：WhiteDoveWing(白鸽之翼)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：获得{Block:diff()}点格挡。（\n[gold]升级[/gold]你[gold]手牌[/gold]中的{IfUpgraded:show:所有牌|一张牌}。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class WhiteDoveWing : CardBaseModel
{
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Uncommon;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(9m, ValueProp.Move)
    ];

    public WhiteDoveWing() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
		await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
		if (base.IsUpgraded)
		{
			foreach (CardModel item in PileType.Hand.GetPile(base.Owner).Cards.Where((CardModel c) => c.IsUpgradable))
			{
				CardCmd.Upgrade(item);
			}
			return;
		}
		CardModel cardModel = await CardSelectCmd.FromHandForUpgrade(choiceContext, base.Owner, this);
		if (cardModel != null)
		{
			CardCmd.Upgrade(cardModel);
		}
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Block"].UpgradeValueBy(3m);
    }
}
