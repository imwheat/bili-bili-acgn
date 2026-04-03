//****************** 代码文件申明 ***********************
//* 文件：KaiYun(开云)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：本回合你每打出1张牌，本回合获得{Tang:diff()}层[gold]唐氏[/gold]。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;
using BiliBiliACGN.BiliBiliACGNCode.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class KaiYun : CardBaseModel
{

    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("KaiYun", 1m)
    ];

    public KaiYun() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 获得开云BUFF
        await PowerCmd.Apply<KaiYunPower>(base.Owner.Creature, base.DynamicVars["KaiYun"].BaseValue, base.Owner.Creature, null);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        AddKeyword(CardKeyword.Retain);
    }
}
