//****************** 代码文件申明 ***********************
//* 文件：DeathNoteA(死亡笔记A)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：死亡笔记链 A：易伤、弃牌堆加入 T。消耗。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class DeathNoteA : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>(), HoverTipFactory.FromPower<ArtifactPower>(),HoverTipFactory.FromCard<DeathNoteT>()];

    private const int energyCost = 3;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Vulnerable", 99m),
    ];

    public DeathNoteA() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 施加易伤；弃牌堆加入 DeathNoteT
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        // 移除 AI
        await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        // 施加易伤
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, base.DynamicVars["Vulnerable"].BaseValue, base.Owner.Creature, this);
        // 弃牌堆加入 DeathNoteT
        CardModel card = base.CombatState.CreateCard<DeathNoteT>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, addedByPlayer: true));
		await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
