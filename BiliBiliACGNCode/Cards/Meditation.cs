//****************** 代码文件申明 ***********************
//* 文件：Meditation(我在冥想)
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：给予所有敌人{Tang:diff()}层[gold]变唐[/gold]。\n获得{Power:diff()}点[gold]红温[/gold]。
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(BottleCardPool))]
public sealed class Meditation : CardBaseModel
{
    #region 卡牌关键词与悬停
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<AngerPower>(),
        HoverTipFactory.FromPower<GetTangPower>(),
    ];
    #endregion
    #region 卡牌属性配置
    private const int energyCost = 1;
    private const CardType type = CardType.Skill;
    private const CardRarity rarity = CardRarity.Ancient;
    private const TargetType targetType = TargetType.AllEnemies;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Power", 4m),
        new DynamicVar("Tang", 4m),
    ];

    public Meditation() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    #endregion

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 播放动画
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        // 获得[gold]红温[/gold]
        await PowerCmd.Apply<AngerPower>(base.Owner.Creature, base.DynamicVars["Power"].BaseValue, base.Owner.Creature, this);
        foreach(var enemy in base.CombatState.HittableEnemies){
            await PowerCmd.Apply<GetTangPower>(enemy, base.DynamicVars["Tang"].BaseValue, base.Owner.Creature, null);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Power"].UpgradeValueBy(2m);
        base.DynamicVars["Tang"].UpgradeValueBy(4m);
    }
}
