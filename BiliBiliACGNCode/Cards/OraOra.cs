//****************** 代码文件申明 ***********************
//* 文件：OraOra(欧拉欧拉)
//* 作者：wheat
//* 创建时间：2026/04/05
//* 描述：X费。造成{Damage:diff()}点伤害X次；若X≥6则把X翻3倍后再结算（见文案）。
//*******************************************************
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Cards;

[Pool(typeof(ColorlessCardPool))]
public sealed class OraOra : CardBaseModel
{
    private const int energyCost = -1;
    protected override bool HasEnergyCostX => true;
    private const CardType type = CardType.Attack;
    private const CardRarity rarity = CardRarity.Rare;
    private const TargetType targetType = TargetType.AnyEnemy;
    private const bool shouldShowInCardLibrary = true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10m, ValueProp.Move),
        new DynamicVar("XThreshold", 5m),
        new DynamicVar("XMultiplier", 2m)
    ];

    public OraOra() : base(energyCost, type, rarity, targetType, shouldShowInCardLibrary) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 读取本次 X；若 X≥阈值则有效次数 = X*XMultiplier；每次 HitDamage
        int num = ResolveEnergyXValue();
        if(base.IsUpgraded){
            ++num;
        }
        // 若 X≥阈值则有效次数 = X*XMultiplier
        if(num >= base.DynamicVars["XThreshold"].BaseValue){
            num = (int)(num * base.DynamicVars["XMultiplier"].BaseValue);
        }
        
        // 造成伤害 X 次
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            .WithHitCount(num).FromCard(this)
			.Targeting(cardPlay.Target)
			.Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
    }
}
