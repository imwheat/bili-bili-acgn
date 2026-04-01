//****************** 代码文件申明 ***********************
//* StrangeMurmurEncounter
//* 作者：wheat
//* 创建时间：2026/04/01 10:00:00 星期二
//* 描述：奇怪的低语战斗场景
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters.Monsters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;

public sealed class StrangeMurmurEncounter : EncounterModel
{
	public override RoomType RoomType => RoomType.Monster;

	public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<PopPipi>()];
	// TODO 暂时先用这个，之后再改
	protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
	{
		PopPipi popPipi = (PopPipi)ModelDb.Monster<PopPipi>().ToMutable();
		popPipi.StartsWithStrongPunch = true;
		popPipi.StartingHpReduction = base.Rng.NextInt(2, 10);
		return [
			new (popPipi, null),
		];
	}
}
