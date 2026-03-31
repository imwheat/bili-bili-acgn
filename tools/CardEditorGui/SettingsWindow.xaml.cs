using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

public partial class SettingsWindow : Window
{
    private readonly ObservableCollection<CardPoolEntry> _poolRows = [];

    public SettingsWindow()
    {
        InitializeComponent();
        GridPools.ItemsSource = _poolRows;
        TxtSettingsPath.Text = EditorSettingsJson.GetDefaultSettingsPath();
        TxtCardPoolPath.Text = "exe 合并配置: " + ExeBundledSettingsJson.GetDefaultFilePath();
        Loaded += (_, _) => LoadIntoUi(EditorSettingsJson.LoadOrCreateDefault());
    }

    private void LoadIntoUi(EditorSettings s)
    {
        TxtDefaultNamespace.Text = s.DefaultNamespace;
        TxtDefaultSaveDirectory.Text = s.DefaultSaveDirectory ?? "";
        TxtDefaultCardScriptOutputDirectory.Text = s.DefaultCardScriptOutputDirectory ?? "";
        TxtCardLocalizationJsonPath.Text = s.CardLocalizationJsonPath ?? "";
        _poolRows.Clear();
        foreach (var e in CardPoolJson.LoadOrCreateDefault())
            _poolRows.Add(new CardPoolEntry { Name = e.Name, DisplayName = e.DisplayName });
        if (_poolRows.Count == 0)
        {
            foreach (var e in CardPoolCatalog.CloneDefaultPools())
                _poolRows.Add(e);
        }
    }

    private EditorSettings CollectFromUi()
    {
        return new EditorSettings
        {
            SchemaVersion = 1,
            DefaultNamespace = TxtDefaultNamespace.Text.Trim(),
            DefaultSaveDirectory = TxtDefaultSaveDirectory.Text.Trim(),
            DefaultCardScriptOutputDirectory = TxtDefaultCardScriptOutputDirectory.Text.Trim(),
            CardLocalizationJsonPath = TxtCardLocalizationJsonPath.Text.Trim()
        };
    }

    private void BtnBrowseSaveDir_Click(object sender, RoutedEventArgs e)
    {
        using var dlg = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "选择卡牌 JSON 的默认保存目录",
            UseDescriptionForTitle = true
        };
        var cur = TxtDefaultSaveDirectory.Text.Trim();
        if (Directory.Exists(cur))
            dlg.SelectedPath = cur;

        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            TxtDefaultSaveDirectory.Text = dlg.SelectedPath;
    }

    private void BtnBrowseScriptDir_Click(object sender, RoutedEventArgs e)
    {
        using var dlg = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "选择生成卡牌 .cs 脚本的默认输出目录",
            UseDescriptionForTitle = true
        };
        var cur = TxtDefaultCardScriptOutputDirectory.Text.Trim();
        if (Directory.Exists(cur))
            dlg.SelectedPath = cur;

        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            TxtDefaultCardScriptOutputDirectory.Text = dlg.SelectedPath;
    }

    private void BtnBrowseCardLocalizationJson_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "选择或指定卡牌信息 JSON 文件（如 cards.json）",
            CheckFileExists = false
        };
        var cur = TxtCardLocalizationJsonPath.Text.Trim();
        if (!string.IsNullOrEmpty(cur))
        {
            try
            {
                var full = Path.GetFullPath(cur);
                if (File.Exists(full))
                {
                    dlg.FileName = Path.GetFileName(full);
                    var dir = Path.GetDirectoryName(full);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                        dlg.InitialDirectory = dir;
                }
                else
                {
                    var dir = Path.GetDirectoryName(full);
                    if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                        dlg.InitialDirectory = dir;
                    var name = Path.GetFileName(full);
                    if (!string.IsNullOrEmpty(name))
                        dlg.FileName = name;
                }
            }
            catch
            {
                // ignore invalid path
            }
        }
        else
            dlg.FileName = "cards.json";

        if (dlg.ShowDialog() == true)
            TxtCardLocalizationJsonPath.Text = dlg.FileName;
    }


    private void BtnAddPool_Click(object sender, RoutedEventArgs e)
    {
        var name = TxtNewPoolType.Text.Trim();
        if (string.IsNullOrEmpty(name))
            return;
        if (_poolRows.Any(x => string.Equals(x.Name?.Trim(), name, StringComparison.Ordinal)))
        {
            System.Windows.MessageBox.Show("列表中已有该项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var disp = TxtNewPoolDisplayName.Text.Trim();
        _poolRows.Add(new CardPoolEntry { Name = name, DisplayName = string.IsNullOrEmpty(disp) ? null : disp });
        TxtNewPoolType.Clear();
        TxtNewPoolDisplayName.Clear();
        GridPools.SelectedItem = _poolRows[^1];
    }

    private void BtnRemovePool_Click(object sender, RoutedEventArgs e)
    {
        if (GridPools.SelectedItem is not CardPoolEntry row)
        {
            System.Windows.MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _poolRows.Remove(row);
        if (_poolRows.Count == 0)
            _poolRows.Add(new CardPoolEntry { Name = "ColorlessCardPool", DisplayName = "无色牌" });
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var settings = CollectFromUi();
            var missing = new List<string>();
            if (string.IsNullOrEmpty(settings.DefaultNamespace))
                missing.Add("默认命名空间");
            if (missing.Count > 0)
            {
                var body = string.Join("\n", missing.Select(line => "• " + line));
                System.Windows.MessageBox.Show(
                    "请补全以下必填项：\n\n" + body,
                    "设置",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var names = new HashSet<string>(StringComparer.Ordinal);
            foreach (var r in _poolRows)
            {
                var n = r.Name?.Trim() ?? "";
                if (n.Length == 0)
                {
                    System.Windows.MessageBox.Show("卡池「类型名」不能为空。", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (!names.Add(n))
                {
                    System.Windows.MessageBox.Show($"重复的类型名：{n}", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            var dir = settings.DefaultSaveDirectory;
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                var r = System.Windows.MessageBox.Show(
                    $"目录不存在：\n{dir}\n\n是否创建？",
                    "默认保存目录",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                    Directory.CreateDirectory(dir);
                else
                    return;
            }

            var scriptDir = settings.DefaultCardScriptOutputDirectory;
            if (!string.IsNullOrEmpty(scriptDir) && !Directory.Exists(scriptDir))
            {
                var r = System.Windows.MessageBox.Show(
                    $"目录不存在：\n{scriptDir}\n\n是否创建？",
                    "卡牌脚本生成目录",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                    Directory.CreateDirectory(scriptDir);
                else
                    return;
            }

            var locPath = settings.CardLocalizationJsonPath;
            if (!string.IsNullOrEmpty(locPath))
            {
                try
                {
                    var parent = Path.GetDirectoryName(Path.GetFullPath(locPath));
                    if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                    {
                        var r = System.Windows.MessageBox.Show(
                            $"卡牌信息 JSON 所在目录不存在：\n{parent}\n\n是否创建？",
                            "卡牌信息 JSON 路径",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Question);
                        if (r == MessageBoxResult.Yes)
                            Directory.CreateDirectory(parent);
                        else
                            return;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"卡牌信息 JSON 路径无效：\n{ex.Message}", "设置",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            CardPoolJson.SaveDefault(_poolRows.Select(p => new CardPoolEntry { Name = p.Name, DisplayName = p.DisplayName }).ToList());
            EditorSettingsJson.SaveDefault(settings);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "保存设置失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
