using Newtonsoft.Json;

namespace slack_bot_hive.Services.CallbackIdBuilders
{
    public class PageCallbackId 
    {
        public string ActionName { get; }
        public string CallbackType { get; }
        public string Page { get; }
        public string CacheItemId { get; }

        public PageCallbackId(string actionName, string callbackType, string page, string cacheItemId)
        {
            ActionName = actionName;
            CallbackType = callbackType;
            Page = page;
            CacheItemId = cacheItemId;
        }

        public static PageCallbackId FromJson(string json) => JsonConvert.DeserializeObject<PageCallbackId>(json);
    }

    public static partial class Serialize
    {
        public static string ToJson(this PageCallbackId self) => JsonConvert.SerializeObject(self);
    }
}
