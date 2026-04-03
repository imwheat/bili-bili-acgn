//****************** 代码文件申明 ***********************
//* 文件：QingYanBlueEyesWhiteDragon
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：青眼白龙（诅咒）1费，关键词【虚无、消耗
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

[Pool(typeof(CurseCardPool))]
public sealed class BlueEyesWhiteDragon : CardBaseModel
{
    private const int energyCost = 1;
    private const CardType type = CardType.Curse;
    private const CardRarity rarity = CardRarity.Curse;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    public override int MaxUpgradeLevel => 0;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Power", 2m), new DynamicVar("Gold", 870m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Eternal, CardKeyword.Exhaust, CardKeyword.Ethereal];

    public BlueEyesWhiteDragon() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await Task.CompletedTask;
    }
    public override async Task OnTurnEndInHand(PlayerChoiceContext choiceContext)
    {
        // 给予对面所有敌人2点力量Buff
        foreach(var enemy in base.CombatState.HittableEnemies)
        {
            await PowerCmd.Apply<StrengthPower>(enemy, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        }
    }
    public override async Task BeforeCardRemoved(CardModel card)
    {
        // 移除获得870块钱
        await PlayerCmd.GainGold(base.DynamicVars["Gold"].BaseValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
    }
}

