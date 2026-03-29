using System.Collections.ObjectModel;
using System.Windows;
using MessageBox = System.Windows.MessageBox;
using CardEditor.Shared;
using CardEditor.Shared.Models;

namespace CardEditorGui;

public partial class DynamicVarTemplatesWindow : Window
{
    private readonly MainWindow _main;
    private readonly ObservableCollection<DynamicVarTemplate> _rows = [];

    public DynamicVarTemplatesWindow(MainWindow main)
    {
        _main = main;
        Owner = main;
        InitializeComponent();
        foreach (var t in DynamicVarTemplateJson.LoadOrCreateDefault())
        {
            _rows.Add(new DynamicVarTemplate
            {
                Name = t.Name,
                Description = t.Description,
                ExampleText = t.ExampleText
            });
        }
        GridTemplates.ItemsSource = _rows;
    }

    private void BtnAddToCard_Click(object sender, RoutedEventArgs e)
    {
        if (GridTemplates.SelectedItem is not DynamicVarTemplate row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var kind = row.Name?.Trim() ?? "";
        if (kind.Length == 0)
        {
            MessageBox.Show("name（kind）不能为空。", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!_main.TryAddDynamicVarFromTemplateKind(kind))
            MessageBox.Show("已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    private void BtnNew_Click(object sender, RoutedEventArgs e)
    {
        _rows.Add(new DynamicVarTemplate { Name = "NewKind", Description = "", ExampleText = "" });
        GridTemplates.SelectedItem = _rows[^1];
    }

    private void BtnRemove_Click(object sender, RoutedEventArgs e)
    {
        if (GridTemplates.SelectedItem is not DynamicVarTemplate row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        _rows.Remove(row);
    }

    private void BtnUp_Click(object sender, RoutedEventArgs e)
    {
        if (GridTemplates.SelectedItem is not DynamicVarTemplate row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var i = _rows.IndexOf(row);
        if (i <= 0)
            return;
        _rows.Move(i, i - 1);
        GridTemplates.SelectedItem = row;
    }

    private void BtnDown_Click(object sender, RoutedEventArgs e)
    {
        if (GridTemplates.SelectedItem is not DynamicVarTemplate row)
        {
            MessageBox.Show("请先选中一行。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var i = _rows.IndexOf(row);
        if (i < 0 || i >= _rows.Count - 1)
            return;
        _rows.Move(i, i + 1);
        GridTemplates.SelectedItem = row;
    }

    private void BtnOk_Click(object sender, RoutedEventArgs e)
    {
        var names = new HashSet<string>(StringComparer.Ordinal);
        foreach (var r in _rows)
        {
            var n = r.Name?.Trim() ?? "";
            if (n.Length == 0)
            {
                MessageBox.Show("name（kind）不能为空。", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (!names.Add(n))
            {
                MessageBox.Show($"重复的 name：{n}", "校验", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        try
        {
            DynamicVarTemplateJson.SaveDefault(_rows.ToList());
            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "保存失败", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
