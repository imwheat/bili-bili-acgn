using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

public partial class SettingsWindow : Window
{
    private readonly ObservableCollection<string> _poolTypes = [];

    public SettingsWindow()
    {
        InitializeComponent();
        LstPoolTypes.ItemsSource = _poolTypes;
        TxtSettingsPath.Text = EditorSettingsJson.GetDefaultSettingsPath();
        Loaded += (_, _) => LoadIntoUi(EditorSettingsJson.LoadOrCreateDefault());
    }

    private void LoadIntoUi(EditorSettings s)
    {
        TxtDefaultNamespace.Text = s.DefaultNamespace;
        TxtDefaultSaveDirectory.Text = s.DefaultSaveDirectory ?? "";
        TxtDefaultCardScriptOutputDirectory.Text = s.DefaultCardScriptOutputDirectory ?? "";
        TxtCardLocalizationJsonPath.Text = s.CardLocalizationJsonPath ?? "";
        _poolTypes.Clear();
        foreach (var p in s.PoolTypeOptions)
        {
            if (!string.IsNullOrWhiteSpace(p))
                _poolTypes.Add(p.Trim());
        }
        if (_poolTypes.Count == 0)
            _poolTypes.Add("ColorlessCardPool");
    }

    private EditorSettings CollectFromUi()
    {
        var pools = _poolTypes.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.Ordinal).ToList();
        if (pools.Count == 0)
            pools.Add("ColorlessCardPool");

        return new EditorSettings
        {
            SchemaVersion = 1,
            DefaultNamespace = TxtDefaultNamespace.Text.Trim(),
            DefaultSaveDirectory = TxtDefaultSaveDirectory.Text.Trim(),
            DefaultCardScriptOutputDirectory = TxtDefaultCardScriptOutputDirectory.Text.Trim(),
            CardLocalizationJsonPath = TxtCardLocalizationJsonPath.Text.Trim(),
            PoolTypeOptions = pools
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
        if (_poolTypes.Contains(name, StringComparer.Ordinal))
        {
            System.Windows.MessageBox.Show("列表中已有该项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _poolTypes.Add(name);
        TxtNewPoolType.Clear();
        LstPoolTypes.SelectedItem = name;
        LstPoolTypes.ScrollIntoView(name);
    }

    private void BtnRemovePool_Click(object sender, RoutedEventArgs e)
    {
        if (LstPoolTypes.SelectedItem is not string sel)
        {
            System.Windows.MessageBox.Show("请先选中一项。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _poolTypes.Remove(sel);
        if (_poolTypes.Count == 0)
            _poolTypes.Add("ColorlessCardPool");
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
