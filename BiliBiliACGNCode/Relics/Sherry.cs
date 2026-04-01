//****************** 代码文件申明 ***********************
//* 文件：Sherry
//* 作者：wheat
//* 创建时间：2026/04/01 19:40:00 星期三
//* 描述：雪莉
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class Sherry : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<ASherry>()];
    public override async Task AfterObtained()
    {
        // 获得啊~雪莉
        CardModel card = base.Owner.RunState.CreateCard<ASherry>(base.Owner);
		CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
        // 移除受伤的雪莉
        var sherryHurt = base.Owner.GetRelic<SherryHurt>();
        if(sherryHurt != null)
        {
            base.Owner.RemoveRelicInternal(sherryHurt, false);
        }
    }
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner)
		{
			return false;
		}
        var removeOption = options.FirstOrDefault(option => option.OptionId == "HealSherry");
        if (removeOption != null)
        {
            options.Remove(removeOption);
        }
		return true;
	}
}