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

public sealed class BottleCharacter : CustomCharacterModel
{
    public override Color NameColor => new Color("789ccd");

    public override CharacterGender Gender => CharacterGender.Masculine;

    public override int StartingHp => 80;

    public override CardPoolModel CardPool => ModelDb.CardPool<BottleCardPool>();

    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BottleRelicPool>();

    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BottlePotionPool>();
    // 人物模型tscn路径。要自定义见下。
    public override string CustomVisualPath => "res://BiliBiliACGN/scenes/bottle_chracter.tscn";
    // 人物头像路径。
    public override string CustomIconTexturePath => "res://BiliBiliACGN/images/characters/bottle_icon.png";
    // 能量表盘tscn路径。要自定义见下。
    public override string CustomEnergyCounterPath => "res://BiliBiliACGN/scenes/bottle_energy_counter.tscn";
    // 人物选择背景。
    public override string CustomCharacterSelectBg => "res://BiliBiliACGN/scenes/bottle_select_bg.tscn";
    // 人物选择图标。
    public override string CustomCharacterSelectIconPath => "res://BiliBiliACGN/images/characters/bottle_select_icon.png";
    // 人物选择图标-锁定状态。
    public override string CustomCharacterSelectLockedIconPath => "res://BiliBiliACGN/images/characters/bottle_select_locked_icon.png";
    // 过渡音效。这个不能删。
    public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";

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
        ModelDb.Card<BottleBlock>(),
        ModelDb.Card<AngryStrike>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [
        ModelDb.Relic<Alzheimer>()
    ];

    // 攻击建筑师的攻击特效列表
    public override List<string> GetArchitectAttackVfx() => [
        "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
    ];
}   