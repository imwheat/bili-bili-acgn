//****************** 代码文件申明 ***********************
//* 文件：GuguGaga(咕咕嘎嘎)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：移除所有敌人的格挡与人工制品，给予{Vulnerable:diff()}层易伤。消耗。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(EventCardPool))]
public sealed class GuguGaga : CardBaseModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips => [
        HoverTipFactory.Static(StaticHoverTip.Block),
        HoverTipFactory.FromPower<ArtifactPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
    ];

    private const int energyCost = 2;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Event;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Vulnerable", 2m)
    ];

    public GuguGaga() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 全体敌人移除格挡；移除/禁用「人工智能」相关机制；施加 Vulnerable 层易伤
		await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
		VfxCmd.PlayOnCreatureCenter(base.Owner.Creature, "vfx/vfx_flying_slash");
		var amount = base.DynamicVars["Vulnerable"].BaseValue;
        foreach(var enemy in base.CombatState.HittableEnemies){
            await CreatureCmd.LoseBlock(enemy, enemy.Block);
            await PowerCmd.Remove<ArtifactPower>(enemy);
            await PowerCmd.Apply<VulnerablePower>(enemy, amount, base.Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Vulnerable"].UpgradeValueBy(3m);
        base.AddKeyword(CardKeyword.Retain);
    }
}
