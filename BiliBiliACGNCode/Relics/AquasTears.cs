//****************** 代码文件申明 ***********************
//* 文件：AquasTears
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：阿库娅的眼泪 每当你被施加负面状态，获得临时{Amount}力量
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class AquasTears : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Amount", 2m)];
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<StrengthPower>()];
    public override async Task BeforePowerAmountChanged(PowerModel power, decimal amount, Creature target, Creature? applier, CardModel? cardSource)
    {
        if (power.Type == PowerType.Debuff && amount > 0m && target == base.Owner.Creature)
		{
            Flash();
			await PowerCmd.Apply<FlexPotionPower>(base.Owner.Creature, base.DynamicVars["Amount"].BaseValue, base.Owner.Creature, null);
		}
    }
}
