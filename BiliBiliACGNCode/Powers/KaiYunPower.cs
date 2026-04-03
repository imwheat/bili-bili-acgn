//****************** 代码文件申明 ***********************
//* 文件：KaiYunPower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 开云
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class KaiYunPower : PowerBaseModel
{
    protected override string customIconPath => "kai_yun";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
}