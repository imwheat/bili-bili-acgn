//****************** 代码文件申明 ***********************
//* 文件：NuclearRadiation
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：核辐射（诅咒牌，无法打出，如果留在手牌失去1点生命值，1层易伤）
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(CurseCardPool))]
public sealed class NuclearRadiation : CardBaseModel
{
    private const int energyCost = -1;
    private const CardType type = CardType.Curse;
    private const CardRarity rarity = CardRarity.Curse;
    private const TargetType targetType = TargetType.None;
    private const bool shouldShowInCardLibrary = true;
    public override int MaxUpgradeLevel => 0;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new HpLossVar(1m), new DynamicVar("Power", 1m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Unplayable];

    public NuclearRadiation() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        // 如果留在手牌失去1点生命值，1层易伤
        await CreatureCmd.Damage(choiceContext, base.Owner.Creature, base.DynamicVars.HpLoss.BaseValue, ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
        await PowerCmd.Apply<VulnerablePower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}

