//****************** 代码文件申明 ***********************
//* 文件：AtFieldGenerator
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：A.T.力场发生器 每回合获得5点格挡，但每次受到未被格挡的伤害时，力场消失一回合。
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class AtFieldGenerator : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 5m)];
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.Static(StaticHoverTip.Block)];
    private bool _isActivating = true;
    private bool IsActivating
	{
		get
		{
			return _isActivating;
		}
		set
		{
			AssertMutable();
			_isActivating = value;
			UpdateDisplay();
		}
	}
    private void UpdateDisplay()
	{
		base.Status = IsActivating ? RelicStatus.Normal : RelicStatus.Disabled;
	}

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == base.Owner.Creature)
		{
            IsActivating = result.UnblockedDamage <= 0;
		}
    }
    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == base.Owner.Creature.Side)
		{
            // 如果力场正在激活，给予玩家护盾
            if(IsActivating)
            {
                Flash();
                await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, ValueProp.Unpowered, null);
            }
		}
    }

}
