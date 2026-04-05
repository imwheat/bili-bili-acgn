//****************** 代码文件申明 ***********************
//* 文件：DeathNoteD(死亡笔记D)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：倒计时与胜利、移除 AI、脆弱；向弃牌堆加入死亡笔记E。消耗。
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
public sealed class DeathNoteD : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<FrailPower>(), HoverTipFactory.FromPower<ArtifactPower>(),HoverTipFactory.FromCard<DeathNoteE>()];

    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Fragile", 99m),
    ];

    public DeathNoteD() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 移除 AI；施加脆弱；弃牌堆加入 DeathNoteE
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await PowerCmd.Remove<ArtifactPower>(cardPlay.Target);
        // 施加脆弱
        await PowerCmd.Apply<FrailPower>(cardPlay.Target, base.DynamicVars["Fragile"].BaseValue, base.Owner.Creature, this);
        // 弃牌堆加入 DeathNoteE
        CardModel card = base.CombatState.CreateCard<DeathNoteE>(base.Owner);
		CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Discard, addedByPlayer: true));
		await Cmd.Wait(0.5f);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
    }
}
