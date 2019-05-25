using Newtonsoft.Json;

namespace slack_bot_hive.Services.CallbackIdBuilders
{
    public class DeleteCallbackId 
    {
        public string ActionName { get; }
        public string CallbackType { get; }
        public string DocumentId { get; }

        public DeleteCallbackId(string actionName, string actionType, string documentId)
        {
            ActionName = actionName;
            CallbackType = actionType;
            DocumentId = documentId;
        }
        public static DeleteCallbackId FromJson(string json) => JsonConvert.DeserializeObject<DeleteCallbackId>(json);
    }

    
    public static partial class Serialize
    {
        public static string ToJson(this DeleteCallbackId self) => JsonConvert.SerializeObject(self);
    }
}
