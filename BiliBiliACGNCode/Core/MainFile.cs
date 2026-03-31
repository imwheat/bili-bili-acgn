//****************** 代码文件申明 ***********************
//* 文件：MainFile
//* 作者：wheat
//* 创建时间：2026/03/26 10:51:22 星期四
//* 描述：主文件，用于初始化Mod
//*******************************************************
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace BiliBiliACGN.BiliBiliACGNCode.Core;

[ModInitializer("Init")]
public class MainFile
{
	public const String ModId = "BiliBiliACGN";
	// 初始化函数
	public static void Init()
	{
		// 打patch（即修改游戏代码的功能）用
		// 传入参数随意，只要不和其他人撞车即可
		var harmony = new Harmony("sts2.NewNewGame.BiliBiliACGN");
		harmony.PatchAll();
		// 使得tscn可以加载自定义脚本
		ScriptManagerBridge.LookupScriptsInAssembly(typeof(MainFile).Assembly);
		Log.Debug("Mod initialized!");
	}
}
