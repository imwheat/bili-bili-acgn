using System.IO;
using System.Windows;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

public partial class KeywordEditorSettingsWindow : Window
{
    public KeywordEditorSettingsWindow()
    {
        InitializeComponent();
        TxtSettingsPath.Text = $"合并配置 settings.json（keywordEditorSettings 段）: {ExeBundledSettingsJson.GetDefaultFilePath()}";
        Loaded += (_, _) => LoadIntoUi(KeywordEditorSettingsJson.LoadOrCreateDefault());
    }

    private void LoadIntoUi(KeywordEditorSettings s)
    {
        TxtKeywordsJsonPath.Text = s.KeywordLocalizationJsonPath ?? "";
        TxtPrefix.Text = s.KeywordLocalizationNamespacePrefix ?? "BILIBILIACGN";
    }

    private KeywordEditorSettings CollectFromUi() =>
        new()
        {
            SchemaVersion = 1,
            KeywordLocalizationJsonPath = TxtKeywordsJsonPath.Text.Trim(),
            KeywordLocalizationNamespacePrefix = (TxtPrefix.Text ?? "").Trim().ToUpperInvariant()
        };

    private void BtnBrowseKeywordsJson_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "JSON (*.json)|*.json|所有文件 (*.*)|*.*",
            Title = "选择或指定 card_keywords.json",
            CheckFileExists = false
        };
        var cur = TxtKeywordsJsonPath.Text.Trim();
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
            dlg.FileName = "card_keywords.json";

        if (dlg.ShowDialog() == true)
            TxtKeywordsJsonPath.Text = dlg.FileName;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var s = CollectFromUi();
            if (!string.IsNullOrEmpty(s.KeywordLocalizationJsonPath))
            {
                try
                {
                    var parent = Path.GetDirectoryName(Path.GetFullPath(s.KeywordLocalizationJsonPath));
                    if (!string.IsNullOrEmpty(parent) && !Directory.Exists(parent))
                    {
                        var r = System.Windows.MessageBox.Show(
                            $"card_keywords.json 所在目录不存在：\n{parent}\n\n是否创建？",
                            "card_keywords.json 路径",
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
                    System.Windows.MessageBox.Show($"card_keywords.json 路径无效：\n{ex.Message}", "设置",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            KeywordEditorSettingsJson.SaveDefault(s);
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

