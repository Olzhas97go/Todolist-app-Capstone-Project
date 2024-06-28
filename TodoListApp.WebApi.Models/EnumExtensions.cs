namespace TodoListApp.WebApi.Models;
public static class EnumExtensions
{
    public static string ToFriendlyString(this Enum value)
    {
        return value.ToString(); // You can customize the conversion if needed
    }
}
