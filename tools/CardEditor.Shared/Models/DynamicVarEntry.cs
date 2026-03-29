using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace CardEditor.Shared.Models;

public sealed class DynamicVarEntry : INotifyPropertyChanged
{
    private string _kind = "Damage";
    private decimal _baseValue;
    private decimal _upgradeValue;
    /// <summary>默认与 <see cref="Kind"/> 为 Damage 对齐；Damage/Block 默认含 <see cref="ValueProp.Move"/>。</summary>
    private ValueProp _valueProp = ValueProp.Move;

    [JsonPropertyName("kind")]
    public string Kind
    {
        get => _kind;
        set
        {
            if (_kind == value) return;
            var old = _kind;
            _kind = value;
            OnPropertyChanged();
            AdjustValuePropWhenKindChanges(old, value);
        }
    }

    /// <summary>Damage、Block 新建或 kind 改为二者之一且当前无标志时默认 <see cref="ValueProp.Move"/>；从 Damage/Block 改为其它 kind 时清空标志。</summary>
    private void AdjustValuePropWhenKindChanges(string? oldKind, string? newKind)
    {
        var oldDb = IsDamageOrBlockKind(oldKind);
        var newDb = IsDamageOrBlockKind(newKind);
        if (oldDb && !newDb)
        {
            if (_valueProp != ValueProp.None)
            {
                _valueProp = ValueProp.None;
                OnPropertyChanged(nameof(ValueProp));
                NotifyValuePropFlagBindings();
            }
            return;
        }
        if (newDb && _valueProp == ValueProp.None)
        {
            _valueProp = ValueProp.Move;
            OnPropertyChanged(nameof(ValueProp));
            NotifyValuePropFlagBindings();
        }
    }

    public static bool IsDamageOrBlockKind(string? kind)
    {
        var k = kind?.Trim() ?? "";
        return k.Equals("Damage", StringComparison.OrdinalIgnoreCase)
               || k.Equals("Block", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>Damage、Block 默认 <see cref="ValueProp.Move"/>，其余为 <see cref="ValueProp.None"/>。</summary>
    public static ValueProp DefaultValuePropForKind(string? kind) =>
        IsDamageOrBlockKind(kind) ? ValueProp.Move : ValueProp.None;

    [JsonPropertyName("baseValue")]
    public decimal BaseValue
    {
        get => _baseValue;
        set
        {
            if (_baseValue == value) return;
            _baseValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FinalValue));
        }
    }

    /// <summary>升级增量，与 BaseValue 相加得到最终数值。</summary>
    [JsonPropertyName("upgradeValue")]
    public decimal UpgradeValue
    {
        get => _upgradeValue;
        set
        {
            if (_upgradeValue == value) return;
            _upgradeValue = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(FinalValue));
        }
    }

    /// <summary>最终数值 = BaseValue + UpgradeValue（仅展示，不单独序列化）。</summary>
    [JsonIgnore]
    public decimal FinalValue => BaseValue + UpgradeValue;

    /// <summary>数值属性（位标志，可组合）。未写入 JSON 的旧数据反序列化为 <see cref="ValueProp.None"/>，生成代码时对 Damage/Block 有默认回退。</summary>
    [JsonPropertyName("valueProp")]
    public ValueProp ValueProp
    {
        get => _valueProp;
        set
        {
            if (_valueProp == value) return;
            _valueProp = value;
            OnPropertyChanged();
            NotifyValuePropFlagBindings();
        }
    }

    [JsonIgnore]
    public bool FlagMove
    {
        get => _valueProp.HasFlag(ValueProp.Move);
        set => SetValuePropFlag(ValueProp.Move, value);
    }

    [JsonIgnore]
    public bool FlagUnpowered
    {
        get => _valueProp.HasFlag(ValueProp.Unpowered);
        set => SetValuePropFlag(ValueProp.Unpowered, value);
    }

    [JsonIgnore]
    public bool FlagUnblockable
    {
        get => _valueProp.HasFlag(ValueProp.Unblockable);
        set => SetValuePropFlag(ValueProp.Unblockable, value);
    }

    [JsonIgnore]
    public bool FlagSkipHurtAnim
    {
        get => _valueProp.HasFlag(ValueProp.SkipHurtAnim);
        set => SetValuePropFlag(ValueProp.SkipHurtAnim, value);
    }

    private void SetValuePropFlag(ValueProp flag, bool on)
    {
        var n = on ? _valueProp | flag : _valueProp & ~flag;
        if (n == _valueProp) return;
        _valueProp = n;
        OnPropertyChanged(nameof(ValueProp));
        NotifyValuePropFlagBindings();
    }

    private void NotifyValuePropFlagBindings()
    {
        OnPropertyChanged(nameof(FlagMove));
        OnPropertyChanged(nameof(FlagUnpowered));
        OnPropertyChanged(nameof(FlagUnblockable));
        OnPropertyChanged(nameof(FlagSkipHurtAnim));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
