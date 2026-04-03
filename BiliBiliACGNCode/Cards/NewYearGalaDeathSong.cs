//****************** 代码文件申明 ***********************
//* 文件：NewYearGalaDeathSong(拜年祭死歌)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：对所有敌人造成{Damage:diff()}点伤害，给予{VulnerablePower:diff()}层易伤。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class NewYearGalaDeathSong : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<VulnerablePower>()];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 2;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(14m, ValueProp.Move),
        new DynamicVar("VulnerablePower", 1m)
    ];

    public NewYearGalaDeathSong() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 对所有敌人造成伤害，给予 VulnerablePower 层易伤
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .FromCard(this)
            .TargetingAllOpponents(base.CombatState)
            .Execute(choiceContext);
        // 对所有敌人给予 VulnerablePower 层易伤
        foreach(var enemy in base.CombatState.HittableEnemies){
            await PowerCmd.Apply<VulnerablePower>(enemy, base.DynamicVars["VulnerablePower"].BaseValue, base.Owner.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(5m);
        base.DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}
