//****************** 代码文件申明 ***********************
//* 文件：MillenniumPuzzle
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：千年积木
//*******************************************************

using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace BiliBiliACGN.BiliBiliACGNCode.Relics;

[Pool(typeof(SharedRelicPool))]
public sealed class MillenniumPuzzle : RelicBaseModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
}
