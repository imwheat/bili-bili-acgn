//* 文件：HailOfBladesPower
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：能力 丛刃 红怒时所有攻击牌卡费-1
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class HailOfBladesPower : PowerBaseModel
{
    protected override string customIconPath => "hail_of_blades";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		modifiedCost = originalCost;
		if (card.Owner.Creature != base.Owner)
		{
			return false;
		}
		if(base.Owner.HasPower<RagePower>())
		{
			return false;
		}
		if (card.Type != CardType.Attack)
		{
			return false;
		}
		if (originalCost <= 0m)
		{
			return false;
		}
		modifiedCost = originalCost - (decimal)base.Amount;
		if (modifiedCost < 0m)
		{
			modifiedCost = default(decimal);
		}
		return true;
	}
}