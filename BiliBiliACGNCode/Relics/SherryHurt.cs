//****************** 代码文件申明 ***********************
//* 文件：SherryHurt
//* 作者：wheat
//* 创建时间：2026/04/01 19:20:00 星期三
//* 描述：受伤的雪莉
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(EventRelicPool))]
public sealed class SherryHurt : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner)
		{
			return false;
		}
		options.Add(new HealSherryRestSiteOption(player));
		return true;
	}
}