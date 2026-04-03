//* ChampagnePopPower
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：能力 开香槟 你的下一张[gold]有一说一[/gold]，耗能变成0。
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ChampagnePopPower : PowerBaseModel
{
    protected override string customIconPath => "champagne_pop";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
	{
		modifiedCost = originalCost;
		if (card.Owner.Creature != base.Owner)
		{
			return false;
		}
		if (!card.Keywords.Contains(CustomKeyWords.YYSY))
		{
			return false;
		}
		bool flag;
		switch (card.Pile?.Type)
		{
		case PileType.Hand:
		case PileType.Play:
			flag = true;
			break;
		default:
			flag = false;
			break;
		}
		if (!flag)
		{
			return false;
		}
		modifiedCost = default(decimal);
		return true;
	}

	public override async Task BeforeCardPlayed(CardPlay cardPlay)
	{
		if (cardPlay.Card.Owner.Creature == base.Owner && cardPlay.Card.Type == CardType.Attack)
		{
			bool flag;
			switch (cardPlay.Card.Pile?.Type)
			{
			case PileType.Hand:
			case PileType.Play:
				flag = true;
				break;
			default:
				flag = false;
				break;
			}
			if (flag)
			{
				await PowerCmd.Decrement(this);
			}
		}
	}

}