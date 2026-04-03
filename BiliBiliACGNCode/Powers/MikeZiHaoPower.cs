//****************** 代码文件申明 ***********************
//* 文件：MikeZiHaoPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 麦克子豪
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class MikeZiHaoPower : PowerBaseModel
{
    protected override string customIconPath => "mikezihao";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 进入红怒时获得 Amount 点力量
}
