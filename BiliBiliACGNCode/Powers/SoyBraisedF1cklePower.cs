//****************** 代码文件申明 ***********************
//* 文件：SoyBraisedF1cklePower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 红烧飞扣
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SoyBraisedF1cklePower : PowerBaseModel
{
    protected override string customIconPath => "soybraisedf1ckle";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 战斗结束后回复 Amount 点生命
}
