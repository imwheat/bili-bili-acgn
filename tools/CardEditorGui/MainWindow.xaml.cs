using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

public partial class MainWindow : Window
{
    private string? _currentPath;
    private bool _dirty;
    private bool _suppressDirty;
    /// <summary>最近一次从磁盘加载或成功保存后的卡牌数据，用于「保存并生成」时判断变更范围。</summary>
    private CardDefinition _persistedSnapshot = null!;
    private EditorSettings _settings = new();
    private readonly ObservableCollection<CardPoolEntry> _poolEntries = [];
    private readonly ObservableCollection<BuffOptionEntry> _buffOptions = [];
    private readonly ObservableCollection<KeywordOptionEntry> _keywordOptions = [];
    private readonly ObservableCollection<string> _canonicalKeywordFields = [];
    private readonly ObservableCollection<string> _extraHoverTipKeywordFields = [];
    private readonly ObservableCollection<DynamicVarEntry> _dynamicVars = [];
    private readonly ObservableCollection<UpgradeEffectEntry> _upgradeEffects = [];
    private readonly ObservableCollection<CardPlayAction> _playActions = [];

    /// <summary>CardPlayAction「数值来源」下拉：literal + 各 DynamicVar.kind。</summary>
    public ObservableCollection<ValueBindingOption> PlayActionValueBindingOptions { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        GridDynamicVars.ItemsSource = _dynamicVars;
        ColPlayActionType.ItemsSource = new[] { "DrawCards", "Damage", "Block", "Discard", "Exhaust", "Buff" };
        ColPlayActionBuffType.ItemsSource = _buffOptions;
        ColPlayActionBuffType.DisplayMemberPath = nameof(BuffOptionEntry.Name);
        ColPlayActionBuffType.SelectedValuePath = nameof(BuffOptionEntry.Name);
        // DataGridComboBoxColumn 的 ItemsSource 在 XAML 中绑定常无法解析，必须在代码里挂 ObservableCollection
        ColPlayActionValueBinding.ItemsSource = PlayActionValueBindingOptions;
        ColPlayActionRepeatBinding.ItemsSource = PlayActionValueBindingOptions;
        GridCardPlayActions.ItemsSource = _playActions;
        ColUpgradeEffectKind.ItemsSource = new[]
        {
            new ValueBindingOption { Key = "EnergyCostDelta", Display = "能耗变化 (UpgradeBy)" },
            new ValueBindingOption { Key = "AddKeyword", Display = "添加关键字" },
            new ValueBindingOption { Key = "RemoveKeyword", Display = "移除关键字" }
        };
        ColUpgradeEffectKeyword.ItemsSource = _keywordOptions;
        ColUpgradeEffectKeyword.DisplayMemberPath = nameof(KeywordOptionEntry.Name);
        ColUpgradeEffectKeyword.SelectedValuePath = nameof(KeywordOptionEntry.Name);
        GridUpgradeEffects.ItemsSource = _upgradeEffects;
        _upgradeEffects.CollectionChanged += UpgradeEffects_CollectionChanged;
        LstCanonicalKeywords.ItemsSource = _canonicalKeywordFields;
        LstExtraHoverTips.ItemsSource = _extraHoverTipKeywordFields;
        _canonicalKeywordFields.CollectionChanged += KeywordFieldLists_CollectionChanged;
        _extraHoverTipKeywordFields.CollectionChanged += KeywordFieldLists_CollectionChanged;
        _dynamicVars.CollectionChanged += DynamicVars_CollectionChanged;
        FillComboDefaults();
        LoadSettingsAndPools();
        NewDocument();
        HookFieldChanges();
        InitializeDescriptionBbCode();
        RebuildPlayActionValueBindingOptions();
        Closing += (_, e) =>
        {
            if (_dirty && !ConfirmDiscard())
                e.Cancel = true;
        };
    }

    private void LoadSettingsAndPools()
    {
        _settings = EditorSettingsJson.LoadOrCreateDefault();
        _poolEntries.Clear();
        foreach (var e in CardPoolJson.LoadOrCreateDefault())
            _poolEntries.Add(new CardPoolEntry { Name = e.Name, DisplayName = e.DisplayName });
        if (_poolEntries.Count == 0)
        {
            foreach (var e in CardPoolCatalog.CloneDefaultPools())
                _poolEntries.Add(e);
        }

        CmbPoolType.ItemsSource = null;
        CmbPoolType.ItemsSource = _poolEntries;
        CmbPoolType.DisplayMemberPath = nameof(CardPoolEntry.DisplayLabel);
        CmbPoolType.SelectedValuePath = nameof(CardPoolEntry.Name);

        _buffOptions.Clear();
        foreach (var b in BuffOptionsJson.LoadOrCreateDefault())
            _buffOptions.Add(new BuffOptionEntry { Name = b.Name, Notes = b.Notes });

        _keywordOptions.Clear();
        foreach (var k in CardKeywordOptionsJson.LoadOrCreateDefault())
            _keywordOptions.Add(new KeywordOptionEntry { Name = k.Name, Notes = k.Notes });
    }

