//****************** 代码文件申明 ***********************
//* 文件：HappyWaterCowPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 欢乐水牛
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class HappyWaterCowPower : PowerBaseModel
{
    protected override string customIconPath => "happywatercow";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 本回合受到伤害减半
}
