//****************** 代码文件申明 ***********************
//* 文件：Creep
//* 作者：wheat
//* 创建时间：2026/03/31 08:56:24 星期二
//* 描述：在本回合获得{Power:diff()}点[gold]有一说一[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class Creep : CardBaseModel
{
    #region 卡牌属性配置
    private const int energyCost = 0;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Common;
    private const TargetType targetType = TargetType.Self;
    private const bool shouldShowInCardLibrary = true;

    /// <summary>
    /// 牌面动态变量配置。
    /// </summary>
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 2m)
    ];

    public Creep() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    /// <summary>
    /// 出牌效果。
    /// </summary>
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        #region 卡牌打出效果
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<YYSYPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<YYSYLossPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        #endregion
    }

    /// <summary>
    /// 升级效果。
    /// </summary>
    protected override void OnUpgrade()
    {
        #region 升级效果
        base.DynamicVars["Power"].UpgradeValueBy(1m);

        #endregion
    }
}
