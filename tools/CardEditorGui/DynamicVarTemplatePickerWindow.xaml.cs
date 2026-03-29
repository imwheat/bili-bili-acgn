using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

/// <summary>从模版列表中选择示例文本插入卡牌描述（只读，与管理窗口数据源相同）。</summary>
public partial class DynamicVarTemplatePickerWindow : Window
{
    private readonly MainWindow _main;
    private readonly ObservableCollection<DynamicVarTemplate> _rows = [];

    public DynamicVarTemplatePickerWindow(MainWindow main)
    {
        _main = main;
        Owner = main;
        InitializeComponent();
        GridTemplates.ItemsSource = _rows;
        ReloadFromDisk();
    }

    private void ReloadFromDisk()
    {
        _rows.Clear();
        foreach (var t in DynamicVarTemplateJson.LoadOrCreateDefault())
        {
            _rows.Add(new DynamicVarTemplate
            {
                Name = t.Name,
                Description = t.Description,
                ExampleText = t.ExampleText
            });
        }
    }

    private void BtnAdd_Click(object sender, RoutedEventArgs e)
    {
        if (GridTemplates.SelectedItem is not DynamicVarTemplate row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var ex = row.ExampleText?.Trim() ?? "";
        if (ex.Length == 0)
        {
            MessageBox.Show("该模版「示例」为空。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _main.InsertDescriptionTemplateExample(ex);
    }

    private void BtnManage_Click(object sender, RoutedEventArgs e)
    {
        var w = new DynamicVarTemplatesWindow(_main);
        if (w.ShowDialog() == true)
            ReloadFromDisk();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
