//****************** 代码文件申明 ***********************
//* EHeiJiangEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：诶嘿酱事件
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

public sealed class EHeiJiang : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, WearMask, "E_HEI_JIANG.pages.INITIAL.options.WEARMASK", HoverTipFactory.FromRelic<EiHeiMask>()),
            new EventOption(this, Leave, "E_HEI_JIANG.pages.INITIAL.options.LEAVE")
        ];
    }

    private async Task WearMask()
    {
        // 获得诶嘿酱的面具
        await RelicCmd.Obtain<EiHeiMask>(base.Owner);
    }


    private Task Leave()
    {
        SetEventFinished(L10NLookup("E_HEI_JIANG.pages.LEAVE.description"));
        return Task.CompletedTask;
    }
}
