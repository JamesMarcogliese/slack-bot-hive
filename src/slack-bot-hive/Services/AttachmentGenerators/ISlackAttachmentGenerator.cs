using System.Collections.Generic;
using slack_bot_hive.Models;

namespace slack_bot_hive.Services.AttachmentGenerators
{
    public interface ISlackAttachmentGenerator
    {
        string GenerateAttachment(IEnumerable<Note> notes);
        string GenerateAttachment(IEnumerable<Note> notes, string cacheItemId, int page);
    }

}
