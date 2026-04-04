//****************** 代码文件申明 ***********************
//* 文件：TouchOfOrobasGetUpgradedStarterRelicPatch
//* 作者：wheat
//* 创建时间：2026/04/04
//* 描述：Harmony Postfix，在 TouchOfOrobas.GetUpgradedStarterRelic 返回后允许模组改写升级后的起始遗物
//*******************************************************
using System.Collections.Generic;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace BiliBiliACGN.BiliBiliACGNCode.Core.Patches;

[HarmonyPatch(typeof(TouchOfOrobas))]
public static class TouchOfOrobasGetUpgradedStarterRelicPatch
{
    /// <summary>
    /// 在原版 <c>GetUpgradedStarterRelic</c> 计算完返回值之后调用；
    /// 通过修改 <paramref name="__result"/> 覆盖最终返回的遗物。
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch("get_RefinementUpgrades")]
    public static void GetRefinementUpgrades_Postfix(ref Dictionary<ModelId, RelicModel> __result)
    {
        // 如果原版 __result 中没有阿尔茨海默症，则添加阿尔茨海默症晚期
        if(__result.ContainsKey(ModelDb.Relic<Alzheimer>().Id))
            return;
        __result[ModelDb.Relic<Alzheimer>().Id] = ModelDb.Relic<AlzheimerLateStage>();
    }
}
