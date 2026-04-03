//****************** 代码文件申明 ***********************
//* 文件：RedBullPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 红牛
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class RedBullPower : PowerBaseModel
{
    protected override string customIconPath => "redbull";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 每获得 Amount 点红温获得 1 点能量（监听红温变化）
}
