//****************** 代码文件申明 ***********************
//* 文件：NoRightToKnightMePower
//* 作者：wheat
//* 创建时间：2026/04/03 星期五
//* 描述：能力 无权为我授勋
//*******************************************************

using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class NoRightToKnightMePower : PowerBaseModel
{
    protected override string customIconPath => "no_right_to_knight_me";
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    
}