//****************** 代码文件申明 ***********************
//* 文件：BlueTamago(蓝色团子)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：在你的回合开始时，额外抽{Cards:diff()}张牌。固有。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class BlueTamago : CardBaseModel
{

    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Power;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar(1)
    ];

    public BlueTamago() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 添加蓝色团子BUFF
        await PowerCmd.Apply<BlueTamagoPower>(base.Owner.Creature, base.DynamicVars["Cards"].BaseValue, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Cards"].UpgradeValueBy(1m);
        base.AddKeyword(CardKeyword.Innate);
    }
}
