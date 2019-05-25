using System;
using System.Collections.Generic;
using System.Linq;
using slack_bot_hive.Models;
using Newtonsoft.Json;
using slack_bot_hive.Services.CallbackIdBuilders;
using Slack.Api.CSharp.EventsApi;

namespace slack_bot_hive.Services.AttachmentGenerators
{
    public class ReviewNotesAttachmentGenerator : SlackAttachmentGenerator
    {

        public override string GenerateAttachment(IEnumerable<Note> notes, string cacheItemId, int page = 1)
        {
            if (page < 1) throw new ArgumentOutOfRangeException(nameof(page));

            int totalPages = GetTotalPageCount(notes.Count(), Constants.Constants.ReturnedNotesMaxReview);

            int rangeStart = GetPagedItemOffsetStart(page, Constants.Constants.ReturnedNotesMaxReview);

            int pageItemCount = GetPageItemCount(notes.Count(),rangeStart, Constants.Constants.ReturnedNotesMaxReview);

            List<SlackAttachment> listOfAttachments = new List<SlackAttachment>();

            foreach (int i in Enumerable.Range(rangeStart, pageItemCount))
            {
                SlackAttachmentAction slackAction = new SlackAttachmentAction()
                {
                    Name = "option",
                    Text = "Delete",
                    Type = "button",
                    Value = "delete"
                };

                DeleteCallbackId deleteCallbackId = new DeleteCallbackId(
                    Constants.Constants.ActionNameReviewSelection,
                    Constants.Constants.CallbackIdTypeNote, 
                    notes.ElementAt(i).DocumentId);

                SlackAttachment slackAttachment = new SlackAttachment()
                {
                    Text = notes.ElementAt(i).Text,
                    Fallback = Constants.Constants.MessagePleaseUpgrade,
                    Actions = new SlackAttachmentAction[] { slackAction },
                    CallbackId = deleteCallbackId.ToJson(),
                    Ts = notes.ElementAt(i).Timestamp
                };

                listOfAttachments.Add(slackAttachment);
            }

            List<SlackAttachmentAction> slackAttachmentActions = new List<SlackAttachmentAction>();

            if (totalPages > page)
            {
                SlackAttachmentAction slackAction = new SlackAttachmentAction()
                {
                    Name = "page",
                    Text = "Next Page",
                    Type = "button",
                    Value = "nextpage"
                };

                slackAttachmentActions.Add(slackAction);
            }

            if (page > 1)
            {
                SlackAttachmentAction slackAction = new SlackAttachmentAction()
                {
                    Name = "page",
                    Text = "Prev Page",
                    Type = "button",
                    Value = "prevpage"
                };

                slackAttachmentActions.Add(slackAction);
            }

            if (slackAttachmentActions.Any())
            {
                PageCallbackId pageCallbackId = new PageCallbackId(
                    Constants.Constants.ActionNameReviewSelection,
                    Constants.Constants.CallbackIdTypePage, 
                    page.ToString(), 
                    cacheItemId);

                SlackAttachment slackAttachment = new SlackAttachment()
                {
                    Fallback = Constants.Constants.MessagePleaseUpgrade,
                    Actions = slackAttachmentActions.ToArray<SlackAttachmentAction>(),
                    CallbackId = pageCallbackId.ToJson(),
                    Color = GetRandomHexColour()
                };

                listOfAttachments.Add(slackAttachment);
            }

            return JsonConvert.SerializeObject(listOfAttachments);
        }

        public override string GenerateAttachment(IEnumerable<Note> notes) => throw new NotImplementedException();
    }
}
