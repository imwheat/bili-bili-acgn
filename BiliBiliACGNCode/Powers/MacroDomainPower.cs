//****************** 代码文件申明 ***********************
//* 文件：MacroDomainPower(宏观领域)
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 宏观领域 本回合你抽到的所有带[gold]有一说一[/gold]的手牌自动打出。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using Godot;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MacroDomainPower : PowerBaseModel
{
    protected override string customIconPath => "macrodomain";
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
        // 每回合结束时，减少1点
        if (side == base.Owner.Side)
		{
			Flash();
			await PowerCmd.Apply<MacroDomainPower>(base.Owner, -1, base.Owner, null);
		}
    }
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        // 自动打出有一说一的卡牌
        if(card.Keywords.Contains(CustomKeyWords.YYSY) && !AutoplayingCards.Contains(card)){
            AutoplayingCards.Add(card);
            await CardCmd.AutoPlay(choiceContext, card, null);
            AutoplayingCards.Remove(card);
        }
    }
}