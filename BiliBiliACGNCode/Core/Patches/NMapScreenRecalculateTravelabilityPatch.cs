//****************** 代码文件申明 ***********************
//* 文件：NMapScreenRecalculateTravelabilityPatch
//* 作者：wheat
//* 创建时间：2026/04/02
//* 描述：在 NMapScreen.RecalculateTravelability 原版前后插入逻辑（不替换原版）
//*******************************************************

using System.Collections;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Modifiers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

/// <summary>
/// 在 <c>NMapScreen.RecalculateTravelability</c> 执行前（Prefix）与执行后（Postfix）插入模组逻辑；
/// Prefix 不返回 false，原版方法会照常运行。
/// </summary>
[HarmonyPatch]
public static class NMapScreenRecalculateTravelabilityPatch
{
    private static MethodBase? _target;

    private static bool _useFlightStyleNextRow = false;
    /// <summary>
    /// 与原版 <c>Modifiers.OfType&lt;Flight&gt;().Any()</c> 分支对应：
    /// <see langword="true"/> 使用 <c>_map.GetPointsInRow(mapCoord.row + 1)</c>；
    /// <see langword="false"/> 使用 <c>_mapPointDictionary[mapCoord].Point.Children</c>。
    /// 可由遗物等在运行时赋值。
    /// </summary>
    public static bool UseFlightStyleNextRow
    {
        get => _useFlightStyleNextRow;
        set
        {
            if(value == _useFlightStyleNextRow) return;
            _useFlightStyleNextRow = value;
            try
            {
                InvokeRecalculateTravelabilityReflect(NMapScreen.Instance);
            }
            catch
            {
                // Instance 为空或版本差异时忽略
            }
            RefreshMapVisual();
        }
    }

    /// <summary>
    /// 找不到目标方法时跳过本补丁，避免启动报错（游戏版本变更时需核对类名是否仍为 NMapScreen）。
    /// </summary>
    [HarmonyPrepare]
    public static bool Prepare()
    {
        _target = ResolveTarget();
        return _target != null;
    }

    public static MethodBase TargetMethod() => _target!;

    /// <summary>
    /// 反射调用 <c>NMapScreen</c> 实例上的 <c>private void RecalculateTravelability()</c>，走完整原版流程并触发本类的 Harmony Postfix。
    /// </summary>
    private static void InvokeRecalculateTravelabilityReflect(NMapScreen? instance)
    {
        if (instance is null)
            return;
        var mb = _target ?? AccessTools.Method(typeof(NMapScreen), "RecalculateTravelability", Type.EmptyTypes);
        mb?.Invoke(instance, null);
    }

    [HarmonyPrefix]
    public static void Prefix(object __instance)
    {
    }

