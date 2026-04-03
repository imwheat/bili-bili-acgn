//****************** 代码文件申明 ***********************
//* 文件：GetTangPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 变唐
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class GetTangPower : PowerBaseModel
{
    protected override string customIconPath => "gettang";

    public override PowerType Type => PowerType.Debuff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 攻击与格挡数值按 Amount 减少
}
