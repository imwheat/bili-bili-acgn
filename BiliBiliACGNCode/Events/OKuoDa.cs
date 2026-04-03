//****************** 代码文件申明 ***********************
//* OKuoDaEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：哦跨哒事件(Pop子与Pipi美)
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class OKuoDa : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    public override EncounterModel? CanonicalEncounter => ModelDb.Encounter<StrangeMurmurEncounter>();

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Oguoda, "O_KUO_DA.pages.INITIAL.options.OGUODA"),
            new EventOption(this, Naiyo, "O_KUO_DA.pages.INITIAL.options.NAIYO"),
            new EventOption(this, Combat, "O_KUO_DA.pages.INITIAL.options.COMBAT")
        ];
    }

    private async Task Oguoda()
    {
        // 获得遗物愤怒的pop子
        await RelicCmd.Obtain<AngryPop>(base.Owner);
    }

    private async Task Naiyo()
    {
        // 获得遗物冷静的pipi美
        await RelicCmd.Obtain<CalmPipi>(base.Owner);
    }

    private Task Combat()
    {
        // 进入战斗
        EnterCombatWithoutExitingEvent<OKuoDaEncounter>([
            new RelicReward(ModelDb.Relic<UltimateShitAnimeCommittee>(), base.Owner),
            new PotionReward(base.Owner)
        ], false);
        
        return Task.CompletedTask;
    }
}
