//****************** 代码文件申明 ***********************
//* 文件：CalmPipi
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：冷静的pipi美 每当你完美格挡攻击，下一回合临时获得+1敏捷，持续叠加
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class CalmPipi : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 1m), new DynamicVar("DexterityApplied", 0m)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<DexterityPower>()];
    public override int DisplayAmount => (int)base.DynamicVars["Amount"].BaseValue;
    public override bool ShowCounter => true;
    private void UpdateValue(decimal value)
	{
        base.DynamicVars["Amount"].BaseValue = value;
		InvokeDisplayAmountChanged();
	}
    /// <summary>
    /// 战斗开始前，重置敏捷叠加量
    /// </summary>
    /// <returns></returns>
    public override Task BeforeCombatStart()
    {
        UpdateValue(0m);
        base.DynamicVars["DexterityApplied"].BaseValue = 0m;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
    /// <summary>
    /// 玩家回合开始时，给予玩家敏捷
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
		if (player == base.Owner && base.DynamicVars["Amount"].BaseValue > 0m)
		{
            Flash();
            await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, null);
            base.DynamicVars["DexterityApplied"].BaseValue += base.DynamicVars["Amount"].BaseValue;
		}
	}
    /// <summary>
    /// 受到伤害时，判断是否完美格挡，如果是，增加敏捷叠加量
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="target"></param>
    /// <param name="result"></param>
    /// <param name="props"></param>
    /// <param name="dealer"></param>
    /// <param name="cardSource"></param>
    /// <returns></returns>
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner.Creature)
		{
            Flash();
            // 完美格挡，增加敏捷叠加量
            if(result.UnblockedDamage == 0){
                UpdateValue(base.DynamicVars["Amount"].BaseValue + 1m);
            }else{
                // 未完美格挡，重置敏捷叠加量
                UpdateValue(0m);
            }
		}
    }
    /// <summary>
    /// 回合结束时，移除给予的玩家敏捷
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="side"></param>
    /// <returns></returns>
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {

        // 如果当前回合是玩家回合，移除给予的玩家敏捷
        if (side == base.Owner.Creature.Side)
		{
            Flash();
			await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, -base.DynamicVars["DexterityApplied"].BaseValue, base.Owner.Creature, null, silent: true);
            base.DynamicVars["DexterityApplied"].BaseValue = 0m;
		}
    }
}
