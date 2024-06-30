namespace TodoListApp.WebApi.Profiles;
// In your WebApi project
public struct NonNullableString
{
    private readonly string _value;

    public NonNullableString(string value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    public override string ToString() => _value;
    public static implicit operator string(NonNullableString value) => value._value;
    public static implicit operator NonNullableString(string value) => new NonNullableString(value);
}

