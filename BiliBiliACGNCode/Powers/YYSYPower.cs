//****************** 代码文件申明 ***********************
//* 文件：YYSYPower
//* 作者：wheat
//* 创建时间：2026/03/29 星期日
//* 描述：能力 YYSY（占位实现，效果请在 Hook 中补充）
//*******************************************************
using System.Linq;
using BaseLib.Extensions;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;

//****************** 代码文件申明 ***********************
//* 文件：YYSYPower
//* 作者：wheat
//* 创建时间：2026/03/29 星期日
//* 描述：能力 有一说一
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class YYSYPower : PowerBaseModel
{
    protected override string customIconPath => "yysy";

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

}
