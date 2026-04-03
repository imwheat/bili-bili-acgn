//****************** 代码文件申明 ***********************
//* 文件：BullDemonPower
//* 作者：wheat
//* 创建时间：2026/04/03 12:00:00 星期五
//* 描述：能力 牛魔形态
//*******************************************************
using MegaCrit.Sts2.Core.Entities.Powers;

namespace BiliBiliACGN.BiliBiliACGNCode.Powers;

public sealed class BullDemonPower : PowerBaseModel
{
    protected override string customIconPath => "bulldemon";

    public override PowerType Type => PowerType.Buff;

    public override PowerStackType StackType => PowerStackType.Counter;

    //TODO: 每打出一张有一说一获得 Amount 点「唐氏」（需对接项目内唐氏 Power/机制）
}
