//****************** 代码文件申明 ***********************
//* 文件：EventRegister
//* 作者：wheat
//* 创建时间：2026/04/01 10:00:00 星期二
//* 描述：事件注册器（通过 EventPoolAttribute 反射发现 EventBaseModel）
//*******************************************************
using System.Reflection;
using BiliBiliACGN.BiliBiliACGNCode.Events;
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Core;

public static class EventRegister
{
    private static readonly Dictionary<Type, List<EventModel>> ExtraEventsByActType = [];
    private static readonly List<EventModel> ExtraSharedEvents = [];
    private static bool _initialized;
    private static IReadOnlyList<(Type EventType, Type PoolType)>? _discovered;

    /// <summary>
    /// 扫描当前程序集中所有带 <see cref="EventPoolAttribute"/> 的 <see cref="EventBaseModel"/> 具体类型。
    /// </summary>
    private static IReadOnlyList<(Type EventType, Type PoolType)> DiscoverEventTypes()
    {
        if (_discovered != null)
            return _discovered;

        var assembly = typeof(EventBaseModel).Assembly;
        var list = new List<(Type, Type)>();
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            types = ex.Types.Where(t => t != null).Cast<Type>().ToArray();
            foreach (var le in ex.LoaderExceptions ?? [])
                Log.Error($"[B站动画区Mod] 事件类型加载异常：{le}");
        }
        catch (Exception ex)
        {
            Log.Error($"[B站动画区Mod] 扫描事件类型失败：{ex}");
            types = [];
        }

        foreach (var t in types)
        {
            if (t is null || !typeof(EventBaseModel).IsAssignableFrom(t) || t.IsAbstract)
                continue;
            var attr = t.GetCustomAttribute<EventPoolAttribute>(inherit: false);
            if (attr == null)
                continue;

            if (attr.PoolType != typeof(SharedEventPool) && !typeof(ActModel).IsAssignableFrom(attr.PoolType))
            {
                Log.Error($"[B站动画区Mod] 事件 {t.FullName} 的 EventPool 无效：{attr.PoolType.FullName}（需为 SharedEventPool 或 ActModel 子类）");
                continue;
            }

            list.Add((t, attr.PoolType));
        }

        _discovered = list;
        return _discovered;
    }

    /// <summary>
    /// 注入事件到 ModelDb（需在 ModelDb 可用时调用）。
    /// </summary>
    public static void InjectEvents()
    {
        foreach (var (eventType, _) in DiscoverEventTypes())
            ModelDb.Inject(eventType);
    }

    /// <summary>
    /// 注册事件到 Extra 池（共享 / 按 Act）。
    /// </summary>
    public static void RegisterEvents()
    {
        if (_initialized)
            return;
        _initialized = true;

        Log.Info($"[B站动画区Mod] 事件注册开始：AllEvents={ModelDb.AllEvents.Count()}");
        InjectEvents();

        var eventMethod = typeof(ModelDb).GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == nameof(ModelDb.Event) && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
        if (eventMethod == null)
        {
            Log.Error("[B站动画区Mod] 未找到 ModelDb.Event<>()，事件注册中止");
            return;
        }

        foreach (var (eventType, poolType) in DiscoverEventTypes())
        {
            EventModel? ev = null;
            try
            {
                ev = eventMethod.MakeGenericMethod(eventType).Invoke(null, null) as EventModel;
            }
            catch (Exception ex)
            {
                Log.Error($"[B站动画区Mod] ModelDb.Event 失败：{eventType.FullName}，{ex}");
            }

            if (ev != null)
                Register(ev, poolType);
        }

        Log.Info($"[B站动画区Mod] 事件注册完成：按 Act 注入={ExtraEventsByActType.Sum(x => x.Value.Count)}，共享事件={ExtraSharedEvents.Count}");
    }

    private static void Register(EventModel eventModel, Type poolType)
    {
        if (eventModel is not EventBaseModel)
            return;

        if (poolType == typeof(SharedEventPool))
        {
            if (!ExtraSharedEvents.Any(e => e.GetType() == eventModel.GetType()))
                ExtraSharedEvents.Add(eventModel);
            return;
        }

        if (!ExtraEventsByActType.TryGetValue(poolType, out var list))
        {
            list = [];
            ExtraEventsByActType[poolType] = list;
        }

        if (!list.Any(e => e.GetType() == eventModel.GetType()))
            list.Add(eventModel);
    }

    public static IEnumerable<EventModel> GetExtraEventsForAct(Type actType) =>
        ExtraEventsByActType.TryGetValue(actType, out var list) ? list : [];

    public static IEnumerable<EventModel> GetExtraSharedEvents() => ExtraSharedEvents;
}
