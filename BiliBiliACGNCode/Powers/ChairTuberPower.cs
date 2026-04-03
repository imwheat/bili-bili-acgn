//****************** 代码文件申明 ***********************
//* 文件：ChairTuberPower
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 椅子主播 每回合自动打出{Cards:diff()}张带[gold]有一说一[/gold]的牌。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class ChairTuberPower : PowerBaseModel
{
    protected override string customIconPath => "chairtuber";
    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    private int currentAmount = 0;
    /// <summary>
    /// 自动打出的卡牌集合
    /// </summary>
    private HashSet<CardModel> _autoplayingCards;
    /// <summary>
    /// 自动打出的卡牌集合
    /// </summary>
    private HashSet<CardModel> AutoplayingCards
	{
		get
		{
			AssertMutable();
			if (_autoplayingCards == null)
			{
				_autoplayingCards = new HashSet<CardModel>();
			}
			return _autoplayingCards;
		}
	}

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        currentAmount = 0;
    }
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if(currentAmount < base.Amount && card.Owner == base.Owner.Player){
            // 自动打出有一说一的卡牌
            if(card.Keywords.Contains(CustomKeyWords.YYSY) && !AutoplayingCards.Contains(card)){
                currentAmount++;
                AutoplayingCards.Add(card);
                await CardCmd.AutoPlay(choiceContext, card, null);
                AutoplayingCards.Remove(card);
            }
        }
    }
}