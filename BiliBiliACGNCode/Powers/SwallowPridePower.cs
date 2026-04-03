//****************** 代码文件申明 ***********************
//* 文件：SwallowPridePower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 忍气吞声
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class SwallowPridePower : PowerBaseModel
{
    protected override string customIconPath => "swallowpride";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 进入红怒时获得能量与格挡（见 powers.json / DynamicVars）
}
