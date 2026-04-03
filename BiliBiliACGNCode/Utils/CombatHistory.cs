//****************** 代码文件申明 ***********************
//* 文件：CombatHistory
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：战斗历史记录工具类
//*******************************************************

using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Utils;

public static class CombatHistory
{
    public static bool CheckLastCardPlayedType(Player player, CardModel card, CardType cardType)
    {
        CardPlayStartedEntry cardPlayStartedEntry = CombatManager.Instance.History.CardPlaysStarted.LastOrDefault((CardPlayStartedEntry e) => e.CardPlay.Card.Owner == player && e.HappenedThisTurn(player.Creature.CombatState) && e.CardPlay.Card != card);
        if (cardPlayStartedEntry == null)
        {
            return false;
        }
        return cardPlayStartedEntry.CardPlay.Card.Type == cardType;
    }
}