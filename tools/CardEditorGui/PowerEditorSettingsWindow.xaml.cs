using System.IO;
using System.Windows;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

public partial class PowerEditorSettingsWindow : Window
{
    private PowerEditorSettings _settings = new();

    public PowerEditorSettingsWindow()
    {
        InitializeComponent();
        TxtSettingsPath.Text = $"合并配置 settings.json（powerEditorSettings 段）: {ExeBundledSettingsJson.GetDefaultFilePath()}";
        Loaded += (_, _) => LoadIntoUi(PowerEditorSettingsJson.LoadOrCreateDefault());
    }

    private void LoadIntoUi(PowerEditorSettings s)
    {
        _settings = s;
        TxtDefaultPowersDir.Text = s.DefaultPowersCsDirectory ?? "";
        TxtPowersJsonPath.Text = s.PowerLocalizationJsonPath ?? "";
        TxtPrefix.Text = s.PowerLocalizationNamespacePrefix ?? "BILIBILIACGN";
    }

    private PowerEditorSettings CollectFromUi() =>
        new()
        {
            SchemaVersion = 1,
            DefaultPowersCsDirectory = TxtDefaultPowersDir.Text.Trim(),
            PowerLocalizationJsonPath = TxtPowersJsonPath.Text.Trim(),
            PowerLocalizationNamespacePrefix = (TxtPrefix.Text ?? "").Trim().ToUpperInvariant()
        };

    private void BtnBrowsePowersDir_Click(object sender, RoutedEventArgs e)
    {
        using var dlg = new System.Windows.Forms.FolderBrowserDialog
        {
            Description = "选择 Powers.cs 默认文件夹",
            UseDescriptionForTitle = true
        };
        var cur = TxtDefaultPowersDir.Text.Trim();
        if (Directory.Exists(cur))
            dlg.SelectedPath = cur;
        if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            TxtDefaultPowersDir.Text = dlg.SelectedPath;
    }

    private void BtnBrowsePowersJson_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "选择或指定 powers.json",
            CheckFileExists = false
        };
        var cur = TxtPowersJsonPath.Text.Trim();
        if (!string.IsNullOrEmpty(cur))
        {
            try
            {
                var full = Path.GetFullPath(cur);
                var dir = Path.GetDirectoryName(full);
                if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
                    dlg.InitialDirectory = dir;
                var name = Path.GetFileName(full);
                if (!string.IsNullOrEmpty(name))
                    dlg.FileName = name;
            }
            catch
            {
                // ignore
            }
        }
        else
            dlg.FileName = "powers.json";

        if (dlg.ShowDialog() == true)
            TxtPowersJsonPath.Text = dlg.FileName;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var s = CollectFromUi();

            if (!string.IsNullOrEmpty(s.DefaultPowersCsDirectory) && !Directory.Exists(s.DefaultPowersCsDirectory))
            {
                var r = System.Windows.MessageBox.Show(
                    $"目录不存在：\n{s.DefaultPowersCsDirectory}\n\n是否创建？",
                    "默认 Powers.cs 文件夹",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);
                if (r == MessageBoxResult.Yes)
                    Directory.CreateDirectory(s.DefaultPowersCsDirectory);
                else
                    return;
            }

            if (!string.IsNullOrEmpty(s.PowerLocalizationJsonPath))
            {
                try
                {
                    var parent = Path.GetDirectoryName(Path.GetFullPath(s.PowerLocalizationJsonPath));
                    if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                    {
                        var r = System.Windows.MessageBox.Show(
                            $"powers.json 所在目录不存在：\n{parent}\n\n是否创建？",
                            "powers.json 路径",
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
                    System.Windows.MessageBox.Show($"powers.json 路径无效：\n{ex.Message}", "设置",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            PowerEditorSettingsJson.SaveDefault(s);
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

