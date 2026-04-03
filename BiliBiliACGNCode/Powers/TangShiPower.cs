//****************** 代码文件申明 ***********************
//* 文件：TangShiPower
//* 作者：wheat
//* 创建时间：2026/03/29 星期日
//* 描述：能力 唐氏
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class TangShiPower : PowerBaseModel
{
    protected override string customIconPath => "tangshi";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (base.Owner != dealer || cardSource == null)
		{
			return 0m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 0m;
		}
        if(cardSource.Keywords.Contains(CustomKeyWords.YYSY))
        {
            return base.Amount;
        }
        return 0m;
    }
    public override decimal ModifyBlockAdditive(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if(cardSource?.Owner != base.Owner.Player)
        {
            return 0m;
        }
        if (cardSource == null)
		{
			return 0m;
		}
		if (!props.IsPoweredAttack_())
		{
			return 0m;
		}
        if(cardSource.Keywords.Contains(CustomKeyWords.YYSY))
        {
            return base.Amount;
        }
        return 0m;
    }


}
