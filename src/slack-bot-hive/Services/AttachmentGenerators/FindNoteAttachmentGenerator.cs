using System.Collections.Generic;
using System.Linq;
using slack_bot_hive.Models;
using Newtonsoft.Json;
using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.AttachmentGenerators
{
    public class FindNoteAttachmentGenerator : SlackAttachmentGenerator
    {
        public override string GenerateAttachment(IEnumerable<Note> notes)
        {
            int maxNumberOfNotes = AdjustReturnCount(notes.Count(), Constants.Constants.ReturnedNotesMaxFind);

            List<SlackAttachment> listOfAttachments = new List<SlackAttachment>();

            string colour = GetRandomHexColour();

            foreach (int i in Enumerable.Range(0, maxNumberOfNotes))
            {
                SlackAttachmentField slackAttachmentField = new SlackAttachmentField()
                {
                    Value = notes.ElementAt(i).Text
                };

                SlackAttachment slackAttachment = new SlackAttachment()
                {
                    Color = colour,
                    Fields = new SlackAttachmentField[] { slackAttachmentField },
                    Footer = "<@" + notes.ElementAt(i).Author + ">",
                    Ts = notes.ElementAt(i).Timestamp
                };

                listOfAttachments.Add(slackAttachment);
            }

            return JsonConvert.SerializeObject(listOfAttachments);
        }
        
        public override string GenerateAttachment(IEnumerable<Note> notes, string cacheItemId, int page) => throw new System.NotImplementedException();
    }
}
