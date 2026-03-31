//****************** 代码文件申明 ***********************
//* 文件：InfiniteBullnessPower
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 无量牛处 每回合自动打出{Cards:diff()}张带[gold]有一说一[/gold]的牌。
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
using MegaCrit.Sts2.Core.Logging;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class InfiniteBullnessPower : PowerBaseModel
{
    protected override string customIconPath => "infinitebullness";
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
        Log.Info($"无量牛处触发：当前数量{currentAmount}，最大数量{base.Amount}");
        if(currentAmount < base.Amount){
            currentAmount++;
            // 自动打出有一说一的卡牌
            if(card.Keywords.Contains(CustomKeyWords.YYSY) && !AutoplayingCards.Contains(card)){
                AutoplayingCards.Add(card);
                await CardCmd.AutoPlay(choiceContext, card, null);
                AutoplayingCards.Remove(card);
            }
        }
    }
}