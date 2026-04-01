//****************** 代码文件申明 ***********************
//* 文件：AlzheimerRestSiteOption
//* 作者：wheat
//* 创建时间：2026/04/01 10:00:00 星期二
//* 描述：阿尔茨海默症休息站点选项
//*******************************************************

using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;

public sealed class AlzheimerRestSiteOption : RestSiteOption
{
    private const int _cardsToRemove = 1;
    public override string OptionId => "Alzheimer";

    public AlzheimerRestSiteOption(Player owner) : base(owner){
        base.IsEnabled = GetRemovableCardCount(owner) >= _cardsToRemove;
    }
	public override async Task<bool> OnSelect()
	{
		CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, _cardsToRemove){
            Cancelable = true,
            RequireManualConfirmation = true,
        };
		CardSelectorPrefs prefs = cardSelectorPrefs;
		IEnumerable<CardModel> enumerable = await CardSelectCmd.FromDeckForRemoval(base.Owner, prefs);
		if (!enumerable.Any())
		{
			return false;
		}
		foreach (CardModel item in enumerable)
		{
			await CardPileCmd.RemoveFromDeck(item);
		}
		return true;
	}

	private static int GetRemovableCardCount(Player player)
	{
		return PileType.Deck.GetPile(player).Cards.Count((CardModel c) => c.IsRemovable);
	}
}