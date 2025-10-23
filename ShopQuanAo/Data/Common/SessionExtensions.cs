using System.Text.Json;

namespace ShopQuanAo.Data.Common
{
    public static class SessionExtensions
    {
        static readonly JsonSerializerOptions _opts = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static void SetObject<T>(this ISession session, string key, T value) =>
            session.SetString(key, JsonSerializer.Serialize(value, _opts));

        public static T? GetObject<T>(this ISession session, string key)
        {
            var s = session.GetString(key);
            return s == null ? default : JsonSerializer.Deserialize<T>(s, _opts);
        }
    }
}
