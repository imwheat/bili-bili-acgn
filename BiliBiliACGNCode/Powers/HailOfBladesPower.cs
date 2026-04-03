//* 文件：HailOfBladesPower
//* 作者：wheat
//* 创建时间：2026/04/03
//* 描述：能力 丛刃
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class HailOfBladesPower : PowerBaseModel
{
    protected override string customIconPath => "hail_of_blades";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

}