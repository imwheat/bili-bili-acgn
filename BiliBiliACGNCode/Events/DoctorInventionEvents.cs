//****************** 代码文件申明 ***********************
//* DoctorInventionEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：博士发明事件
//*******************************************************
using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class DoctorInventionEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Try, "DOCTOR_INVENTION.pages.INITIAL.options.TRY"),
            new EventOption(this, Buy, "DOCTOR_INVENTION.pages.INITIAL.options.BUY")
        ];
    }

    private Task Try()
    {
        // TODO: 实现选项 TRY 的具体逻辑
        return Task.CompletedTask;
    }

    private Task Buy()
    {
        // TODO: 实现选项 BUY 的具体逻辑
        return Task.CompletedTask;
    }
}
