//****************** 代码文件申明 ***********************
//* 文件：MysteryWaterCard
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：神秘的水卡 商店药水免费
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MysteryWaterCard : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Uncommon;
    public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal originalPrice)
	{
        // 不是玩家自己，或者不是本地玩家，则不修改价格
		if (player != base.Owner)
		{
			return originalPrice;
		}
        // 不是自己
		if (!LocalContext.IsMe(base.Owner))
		{
			return originalPrice;
		}
        // 是药水，则免费
        if(entry is MerchantPotionEntry){
            return 0m;
        }
        // 其他情况，不修改价格
		return originalPrice;
	}
}