    /// <summary>
    /// 在原版重算结束后，按 <see cref="UseFlightStyleNextRow"/> 再应用一层与原版 Flight 分支等价的可通行扩展。
    /// </summary>
    [HarmonyPostfix]
    public static void Postfix(object __instance)
    {
        if (__instance is null)
            return;
        try
        {
            ApplyTravelabilityBranchLikeOriginal(__instance, UseFlightStyleNextRow);
        }
        catch
        {
            // 版本差异或反射失败时静默跳过，避免地图卡死
        }
    }
    /// <summary>
    /// 刷新地图
    /// </summary>
    public static void RefreshMapVisual(){
        NMapScreen.Instance?.RefreshAllPointVisuals();
    }
    /// <summary>
    /// 对应原版片段：
    /// if (mapCoord.row != _map.GetRowCount() - 1) {
    ///   var enumerable = flight ? _map.GetPointsInRow(mapCoord.row + 1) : _mapPointDictionary[mapCoord].Point.Children;
    ///   foreach (MapPoint item in enumerable) { _mapPointDictionary[item.coord].State = MapPointState.Travelable; }
    /// }
    /// </summary>
    private static void ApplyTravelabilityBranchLikeOriginal(object nMapScreen, bool useFly)
    {
        // 如果不启用飞行那就不执行
        if(useFly == false) return;
        // 如果已经有飞行规则了，那就不做更改
        var runState = RunManager.Instance.DebugOnlyGetState();
        if(runState == null || runState.Modifiers.OfType<Flight>().Any()) return;
        // 获取地图和地图点字典
        var screenType = nMapScreen.GetType();
        var map = AccessTools.DeclaredField(screenType, "_map")?.GetValue(nMapScreen);
        var dictObj = AccessTools.DeclaredField(screenType, "_mapPointDictionary")?.GetValue(nMapScreen);
        if (map == null || dictObj == null)
            return;

        // 获取地图行数和地图点行数
        var mapType = map.GetType();
        var getRowCount = AccessTools.Method(mapType, "GetRowCount");
        var getPointsInRow = AccessTools.Method(mapType, "GetPointsInRow", [typeof(int)]);
        var rowCountObj = getRowCount.Invoke(map, null);
        var rowCount = rowCountObj is int i ? i : rowCountObj is long l ? (int)l : (int?)null;
        if (rowCount is null || getPointsInRow == null)
            return;

        if (dictObj is not IDictionary rawDict)
            return;

        foreach (var keyObj in rawDict.Keys)
        {
            if (keyObj is null)
                continue;
            var row = GetMapCoordRow(keyObj);
            if (row is null)
                continue;
            if (row.Value == rowCount.Value - 1)
                continue;

            IEnumerable? enumerable = getPointsInRow.Invoke(map, [row.Value + 1]) as IEnumerable;

            if (enumerable == null)
                continue;

            foreach (var item in enumerable)
            {
                if (item is null)
                    continue;
                var coord = GetMapPointCoord(item);
                if (coord == null)
                    continue;
                if (!rawDict.Contains(coord))
                    continue;
                var entry = rawDict[coord];
                if (entry is null)
                    continue;
                SetMapPointNodeStateTravelable(entry);
            }
        }
    }

    private static IEnumerable? GetChildrenEnumerableFromDictionary(IDictionary dict, object mapCoordKey)
    {
        var wrapper = dict[mapCoordKey];
        if (wrapper is null)
            return null;
        var point = wrapper.GetType().GetProperty("Point", InstancePublic)?.GetValue(wrapper);
        if (point is null)
            return null;
        return point.GetType().GetProperty("Children", InstancePublic)?.GetValue(point) as IEnumerable;
    }

    private static object? GetMapPointCoord(object mapPoint)
    {
        var t = mapPoint.GetType();
        return t.GetProperty("coord", InstancePublic)?.GetValue(mapPoint)
               ?? t.GetProperty("Coord", InstancePublic)?.GetValue(mapPoint);
    }

    private static int? GetMapCoordRow(object mapCoord)
    {
        var t = mapCoord.GetType();
        if (t.GetField("row", InstancePublic)?.GetValue(mapCoord) is int r)
            return r;
        if (t.GetProperty("row", InstancePublic)?.GetValue(mapCoord) is int r2)
            return r2;
        if (t.GetProperty("Row", InstancePublic)?.GetValue(mapCoord) is int r3)
            return r3;
        return null;
    }

    private static void SetMapPointNodeStateTravelable(object mapPointDictionaryValue)
    {
        var stateProp = mapPointDictionaryValue.GetType().GetProperty("State", InstancePublic);
        if (stateProp?.PropertyType is not { IsEnum: true } stateType)
            return;
        var travelable = Enum.GetValues(stateType).Cast<object>()
            .FirstOrDefault(v => v.ToString() == "Travelable");
        if (travelable != null)
            stateProp.SetValue(mapPointDictionaryValue, travelable);
    }

    private const BindingFlags InstancePublic = BindingFlags.Public | BindingFlags.Instance;

    private static MethodBase? ResolveTarget()
    {
        try
        {
            foreach (var t in typeof(ModelDb).Assembly.GetTypes())
            {
                if (t.Name != "NMapScreen")
                    continue;
                var m = AccessTools.Method(t, "RecalculateTravelability", Type.EmptyTypes);
                if (m != null)
                    return m;
            }
        }
        catch (ReflectionTypeLoadException)
        {
            // 部分类型加载失败时忽略，视为未找到
        }

        return null;
    }
}
