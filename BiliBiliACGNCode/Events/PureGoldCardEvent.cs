//****************** 代码文件申明 ***********************
//* PureGoldCardEvents
//* 作者：wheat
//* 创建时间：2026/04/01 18:43:00 星期三
//* 描述：纯金卡牌事件 
//*******************************************************
using BiliBiliACGN.BiliBiliACGNCode.Utils;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Core.Models.Encounters;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace BiliBiliACGN.BiliBiliACGNCode.Events;

[EventPool(typeof(SharedEventPool))]
public sealed class PureGoldCardEvent : EventBaseModel
{
    public override bool IsShared => true;
    public override IReadOnlySet<Type> OwnerActTypes => new HashSet<Type> { };
    public override EventLayoutType LayoutType => EventLayoutType.Default;
    private static readonly string _trialNondescriptVfx = SceneHelper.GetScenePath("vfx/events/trial_nondescript_vfx");
    private static string ParkPath => ImageHelper.GetImagePath("events/pure_gold_card_park.png");
    public override EncounterModel? CanonicalEncounter => ModelDb.Encounter<PureGoldCardEncounter>();
    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
            new EventOption(this, Pickup, "PURE_GOLD_CARD.pages.INITIAL.options.PICKUP", HoverTipFactory.FromRelic<PureGoldCard>()),
            new EventOption(this, GiveItBack, "PURE_GOLD_CARD.pages.INITIAL.options.GIVEITBACK"),
        ];
    }

    /// <summary>
    /// 捡起卡牌
    /// 获得诅咒[red]青眼白龙[/red],进入[gold]商店[/gold]时移除。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task Pickup()
    {
        // 获得24k纯金卡遗物
        await RelicCmd.Obtain<PureGoldCard>(base.Owner);
    }
    /// <summary>
    /// 添加vfx到事件场景图片
    /// </summary>
    /// <param name="portraitPath"></param>
    private void AddVfxAnchoredToPortrait(string portraitPath)
	{
		if (LocalContext.IsMe(base.Owner))
		{
			Node2D node2D = PreloadManager.Cache.GetScene(portraitPath).Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
			node2D.Position = new Vector2(292f, 68f);
			NEventRoom.Instance.Layout.AddVfxAnchoredToPortrait(node2D);
		}
	}
    /// <summary>
    /// 物归原主
    /// 决定去附近的龟有公园派驻所物归原主。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private async Task GiveItBack()
    {
        AddVfxAnchoredToPortrait(_trialNondescriptVfx);
        // 设置事件场景图片
        if (LocalContext.IsMe(base.Owner))
		{
			NEventRoom.Instance.SetPortrait(PreloadManager.Cache.GetTexture2D(ParkPath));
		}

        SetEventState(L10NLookup("PURE_GOLD_CARD.pages.PARK.description"), [
			new EventOption(this, AcceptDuel, "PURE_GOLD_CARD.pages.PARK.options.DUEL", HoverTipFactory.FromCard<FourthBlueEyesWhiteDragon>()),
			new EventOption(this, RefuseDuel, "PURE_GOLD_CARD.pages.PARK.options.REFUSE")
		]);
    }
    private async Task AcceptDuel()
    {
        // 接受决斗
        EnterCombatWithoutExitingEvent<PureGoldCardEncounter>([
            new CardReward(new CardCreationOptions(new List<CardModel> { ModelDb.Card<FourthBlueEyesWhiteDragon>() }, CardCreationSource.Encounter, CardRarityOddsType.Uniform), 1, base.Owner),
            new PotionReward(base.Owner)
        ], false);
    }
    private async Task RefuseDuel()
    {
        // 拒绝决斗
        SetEventFinished(L10NLookup("PURE_GOLD_CARD.pages.PARK.LEAVE.description"));
    }

}
