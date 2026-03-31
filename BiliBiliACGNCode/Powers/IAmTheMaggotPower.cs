//****************** 代码文件申明 ***********************
//* 文件：IAmTheMaggotPower(我是区)
//* 作者：wheat
//* 创建时间：2026/03/31 10:20:49 星期二
//* 描述：能力 我是区 每回合随机赋予一张手牌[gold]有一说一[/gold]。
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using Godot;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using BiliBiliACGN.BiliBiliACGNCode.Cards;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class IAmTheMaggotPower : PowerBaseModel
{
    protected override string customIconPath => "iamthemaggot";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var pile = PileType.Hand.GetPile(player);
        if (pile != null){
            // 获取所有没有有一说一的手牌
            List<CardModel> cards = new List<CardModel>();
            foreach(var card in pile.Cards){
                if(!card.Keywords.Contains(CustomKeyWords.YYSY)){
                    cards.Add(card);
                }
            }
            // 实际执行操作次数为没有有一说一的手牌数量和能力次数中的较小值
            int n = Mathf.Min(cards.Count, base.Amount);
            var randomCards = cards.TakeRandom(n, player.RunState.Rng.CombatCardSelection);
            foreach(var card in randomCards){
                card.AddKeyword(CustomKeyWords.YYSY);
            }
        }
    }
}