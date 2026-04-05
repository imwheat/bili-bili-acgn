//****************** 代码文件申明 ***********************
//* 文件：PureGoldCard
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：24k纯金卡（遗物占位，逻辑待实现）
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class PureGoldCard : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromCard<BlueEyesWhiteDragon>()];
    private bool _wasUsed;

	public override bool IsUsedUp => _wasUsed;

	[SavedProperty]
	public bool WasUsed
	{
		get
		{
			return _wasUsed;
		}
		set
		{
			AssertMutable();
			_wasUsed = value;
			if (IsUsedUp)
			{
				base.Status = RelicStatus.Disabled;
			}
		}
    }
    public override async Task AfterObtained()
    {
        // 获得青眼白龙
        CardModel card = base.Owner.RunState.CreateCard<BlueEyesWhiteDragon>(base.Owner);
        CardCmd.PreviewCardPileAdd(new List<CardPileAddResult>(){await CardPileCmd.Add(card, PileType.Deck)}, 2f);
    }
    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if(room is MerchantRoom){
            Flash();
            base.Status = RelicStatus.Disabled;
            // 移除所有的青眼白龙
            var qingYanBlueEyesWhiteDragon = base.Owner.Deck.Cards.Where(card => card is BlueEyesWhiteDragon).ToList();
            await PlayerCmd.GainGold(870m * qingYanBlueEyesWhiteDragon.Count, base.Owner);
            await CardPileCmd.RemoveFromDeck(qingYanBlueEyesWhiteDragon);
            WasUsed = true;
        }

    }


}

