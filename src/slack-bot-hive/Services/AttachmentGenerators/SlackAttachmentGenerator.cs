using System;
using System.Collections.Generic;
using slack_bot_hive.Models;

namespace slack_bot_hive.Services.AttachmentGenerators
{
    public abstract class SlackAttachmentGenerator : ISlackAttachmentGenerator
    {
        protected int AdjustReturnCount(int count, int max)
        {
            if (count > max)
                return max;

            return count;
        }

        protected string GetRandomHexColour()
        {
            var random = new Random();
            return String.Format("#{0:X6}", random.Next(0x1000000)); // = "#A197B9"
        }

        protected int GetTotalPageCount(int totalItems, int recordsPerPage)
        {
            return (totalItems + recordsPerPage - 1) / recordsPerPage;
        }

        protected int GetPagedItemOffsetStart(int page, int recordsPerPage)
        {
            return (page - 1) * recordsPerPage;
        }

        protected int GetPagedItemOffsetEnd(int page, int recordsPerPage, int totalItems)
        {
            int startOffset = GetPagedItemOffsetStart(page, recordsPerPage);
            int itemCount = GetPageItemCount(totalItems, startOffset,recordsPerPage);
            return startOffset + itemCount;
        }

        protected int GetPageItemCount(int totalItems, int offset, int recordsPerPage)
        {
            int remaining = totalItems - offset;

            return remaining > recordsPerPage ? recordsPerPage : remaining;
        }

        public abstract string GenerateAttachment(IEnumerable<Note> notes);

        public abstract string GenerateAttachment(IEnumerable<Note> notes, string cacheItemId, int page = 1);
    }
}
