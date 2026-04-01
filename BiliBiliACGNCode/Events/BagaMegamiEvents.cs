//****************** 代码文件申明 ***********************
//* BagaMegamiEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：智障女神事件
//*******************************************************
using MegaCrit.Sts2.Core.Events;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class BagaMegamiEvents : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Try, "BAGA_MEGAMI_.pages.INITIAL.options.TRY"),
            new EventOption(this, No, "BAGA_MEGAMI_.pages.INITIAL.options.NO"),
            new EventOption(this, Megami, "BAGA_MEGAMI_.pages.INITIAL.options.MEGAMI")
        ];
    }

    private Task Try()
    {
        // TODO: 实现选项 TRY 的具体逻辑
        return Task.CompletedTask;
    }

    private Task No()
    {
        // TODO: 实现选项 NO 的具体逻辑
        return Task.CompletedTask;
    }

    private Task Megami()
    {
        // TODO: 实现选项 MEGAMI 的具体逻辑
        return Task.CompletedTask;
    }
}
