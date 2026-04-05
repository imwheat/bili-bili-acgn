//****************** 代码文件申明 ***********************
//* 文件：DeathNoteT(死亡笔记T)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：死亡笔记链 T：缩小、弃牌堆加入 H。消耗。
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
public sealed class DeathNoteT : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<ShrinkPower>(), HoverTipFactory.FromPower<ArtifactPower>(),HoverTipFactory.FromCard<DeathNoteH>()];

    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Shrink", 99m),
    ];

    public DeathNoteT() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 移除 AI；施加缩小；弃牌堆加入 DeathNoteH
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        // 施加缩小
        await PowerCmd.Apply<ShrinkPower>(cardPlay.Target, base.DynamicVars["Shrink"].BaseValue, base.Owner.Creature, this);
        // 弃牌堆加入 DeathNoteH
        CardModel card = base.CombatState.CreateCard<DeathNoteH>(base.Owner);
        await Task.CompletedTask;
        CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, addedByPlayer: true));
		await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
