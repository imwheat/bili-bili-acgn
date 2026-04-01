//****************** 代码文件申明 ***********************
//* 文件：Alzheimer
//* 作者：wheat
//* 创建时间：2026/03/31 13:00:00 星期二
//* 描述：阿尔茨海默症
//*******************************************************

using BaseLib.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Entities.RestSite;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(BottleRelicPool))]
public sealed class Alzheimer : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
	{
		if (player != base.Owner)
		{
			return false;
		}
		options.Add(new AlzheimerRestSiteOption(player));
		return true;
	}
}