    private string GetDialogInitialDirectory()
    {
        var dir = _settings.DefaultSaveDirectory?.Trim() ?? "";
        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            return dir;
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    private void UpgradeEffects_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressDirty)
            MarkDirty();
    }

    private void KeywordFieldLists_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (!_suppressDirty)
            MarkDirty();
    }

    private void DynamicVars_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildPlayActionValueBindingOptions();
        Dispatcher.BeginInvoke(RefreshDescriptionPreview, DispatcherPriority.Background);
    }

    /// <summary>
    /// 刷新「数值来源」下拉：固定数值 + 当前动态变量 kind（去重）。
    /// 须在 <see cref="PlayActionValueBindingOptions"/> 被 Clear 之前快照各行绑定，否则下拉清空瞬间会把 <see cref="CardPlayAction.ValueBinding"/> 写成空，随后被误判为固定数值。
    /// </summary>
    private void RebuildPlayActionValueBindingOptions()
    {
        var savedBindings = _playActions
            .Select(a => string.IsNullOrWhiteSpace(a.ValueBinding) ? "literal" : a.ValueBinding.Trim())
            .ToList();
        var savedRepeatBindings = _playActions
            .Select(a => string.IsNullOrWhiteSpace(a.RepeatCountBinding) ? "literal" : a.RepeatCountBinding.Trim())
            .ToList();

        PlayActionValueBindingOptions.Clear();
        PlayActionValueBindingOptions.Add(new ValueBindingOption { Key = "literal", Display = "固定数值" });
        foreach (var k in _dynamicVars
                     .Select(v => v.Kind?.Trim() ?? "")
                     .Where(s => s.Length > 0)
                     .Distinct(StringComparer.Ordinal))
        {
            PlayActionValueBindingOptions.Add(new ValueBindingOption { Key = k, Display = $"变量: {k}" });
        }

        var validKinds = _dynamicVars
            .Select(v => v.Kind?.Trim() ?? "")
            .Where(s => s.Length > 0)
            .ToHashSet(StringComparer.Ordinal);

        var changed = false;
        for (var i = 0; i < _playActions.Count; i++)
        {
            var saved = i < savedBindings.Count ? savedBindings[i] : "literal";
            var resolved = saved == "literal" || validKinds.Contains(saved) ? saved : "literal";
            if (!string.Equals(_playActions[i].ValueBinding, resolved, StringComparison.Ordinal))
            {
                _playActions[i].ValueBinding = resolved;
                changed = true;
            }

            var savedR = i < savedRepeatBindings.Count ? savedRepeatBindings[i] : "literal";
            var resolvedR = savedR == "literal" || validKinds.Contains(savedR) ? savedR : "literal";
            if (!string.Equals(_playActions[i].RepeatCountBinding, resolvedR, StringComparison.Ordinal))
            {
                _playActions[i].RepeatCountBinding = resolvedR;
                changed = true;
            }
        }

        GridCardPlayActions.Items.Refresh();
        if (changed && !_suppressDirty)
            MarkDirty();
    }

    private void FillComboDefaults()
    {
        CmbCardType.ItemsSource = new[]
        {
            "Attack", "Skill", "Power", "Status", "Curse", "Quest"
        };
        CmbRarity.ItemsSource = new[]
        {
            "Common", "Uncommon", "Rare", "Special", "Curse", "Quest"
        };
        CmbTargetType.ItemsSource = new[]
        {
            "None",
            "Self",
            "AnyEnemy",
            "AllEnemies",
            "RandomEnemy",
            "AnyPlayer",
            "AnyAlly",
            "AllAllies",
            "TargetedNoCreature",
            "Osty"
        };
    }

    private void NewDocument()
    {
        _currentPath = null;
        ApplyModelToUi(CreateDefaultCard());
        _dirty = false;
        UpdateTitle();
        StatusText.Text = "新建文档";
    }

    private CardDefinition CreateDefaultCard()
    {
        var pool = _poolEntries.Count > 0 ? _poolEntries[0].Name : "ColorlessCardPool";
        return new CardDefinition
        {
            ClassName = "NewCard",
            Title = "",
            Description = null,
            Namespace = _settings.DefaultNamespace,
            EnergyCost = 1,
            CardType = "Attack",
            Rarity = "Common",
            TargetType = "AnyEnemy",
            ShowInCardLibrary = true,
            PoolTypeName = pool,
            CanonicalKeywordFields = [],
            ExtraHoverTipKeywordFields = [],
            DynamicVars =
            [
                new DynamicVarEntry { Kind = "Damage", BaseValue = 6m, UpgradeValue = 0m }
            ],
            UpgradeEffects = [],
            CardPlayActions = [],
            Notes = ""
        };
    }

    private void ApplyModelToUi(CardDefinition m)
    {
        _suppressDirty = true;
        try
        {
            TxtClassName.Text = m.ClassName;
            TxtTitle.Text = m.Title ?? "";
            TxtDescription.Text = m.Description ?? "";
            TxtEnergyCost.Text = m.EnergyCost.ToString();
            EnsurePoolInOptions(m.PoolTypeName);
            SelectPoolCombo(m.PoolTypeName);
            SelectCombo(CmbCardType, m.CardType);
            SelectCombo(CmbRarity, m.Rarity);
            SelectCombo(CmbTargetType, NormalizeTargetTypeForUi(m.TargetType));
            ChkShowInLibrary.IsChecked = m.ShowInCardLibrary;
            TxtNotes.Text = m.Notes ?? "";

            _canonicalKeywordFields.Clear();
            foreach (var x in m.CanonicalKeywordFields ?? [])
            {
                var t = x?.Trim();
                if (!string.IsNullOrEmpty(t))
                    _canonicalKeywordFields.Add(t);
            }
            _extraHoverTipKeywordFields.Clear();
            foreach (var x in m.ExtraHoverTipKeywordFields ?? [])
            {
                var t = x?.Trim();
                if (!string.IsNullOrEmpty(t))
                    _extraHoverTipKeywordFields.Add(t);
            }

            _dynamicVars.Clear();
            foreach (var v in m.DynamicVars)
                _dynamicVars.Add(CloneVar(v));
            if (_dynamicVars.Count == 0)
                _dynamicVars.Add(new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, UpgradeValue = 0m });

            _playActions.Clear();
            if (m.CardPlayActions != null)
            {
                foreach (var a in m.CardPlayActions)
                    _playActions.Add(ClonePlayAction(a));
            }
            _upgradeEffects.Clear();
            if (m.UpgradeEffects != null)
            {
                foreach (var u in m.UpgradeEffects)
                    _upgradeEffects.Add(CloneUpgradeEffect(u));
            }
            RebuildPlayActionValueBindingOptions();
        }
        finally
        {
            _suppressDirty = false;
            RefreshPersistedSnapshot();
        }
    }

    private void RefreshPersistedSnapshot()
    {
        _persistedSnapshot = CardDefinitionModelComparer.DeepClone(CollectModelFromUi());
    }

    /// <summary>与游戏内 <c>TargetType</c> 对齐；旧 JSON 中的 Player 映射为 AnyPlayer。</summary>
    private static string NormalizeTargetTypeForUi(string? targetType)
    {
        if (string.IsNullOrWhiteSpace(targetType))
            return "AnyEnemy";
        var t = targetType.Trim();
        return t.Equals("Player", StringComparison.Ordinal) ? "AnyPlayer" : t;
    }

    private void EnsurePoolInOptions(string? poolTypeName)
    {
        var name = string.IsNullOrWhiteSpace(poolTypeName) ? null : poolTypeName.Trim();
        if (string.IsNullOrEmpty(name))
            return;
        if (_poolEntries.Any(e => string.Equals(e.Name, name, StringComparison.Ordinal)))
            return;
        _poolEntries.Add(new CardPoolEntry { Name = name, DisplayName = null });
    }

    private void SelectPoolCombo(string? poolTypeName)
    {
        if (string.IsNullOrWhiteSpace(poolTypeName))
        {
            CmbPoolType.SelectedIndex = _poolEntries.Count > 0 ? 0 : -1;
            return;
        }
        var n = poolTypeName.Trim();
        foreach (var e in _poolEntries)
        {
            if (string.Equals(e.Name, n, StringComparison.Ordinal))
            {
                CmbPoolType.SelectedItem = e;
                return;
            }
        }
        CmbPoolType.SelectedIndex = _poolEntries.Count > 0 ? 0 : -1;
    }

    private static DynamicVarEntry CloneVar(DynamicVarEntry v)
    {
        var vp = v.ValueProp;
        if (vp == ValueProp.None && DynamicVarEntry.IsDamageOrBlockKind(v.Kind))
            vp = ValueProp.Move;
        return new DynamicVarEntry
        {
            Kind = v.Kind,
            BaseValue = v.BaseValue,
            UpgradeValue = v.UpgradeValue,
            ValueProp = vp
        };
    }

    private static UpgradeEffectEntry CloneUpgradeEffect(UpgradeEffectEntry u) =>
        new()
        {
            Kind = string.IsNullOrWhiteSpace(u.Kind) ? "EnergyCostDelta" : u.Kind.Trim(),
            Delta = u.Delta,
            KeywordField = string.IsNullOrWhiteSpace(u.KeywordField) ? null : u.KeywordField.Trim(),
            Notes = u.Notes
        };

    private static CardPlayAction ClonePlayAction(CardPlayAction a)
    {
        var rbRaw = a.RepeatCountBinding;
        var rb = string.IsNullOrWhiteSpace(rbRaw) ? "literal" : rbRaw.Trim();
        var rv = a.RepeatCountValue;
        // 旧 JSON 未包含 repeat 字段：binding 为 null 且 value 为 0 → 视为默认执行 1 次
        if (rbRaw is null && rv == 0m)
            rv = 1m;

        return new CardPlayAction
        {
            ActionType = a.ActionType,
            BuffType = string.IsNullOrWhiteSpace(a.BuffType) ? null : a.BuffType.Trim(),
            ValueBinding = string.IsNullOrWhiteSpace(a.ValueBinding) ? "literal" : a.ValueBinding,
            Value = a.Value,
            RepeatCountBinding = rb,
            RepeatCountValue = rv,
            Notes = a.Notes
        };
    }

    private static void SelectCombo(System.Windows.Controls.ComboBox box, string value)
    {
        for (var i = 0; i < box.Items.Count; i++)
        {
            if (box.Items[i]?.ToString() == value)
            {
                box.SelectedIndex = i;
                return;
            }
        }
        box.SelectedIndex = box.Items.Count > 0 ? 0 : -1;
    }

    private string ResolvePoolTypeNameFromCombo()
    {
        if (CmbPoolType.SelectedItem is CardPoolEntry pe)
            return string.IsNullOrWhiteSpace(pe.Name) ? "ColorlessCardPool" : pe.Name.Trim();
        if (CmbPoolType.SelectedValue is string s && !string.IsNullOrWhiteSpace(s))
            return s.Trim();
        return _poolEntries.FirstOrDefault()?.Name ?? "ColorlessCardPool";
    }

    private CardDefinition CollectModelFromUi()
    {
        if (!int.TryParse(TxtEnergyCost.Text.Trim(), out var energy))
            energy = 0;

        var pool = ResolvePoolTypeNameFromCombo();

        return new CardDefinition
        {
            SchemaVersion = 1,
            ClassName = TxtClassName.Text.Trim(),
            Title = TxtTitle.Text.Trim(),
            Description = string.IsNullOrWhiteSpace(TxtDescription.Text) ? null : TxtDescription.Text,
            Namespace = _settings.DefaultNamespace,
            EnergyCost = energy,
            CardType = CmbCardType.SelectedItem?.ToString() ?? "Attack",
            Rarity = CmbRarity.SelectedItem?.ToString() ?? "Common",
            TargetType = CmbTargetType.SelectedItem?.ToString() ?? "AnyEnemy",
            ShowInCardLibrary = ChkShowInLibrary.IsChecked == true,
            PoolTypeName = pool,
            CanonicalKeywordFields = _canonicalKeywordFields.ToList(),
            ExtraHoverTipKeywordFields = _extraHoverTipKeywordFields.ToList(),
            DynamicVars = _dynamicVars.Select(CloneVar).ToList(),
            UpgradeEffects = _upgradeEffects.Select(CloneUpgradeEffect).ToList(),
            CardPlayActions = _playActions.Select(ClonePlayAction).ToList(),
            Notes = string.IsNullOrWhiteSpace(TxtNotes.Text) ? null : TxtNotes.Text
        };
    }

    private void MarkDirty()
    {
        if (_suppressDirty)
            return;
        _dirty = true;
        UpdateTitle();
    }

    private void UpdateTitle()
    {
        var name = string.IsNullOrEmpty(_currentPath)
            ? "未保存"
            : Path.GetFileName(_currentPath);
        Title = _dirty ? $"卡牌定义编辑器 — {name} *" : $"卡牌定义编辑器 — {name}";
    }

    private bool ConfirmDiscard()
    {
        if (!_dirty)
            return true;
        var r = System.Windows.MessageBox.Show("当前内容未保存，是否放弃更改？", "确认",
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        return r == MessageBoxResult.Yes;
    }

    private void MenuNew_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard())
            return;
        NewDocument();
    }

    private void MenuOpen_Click(object sender, RoutedEventArgs e)
    {
        if (!ConfirmDiscard())
            return;
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "打开卡牌定义",
            InitialDirectory = GetDialogInitialDirectory()
        };
        if (dlg.ShowDialog() != true)
            return;
        try
        {
            var model = CardDefinitionJson.LoadFromFile(dlg.FileName);
            _currentPath = dlg.FileName;
            ApplyModelToUi(model);
            _dirty = false;
            UpdateTitle();
            StatusText.Text = $"已打开: {_currentPath}";
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "打开失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuSave_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrEmpty(_currentPath))
        {
            MenuSaveAs_Click(sender, e);
            return;
        }
        SaveToPath(_currentPath);
    }

    private void MenuSaveAs_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "保存卡牌定义",
            InitialDirectory = GetDialogInitialDirectory(),
            FileName = string.IsNullOrWhiteSpace(TxtClassName.Text) ? "card.json" : $"{TxtClassName.Text.Trim()}.json"
        };
        if (dlg.ShowDialog() != true)
            return;
        _currentPath = dlg.FileName;
        SaveToPath(_currentPath);
    }

    private void MenuSaveAndGenerate_Click(object sender, RoutedEventArgs e)
    {
        var missing = new List<string>();
        if (string.IsNullOrWhiteSpace(TxtClassName.Text))
            missing.Add("类名（Class name）");

        if (missing.Count > 0)
        {
            ShowMissingRequiredFields("保存并生成", missing);
            return;
        }

        if (!EnsureJsonSavedPath())
            return;

        _settings = EditorSettingsJson.LoadOrCreateDefault();
        var current = CollectModelFromUi();
        var snap = _persistedSnapshot;

        var outDir = _settings.DefaultCardScriptOutputDirectory?.Trim() ?? "";
        var sanitizedClass = CardCodeGenerator.SanitizeClassName(current.ClassName.Trim());
        var expectedCsPath = string.IsNullOrEmpty(outDir)
            ? null
            : Path.Combine(Path.GetFullPath(outDir), $"{sanitizedClass}.cs");
        var csMissing = expectedCsPath != null && !File.Exists(expectedCsPath);

        var onlyNotes = CardDefinitionModelComparer.OnlyTopLevelNotesChanged(current, snap);
        var locDirty = !CardDefinitionModelComparer.LocalizationSliceEquals(current, snap);
        var codeDirty = !CardDefinitionModelComparer.CodeSliceEquals(current, snap);
        var needCsGenerate = codeDirty || csMissing;

        if (onlyNotes && !csMissing)
        {
            try
            {
                CardDefinitionJson.SaveToFile(current, _currentPath!);
                _dirty = false;
                UpdateTitle();
                RefreshPersistedSnapshot();
                StatusText.Text = "已保存卡牌定义（仅备注变更）";
                System.Windows.MessageBox.Show(
                    "已保存卡牌定义 JSON。\n\n当前仅有「备注」变更，未更新 cards.json 与 C# 脚本。",
                    "保存并生成",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return;
        }

        var locPath = _settings.CardLocalizationJsonPath?.Trim() ?? "";

        missing.Clear();
        if (locDirty && string.IsNullOrWhiteSpace(current.Title))
            missing.Add("卡牌名称（标题），用于写入 cards.json");
        if (locDirty && string.IsNullOrEmpty(locPath))
            missing.Add("「工具 → 设置」中的「卡牌信息 JSON 路径」（名称或描述相对上次保存有变更）");
        if (needCsGenerate && string.IsNullOrEmpty(outDir))
            missing.Add("「工具 → 设置」中的「默认卡牌脚本（.cs）生成目录」（需生成或更新 C#，或目标 .cs 尚不存在）");

        if (missing.Count > 0)
        {
            ShowMissingRequiredFields("保存并生成", missing);
            return;
        }

        try
        {
            CardDefinitionJson.SaveToFile(current, _currentPath!);
            _dirty = false;
            UpdateTitle();

            var lines = new List<string> { "已保存卡牌定义 JSON。" };

            if (locDirty)
            {
                CardLocalizationJsonMerger.MergeTitleAndDescription(locPath, current.ClassName, current.Namespace, current.Title, current.Description);
                lines.Add($"已更新卡牌信息：{Path.GetFullPath(locPath)}");
            }

            string? csPath = null;
            if (needCsGenerate)
                csPath = CardCodeGenerator.WriteGeneratedFile(current, outDir);

            if (csPath != null)
                lines.Add($"已生成 C#：{csPath}");
            else if (!locDirty)
                lines.Add("脚本与本地化均无变更（已仅写入 JSON）。");

            RefreshPersistedSnapshot();
            var msg = string.Join("\n", lines);
            StatusText.Text = msg.Replace('\n', ' ');
            System.Windows.MessageBox.Show(msg, "保存并生成", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "保存并生成失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    /// <summary>若尚未有 JSON 路径，则弹出另存为；取消则返回 false。</summary>
    private bool EnsureJsonSavedPath()
    {
        if (!string.IsNullOrEmpty(_currentPath))
            return true;
        var dlg = new Microsoft.Win32.SaveFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "保存卡牌定义（生成前需保存 JSON）",
            InitialDirectory = GetDialogInitialDirectory(),
            FileName = string.IsNullOrWhiteSpace(TxtClassName.Text) ? "card.json" : $"{TxtClassName.Text.Trim()}.json"
        };
        if (dlg.ShowDialog() != true)
            return false;
        _currentPath = dlg.FileName;
        return true;
    }

    private void MenuSettings_Click(object sender, RoutedEventArgs e)
    {
        var poolBefore = ResolvePoolTypeNameFromCombo();
        var w = new SettingsWindow { Owner = this };
        if (w.ShowDialog() != true)
            return;

        LoadSettingsAndPools();
        EnsurePoolInOptions(poolBefore);
        SelectPoolCombo(poolBefore);
        StatusText.Text = "已更新设置";
    }

    private void MenuPowerTextEditor_Click(object sender, RoutedEventArgs e)
    {
        new PowerLocalizationEditorWindow { Owner = this }.ShowDialog();
    }

    private void MenuKeywordTextEditor_Click(object sender, RoutedEventArgs e)
    {
        new KeywordLocalizationEditorWindow { Owner = this }.ShowDialog();
    }

    private void BtnManageTemplates_Click(object sender, RoutedEventArgs e)
    {
        var w = new DynamicVarTemplatesWindow(this);
        if (w.ShowDialog() == true)
            StatusText.Text = $"模版已保存: {DynamicVarTemplateJson.GetDefaultFilePath()}";
    }

    private void BtnManagePlayActionTemplates_Click(object sender, RoutedEventArgs e)
    {
        var w = new CardPlayActionTemplatesWindow(this);
        if (w.ShowDialog() == true)
            StatusText.Text = $"打出效果模版已保存: {CardPlayActionTemplateJson.GetDefaultFilePath()}";
    }

    /// <summary>
    /// 按模版名称追加一行 <see cref="CardPlayAction"/>：ActionType 与名称一致，value / 重复次数固定为 1，其余默认。
    /// 若已存在<strong>相同模版默认内容</strong>的一行（同 ActionType 且 literal/1/1、无 notes），返回 false。
    /// </summary>
    public bool TryAddCardPlayActionFromTemplate(string actionType)
    {
        var t = actionType.Trim();
        if (t.Length == 0)
            return false;
        if (_playActions.Any(a => IsSameAsTemplateDefaultRow(a, t)))
            return false;
        _playActions.Add(new CardPlayAction
        {
            ActionType = t,
            BuffType = null,
            ValueBinding = "literal",
            Value = 1m,
            RepeatCountBinding = "literal",
            RepeatCountValue = 1m,
            Notes = null
        });
        MarkDirty();
        StatusText.Text = $"已添加打出效果: {t}";
        return true;
    }

    /// <summary>与「从模版添加」生成的默认行一致（用于判断是否重复）。</summary>
    private static bool IsSameAsTemplateDefaultRow(CardPlayAction a, string templateActionType)
    {
        if (!string.Equals(a.ActionType?.Trim(), templateActionType, StringComparison.Ordinal))
            return false;
        if (!string.Equals(a.ValueBinding?.Trim(), "literal", StringComparison.Ordinal))
            return false;
        if (a.Value != 1m)
            return false;
        if (!string.Equals(a.RepeatCountBinding?.Trim(), "literal", StringComparison.Ordinal))
            return false;
        if (a.RepeatCountValue != 1m)
            return false;
        if (!string.IsNullOrWhiteSpace(a.Notes))
            return false;
        return true;
    }

    /// <summary>按模版 name 向当前卡牌追加一行；已存在同 kind 返回 false。</summary>
    public bool TryAddDynamicVarFromTemplateKind(string kind)
    {
        var k = kind.Trim();
        if (k.Length == 0)
            return false;
        if (_dynamicVars.Any(v => string.Equals(v.Kind?.Trim(), k, StringComparison.Ordinal)))
            return false;
        _dynamicVars.Add(new DynamicVarEntry
        {
            Kind = k,
            BaseValue = 0m,
            UpgradeValue = 0m,
            ValueProp = DynamicVarEntry.DefaultValuePropForKind(k)
        });
        MarkDirty();
        StatusText.Text = $"已添加动态变量: {k}";
        return true;
    }

    /// <summary>在必填项未填时弹出提示，列出具体缺项（每条一行，前缀「•」）。</summary>
    private static void ShowMissingRequiredFields(string dialogTitle, IReadOnlyList<string> items)
    {
        if (items.Count == 0)
            return;
        var body = string.Join("\n", items.Select(line => "• " + line));
        System.Windows.MessageBox.Show(
            "请补全以下必填项：\n\n" + body,
            dialogTitle,
            MessageBoxButton.OK,
            MessageBoxImage.Warning);
    }

    private void SaveToPath(string path)
    {
        try
        {
            var model = CollectModelFromUi();
            if (string.IsNullOrWhiteSpace(model.ClassName))
            {
                ShowMissingRequiredFields("保存", ["类名（Class name）"]);
                return;
            }

            var locUnchanged = CardDefinitionModelComparer.LocalizationSliceEquals(model, _persistedSnapshot);
            CardDefinitionJson.SaveToFile(model, path);
            _dirty = false;
            UpdateTitle();

            var settings = EditorSettingsJson.LoadOrCreateDefault();
            var locPath = settings.CardLocalizationJsonPath?.Trim() ?? "";
            if (!locUnchanged && !string.IsNullOrEmpty(locPath))
                CardLocalizationJsonMerger.MergeTitleAndDescription(locPath, model.ClassName, model.Namespace, model.Title, model.Description);

            RefreshPersistedSnapshot();
            StatusText.Text = $"已保存: {path}";
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void MenuExit_Click(object sender, RoutedEventArgs e) => Close();

    private void BtnAddVar_Click(object sender, RoutedEventArgs e)
    {
        _dynamicVars.Add(new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, UpgradeValue = 0m });
        MarkDirty();
    }

    private void BtnRemoveVar_Click(object sender, RoutedEventArgs e)
    {
        if (GridDynamicVars.SelectedItem is not DynamicVarEntry row)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _dynamicVars.Remove(row);
        if (_dynamicVars.Count == 0)
            _dynamicVars.Add(new DynamicVarEntry { Kind = "Damage", BaseValue = 1m, UpgradeValue = 0m });
        MarkDirty();
    }

    private void BtnAddPlayAction_Click(object sender, RoutedEventArgs e)
    {
        _playActions.Add(new CardPlayAction
        {
            ActionType = "DrawCards",
            BuffType = null,
            ValueBinding = "literal",
            Value = 1m,
            RepeatCountBinding = "literal",
            RepeatCountValue = 1m
        });
        MarkDirty();
    }

    private void BtnMovePlayActionUp_Click(object sender, RoutedEventArgs e)
    {
        if (GridCardPlayActions.SelectedItem is not CardPlayAction row)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var i = _playActions.IndexOf(row);
        if (i <= 0)
            return;
        _playActions.Move(i, i - 1);
        GridCardPlayActions.SelectedItem = row;
        MarkDirty();
    }

    private void BtnMovePlayActionDown_Click(object sender, RoutedEventArgs e)
    {
        if (GridCardPlayActions.SelectedItem is not CardPlayAction row)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var i = _playActions.IndexOf(row);
        if (i < 0 || i >= _playActions.Count - 1)
            return;
        _playActions.Move(i, i + 1);
        GridCardPlayActions.SelectedItem = row;
        MarkDirty();
    }

    private void BtnBuffOptions_Click(object sender, RoutedEventArgs e)
    {
        var w = new BuffOptionsWindow { Owner = this };
        if (w.ShowDialog() != true)
            return;

        // 仅刷新 BUFF 选项，不影响其他设置。
        _buffOptions.Clear();
        foreach (var b in BuffOptionsJson.LoadOrCreateDefault())
            _buffOptions.Add(new BuffOptionEntry { Name = b.Name, Notes = b.Notes });
        GridCardPlayActions.Items.Refresh();
        StatusText.Text = $"BUFF 选项已保存: {BuffOptionsJson.GetDefaultFilePath()}";
    }

    private void BtnAddUpgradeEffect_Click(object sender, RoutedEventArgs e)
    {
        var kw = _keywordOptions.Count > 0 ? _keywordOptions[0].Name : null;
        _upgradeEffects.Add(new UpgradeEffectEntry
        {
            Kind = "EnergyCostDelta",
            Delta = -1,
            KeywordField = kw,
            Notes = null
        });
        GridUpgradeEffects.SelectedItem = _upgradeEffects[^1];
        MarkDirty();
    }

    private void BtnRemoveUpgradeEffect_Click(object sender, RoutedEventArgs e)
    {
        if (GridUpgradeEffects.SelectedItem is not UpgradeEffectEntry row)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _upgradeEffects.Remove(row);
        MarkDirty();
    }

    private void BtnKeywordOptions_Click(object sender, RoutedEventArgs e)
    {
        var w = new CardKeywordOptionsWindow { Owner = this };
        if (w.ShowDialog() != true)
            return;
        _keywordOptions.Clear();
        foreach (var k in CardKeywordOptionsJson.LoadOrCreateDefault())
            _keywordOptions.Add(new KeywordOptionEntry { Name = k.Name, Notes = k.Notes });
        GridUpgradeEffects.Items.Refresh();
        StatusText.Text = $"关键字选项已保存: {CardKeywordOptionsJson.GetDefaultFilePath()}";
    }

    private void BtnKeywordCanonicalAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new KeywordPickerDialog(_keywordOptions, _canonicalKeywordFields.ToList()) { Owner = this };
        if (dlg.ShowDialog() != true || string.IsNullOrEmpty(dlg.SelectedName))
            return;
        _canonicalKeywordFields.Add(dlg.SelectedName);
        StatusText.Text = $"已添加 CanonicalKeyword: {dlg.SelectedName}";
    }

    private void BtnKeywordCanonicalRemove_Click(object sender, RoutedEventArgs e)
    {
        if (LstCanonicalKeywords.SelectedItem is not string s)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _canonicalKeywordFields.Remove(s);
    }

    private void BtnKeywordCanonicalUp_Click(object sender, RoutedEventArgs e) =>
        MoveKeywordListItem(_canonicalKeywordFields, LstCanonicalKeywords, -1);

    private void BtnKeywordCanonicalDown_Click(object sender, RoutedEventArgs e) =>
        MoveKeywordListItem(_canonicalKeywordFields, LstCanonicalKeywords, 1);

    private void BtnKeywordHoverAdd_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new KeywordPickerDialog(_keywordOptions, _extraHoverTipKeywordFields.ToList()) { Owner = this };
        if (dlg.ShowDialog() != true || string.IsNullOrEmpty(dlg.SelectedName))
            return;
        _extraHoverTipKeywordFields.Add(dlg.SelectedName);
        StatusText.Text = $"已添加 ExtraHoverTip: {dlg.SelectedName}";
    }

    private void BtnKeywordHoverRemove_Click(object sender, RoutedEventArgs e)
    {
        if (LstExtraHoverTips.SelectedItem is not string s)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _extraHoverTipKeywordFields.Remove(s);
    }

    private void BtnKeywordHoverUp_Click(object sender, RoutedEventArgs e) =>
        MoveKeywordListItem(_extraHoverTipKeywordFields, LstExtraHoverTips, -1);

    private void BtnKeywordHoverDown_Click(object sender, RoutedEventArgs e) =>
        MoveKeywordListItem(_extraHoverTipKeywordFields, LstExtraHoverTips, 1);

    private static void MoveKeywordListItem(ObservableCollection<string> list, System.Windows.Controls.ListBox lb, int delta)
    {
        if (lb.SelectedItem is not string s)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var i = list.IndexOf(s);
        if (i < 0)
            return;
        var ni = i + delta;
        if (ni < 0 || ni >= list.Count)
            return;
        list.Move(i, ni);
        lb.SelectedItem = list[ni];
    }

    private void GridUpgradeEffects_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) => MarkDirty();

    private void GridCardPlayActions_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (e.Row.Item is not CardPlayAction action)
            return;
        if (e.Column is not DataGridTextColumn col)
            return;
        var header = col.Header?.ToString();
        if (header == "value" && action.ValueBinding != "literal")
            e.Cancel = true;
        if (header == "重复次数" && action.RepeatCountBinding != "literal")
            e.Cancel = true;
    }

    private void BtnRemovePlayAction_Click(object sender, RoutedEventArgs e)
    {
        if (GridCardPlayActions.SelectedItem is not CardPlayAction row)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _playActions.Remove(row);
        MarkDirty();
    }

    protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
    {
        if (e.Key == Key.S && Keyboard.Modifiers == ModifierKeys.Control)
        {
            MenuSave_Click(this, e);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.N && Keyboard.Modifiers == ModifierKeys.Control)
        {
            MenuNew_Click(this, e);
            e.Handled = true;
            return;
        }
        if (e.Key == Key.O && Keyboard.Modifiers == ModifierKeys.Control)
        {
            MenuOpen_Click(this, e);
            e.Handled = true;
            return;
        }
        base.OnKeyDown(e);
    }

    private void HookFieldChanges()
    {
        TxtClassName.TextChanged += (_, _) => MarkDirty();
        TxtTitle.TextChanged += (_, _) => MarkDirty();
        TxtDescription.TextChanged += (_, _) => MarkDirty();
        TxtEnergyCost.TextChanged += (_, _) => MarkDirty();
        TxtNotes.TextChanged += (_, _) => MarkDirty();
        CmbPoolType.SelectionChanged += (_, _) => MarkDirty();
        CmbCardType.SelectionChanged += (_, _) => MarkDirty();
        CmbRarity.SelectionChanged += (_, _) => MarkDirty();
        CmbTargetType.SelectionChanged += (_, _) => MarkDirty();
        ChkShowInLibrary.Checked += (_, _) => MarkDirty();
        ChkShowInLibrary.Unchecked += (_, _) => MarkDirty();
        // CellEditEnding 在绑定写回模型之前触发，需延后一帧再刷新「数值来源」，否则会慢一步。
        GridDynamicVars.CellEditEnding += (_, e) =>
        {
            if (e.EditAction == DataGridEditAction.Cancel)
                return;
            var rowItem = e.Row?.Item as DynamicVarEntry;
            Dispatcher.BeginInvoke(() =>
            {
                RebuildPlayActionValueBindingOptions();
                MarkDirty();
                RefreshDescriptionPreview();
            }, DispatcherPriority.Background);
        };
        GridCardPlayActions.CellEditEnding += (_, _) => MarkDirty();
    }
}
