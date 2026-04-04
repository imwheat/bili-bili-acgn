//****************** 代码文件申明 ***********************
//* 文件：BottleCharacter
//* 作者：wheat
//* 创建时间：2026/03/31 12:47:22 星期二
//* 描述：瓶子君152（牛子豪）角色
//*******************************************************

using BaseLib.Abstracts;
using BiliBiliACGN.BiliBiliACGNCode.Cards;
using BiliBiliACGN.BiliBiliACGNCode.Cards.CardPool;
using BiliBiliACGN.BiliBiliACGNCode.Potions.PotionPool;
using BiliBiliACGN.BiliBiliACGNCode.Relics;
using BiliBiliACGN.BiliBiliACGNCode.Relics.RelicPool;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace BiliBiliACGN.BiliBiliACGNCode.Characters;

public sealed class BottleCharacter : PlaceholderCharacterModel
{
    public override string PlaceholderID => "bottle";
    public override Color NameColor => new Color("789ccd");

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 80;

    public override CardPoolModel CardPool => ModelDb.CardPool<BottleCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BottleRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BottlePotionPool>();
    // 人物头像路径。
    public override string CustomIconTexturePath => "res://BiliBiliACGN/images/characters/bottle_icon.png";
    protected override string MapMarkerPath => "res://BiliBiliACGN/images/characters/bottle_map_marker.png";
    public override string CustomArmPointingTexturePath => "res://BiliBiliACGN/images/hands/bottle_point.png";
    public override string CustomArmPaperTexturePath => "res://BiliBiliACGN/images/hands/bottle_paper.png";
    public override string CustomArmRockTexturePath => "res://BiliBiliACGN/images/hands/bottle_rock.png";
    public override string CustomArmScissorsTexturePath => "res://BiliBiliACGN/images/hands/bottle_scissors.png";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "res://BiliBiliACGN/images/characters/bottle_select_icon.png";
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "res://BiliBiliACGN/images/characters/bottle_select_locked_icon.png";
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
    public override string CustomCharacterSelectTransitionPath => "res://BiliBiliACGN/materials/transitions/" + PlaceholderID + "_transition_mat.tres";

    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleStrike>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<PowerlessAngry>(),
        ModelDb.Card<IceBee>(),
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<Alzheimer>(),
    ];
}   