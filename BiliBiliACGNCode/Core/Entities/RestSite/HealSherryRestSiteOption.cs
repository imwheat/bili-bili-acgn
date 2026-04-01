//****************** 代码文件申明 ***********************
//* 文件：HealSherryRestSiteOption
//* 作者：wheat
//* 创建时间：2026/04/01 19:37:00 星期三
//* 描述：治疗雪莉
//*******************************************************

using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;

public sealed class HealSherryRestSiteOption : RestSiteOption
{
    private const int _cardsToRemove = 2;
    public override string OptionId => "HealSherry";

    public HealSherryRestSiteOption(Player owner) : base(owner){
    }
	public override async Task<bool> OnSelect()
	{
        // 获得雪莉
        await RelicCmd.Obtain<Sherry>(base.Owner);
        // 移除2张牌
        CardSelectorPrefs cardSelectorPrefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 0, _cardsToRemove){
            Cancelable = true,
            RequireManualConfirmation = true,
        };
		CardSelectorPrefs prefs = cardSelectorPrefs;
		IEnumerable<CardModel> enumerable = await CardSelectCmd.FromDeckForRemoval(base.Owner, prefs);
		if (enumerable.Any())
		{
            foreach (CardModel item in enumerable)
            {
                await CardPileCmd.RemoveFromDeck(item);
            }
		}
		return true;
	}


}