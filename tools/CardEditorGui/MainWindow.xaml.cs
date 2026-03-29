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
    private readonly ObservableCollection<string> _poolOptions = [];
    private readonly ObservableCollection<DynamicVarEntry> _dynamicVars = [];
    private readonly ObservableCollection<CardPlayAction> _playActions = [];

    /// <summary>CardPlayAction「数值来源」下拉：literal + 各 DynamicVar.kind。</summary>
    public ObservableCollection<ValueBindingOption> PlayActionValueBindingOptions { get; } = [];

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        GridDynamicVars.ItemsSource = _dynamicVars;
        ColPlayActionType.ItemsSource = new[] { "DrawCards", "Damage", "Discard", "Exhaust" };
        // DataGridComboBoxColumn 的 ItemsSource 在 XAML 中绑定常无法解析，必须在代码里挂 ObservableCollection
        ColPlayActionValueBinding.ItemsSource = PlayActionValueBindingOptions;
        GridCardPlayActions.ItemsSource = _playActions;
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
        _poolOptions.Clear();
        foreach (var p in _settings.PoolTypeOptions)
        {
            if (!string.IsNullOrWhiteSpace(p))
                _poolOptions.Add(p.Trim());
        }
        if (_poolOptions.Count == 0)
            _poolOptions.Add("ColorlessCardPool");

        CmbPoolType.ItemsSource = _poolOptions;
    }

    private string GetDialogInitialDirectory()
    {
        var dir = _settings.DefaultSaveDirectory?.Trim() ?? "";
        if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
            return dir;
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
        var pool = _poolOptions.Count > 0 ? _poolOptions[0] : "ColorlessCardPool";
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
            DynamicVars =
            [
                new DynamicVarEntry { Kind = "Damage", BaseValue = 6m, UpgradeValue = 0m }
            ],
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
            SelectCombo(CmbPoolType, m.PoolTypeName);
            SelectCombo(CmbCardType, m.CardType);
            SelectCombo(CmbRarity, m.Rarity);
            SelectCombo(CmbTargetType, NormalizeTargetTypeForUi(m.TargetType));
            ChkShowInLibrary.IsChecked = m.ShowInCardLibrary;
            TxtNotes.Text = m.Notes ?? "";

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
        if (_poolOptions.Contains(name, StringComparer.Ordinal))
            return;
        _poolOptions.Add(name);
    }

    private static DynamicVarEntry CloneVar(DynamicVarEntry v) => new()
    {
        Kind = v.Kind,
        BaseValue = v.BaseValue,
        UpgradeValue = v.UpgradeValue
    };

    private static CardPlayAction ClonePlayAction(CardPlayAction a) => new()
    {
        ActionType = a.ActionType,
        ValueBinding = string.IsNullOrWhiteSpace(a.ValueBinding) ? "literal" : a.ValueBinding,
        Value = a.Value,
        Notes = a.Notes
    };

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

    private CardDefinition CollectModelFromUi()
    {
        if (!int.TryParse(TxtEnergyCost.Text.Trim(), out var energy))
            energy = 0;

        var pool = CmbPoolType.SelectedItem?.ToString()?.Trim();
        if (string.IsNullOrEmpty(pool))
            pool = _poolOptions.Count > 0 ? _poolOptions[0] : "ColorlessCardPool";

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
            DynamicVars = _dynamicVars.Select(CloneVar).ToList(),
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
        if (string.IsNullOrWhiteSpace(TxtClassName.Text))
        {
            System.Windows.MessageBox.Show("请先填写类名。", "保存并生成", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!EnsureJsonSavedPath())
            return;

        _settings = EditorSettingsJson.LoadOrCreateDefault();
        var current = CollectModelFromUi();
        var snap = _persistedSnapshot;

        var onlyNotes = CardDefinitionModelComparer.OnlyTopLevelNotesChanged(current, snap);
        var locDirty = !CardDefinitionModelComparer.LocalizationSliceEquals(current, snap);
        var codeDirty = !CardDefinitionModelComparer.CodeSliceEquals(current, snap);

        if (onlyNotes)
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
        var outDir = _settings.DefaultCardScriptOutputDirectory?.Trim() ?? "";

        if (locDirty && string.IsNullOrEmpty(locPath))
        {
            System.Windows.MessageBox.Show(
                "卡牌名称或描述已变更，请在「工具 → 设置」中配置「卡牌信息 JSON 路径」（如 localization/zhs/cards.json）。",
                "保存并生成",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            return;
        }

        if (codeDirty && string.IsNullOrEmpty(outDir))
        {
            System.Windows.MessageBox.Show(
                "脚本相关字段已变更，请先在「工具 → 设置」中配置「默认卡牌脚本（.cs）生成目录」。",
                "保存并生成",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
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
                CardLocalizationJsonMerger.MergeTitleAndDescription(locPath, current.ClassName, current.Title, current.Description);
                lines.Add($"已更新卡牌信息：{Path.GetFullPath(locPath)}");
            }

            string? csPath = null;
            if (codeDirty)
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
        var poolBefore = CmbPoolType.SelectedItem?.ToString();
        var w = new SettingsWindow { Owner = this };
        if (w.ShowDialog() != true)
            return;

        LoadSettingsAndPools();
        if (!string.IsNullOrEmpty(poolBefore))
            EnsurePoolInOptions(poolBefore);
        SelectCombo(CmbPoolType, poolBefore ?? "");
        StatusText.Text = "已更新设置";
    }

    private void BtnManageTemplates_Click(object sender, RoutedEventArgs e)
    {
        var w = new DynamicVarTemplatesWindow(this);
        if (w.ShowDialog() == true)
            StatusText.Text = $"模版已保存: {DynamicVarTemplateJson.GetDefaultFilePath()}";
    }

    /// <summary>按模版 name 向当前卡牌追加一行；已存在同 kind 返回 false。</summary>
    public bool TryAddDynamicVarFromTemplateKind(string kind)
    {
        var k = kind.Trim();
        if (k.Length == 0)
            return false;
        if (_dynamicVars.Any(v => string.Equals(v.Kind?.Trim(), k, StringComparison.Ordinal)))
            return false;
        _dynamicVars.Add(new DynamicVarEntry { Kind = k, BaseValue = 0m, UpgradeValue = 0m });
        MarkDirty();
        StatusText.Text = $"已添加动态变量: {k}";
        return true;
    }

    private void SaveToPath(string path)
    {
        try
        {
            var model = CollectModelFromUi();
            CardDefinitionJson.SaveToFile(model, path);
            _dirty = false;
            UpdateTitle();
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
        _playActions.Add(new CardPlayAction { ActionType = "DrawCards", ValueBinding = "literal", Value = 1m });
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

    private void GridCardPlayActions_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (e.Row.Item is not CardPlayAction action)
            return;
        if (e.Column is not DataGridTextColumn col)
            return;
        if (col.Header?.ToString() != "value")
            return;
        if (action.ValueBinding != "literal")
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
