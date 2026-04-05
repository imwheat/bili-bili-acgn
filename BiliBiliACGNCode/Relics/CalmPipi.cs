//****************** 代码文件申明 ***********************
//* 文件：CalmPipi
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：冷静的pipi美 结束回合后，如果你完美格挡本轮攻击，下一回合临时获得+1敏捷，持续叠加
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
    public override bool ShowCounter => CombatManager.Instance.IsInProgress;
    private int dmg = 0;
    private void UpdateValue(decimal value)
	{
        base.DynamicVars["Amount"].BaseValue = value;
		InvokeDisplayAmountChanged();
	}
    /// <summary>
    /// <summary>
    /// 玩家回合开始时，给予玩家敏捷
    /// </summary>
    /// <param name="choiceContext"></param>
    /// <param name="player"></param>
    /// <returns></returns>
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
	{
        if(player != base.Owner){
            return;
        }
        if(dmg > 0){
            UpdateValue(0m);
        }else{
            Flash();
            UpdateValue(base.DynamicVars["Amount"].BaseValue + 1m);
            await PowerCmd.Apply<SpeedPotionPower>(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, null);
        }
        dmg = 0;
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
            dmg += result.UnblockedDamage;
		}
    }
}
