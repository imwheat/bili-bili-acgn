//****************** 代码文件申明 ***********************
//* 文件：EventRegister
//* 作者：wheat
//* 创建时间：2026/04/01 10:00:00 星期二
//* 描述：事件注册器
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Events;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Core;

public static class EventRegister
{
    private static readonly Dictionary<Type, List<EventModel>> ExtraEventsByActType = [];
    private static readonly List<EventModel> ExtraSharedEvents = [];
    private static bool _initialized;
    private static readonly IReadOnlyList<Type> _eventTypes = [
        typeof(AnimeTimeMachine),
        typeof(BagaMegami),
        typeof(DesperateDaily),
        typeof(PureGoldCardEvent),
        typeof(Nuke),
        typeof(EHeiJiang),
        typeof(EvaEvents),
        typeof(HaKaSeInvention),
        typeof(OKuoDa),
        typeof(PureGoldCardEvent),
        typeof(SherryEvent),
        typeof(StrangeMurmur)
    ];
    /// <summary>
    /// 注入事件
    /// </summary>
    public static void InjectEvents()
    {
        foreach (var e in _eventTypes)
            ModelDb.Inject(e);
    }
    /// <summary>
    /// 注册事件
    /// </summary>
    public static void RegisterEvents()
    {
        if (_initialized)
            return;
        _initialized = true;

        Log.Info($"[B站动画区Mod] 事件注册开始：AllEvents={ModelDb.AllEvents.Count()}");
        InjectEvents();
        var events = new List<EventModel>(){
            ModelDb.Event<AnimeTimeMachine>(),
            ModelDb.Event<BagaMegami>(),
            ModelDb.Event<DesperateDaily>(),
            ModelDb.Event<PureGoldCardEvent>(),
            ModelDb.Event<Nuke>(),
            ModelDb.Event<EHeiJiang>(),
            ModelDb.Event<EvaEvents>(),
            ModelDb.Event<HaKaSeInvention>(),
            ModelDb.Event<OKuoDa>(),
            ModelDb.Event<PureGoldCardEvent>(),
            ModelDb.Event<SherryEvent>(),
            ModelDb.Event<StrangeMurmur>()
        };
        foreach (var e in events)
            Register(e);
        Log.Info($"[B站动画区Mod] 事件注册完成：按 Act 注入={ExtraEventsByActType.Sum(x => x.Value.Count)}，共享事件={ExtraSharedEvents.Count}");
    }

    private static void Register(EventModel eventModel)
    {
        if (eventModel is not EventBaseModel ebm)
            return;

        var ownerActTypes = ebm.OwnerActTypes;
        if (ownerActTypes == null || ownerActTypes.Count == 0)
        {
            if (!ExtraSharedEvents.Any(e => e.GetType() == eventModel.GetType()))
                ExtraSharedEvents.Add(eventModel);
            return;
        }

        foreach (var actType in ownerActTypes)
        {
            if (!ExtraEventsByActType.TryGetValue(actType, out var list))
            {
                list = [];
                ExtraEventsByActType[actType] = list;
            }
            if (!list.Any(e => e.GetType() == eventModel.GetType()))
                list.Add(eventModel);
        }
    }

    public static IEnumerable<EventModel> GetExtraEventsForAct(Type actType) =>
        ExtraEventsByActType.TryGetValue(actType, out var list) ? list : [];

    public static IEnumerable<EventModel> GetExtraSharedEvents() => ExtraSharedEvents;
}
