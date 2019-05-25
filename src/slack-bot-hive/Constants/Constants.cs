

namespace slack_bot_hive.Constants
{
    public static class Constants
    {
        #region Indexes
        public const string IndexNameNote = "notes";
        public const string IndexNameAuthor = "authors";
        #endregion

        #region Request Types
        public const string RequestTypeUrlVerification = "url_verification";
        public const string RequestTypeEventCallback = "event_callback";
        public const string RequestTypeInteractiveMessage = "interactive_message";
        #endregion

        #region Event Patterns
        public const string EventPatternSetTeam = @"^team";
        public const string EventPatternSaveNote = @"^save (.*)";
        public const string EventPatternFallback = @".*";
        public const string EventPatternFindNote = @"^find (.*)";
        public const string EventPatternHelp = @"^help";
        public const string EventPatternReview = @"^review";
        #endregion

        #region Match Patterns
        public const string MatchPatternSaveNote = @"^save ";
        public const string MatchPatternFindNote = @"^find ";
        #endregion

        #region Action Names
        public const string ActionNameEmpty = "EMPTY";
        public const string ActionNameTeamSelection = "team_action";
        public const string ActionNameReviewSelection = "review_action";
        #endregion

        #region CallbackId Types
        public const string CallbackIdTypeNote = "note";
        public const string CallbackIdTypePage = "page";
        #endregion

        #region Numerical Settings
        public const int ReturnedNotesMaxFind = 4;
        public const int ReturnedNotesMaxReview = 10;
        public const int DisappearingMessageTimeSpan = 10;
        #endregion

        #region External Data Paths
        public const string PathTeamMenu = @"/Constants/TeamList.json";
        #endregion

        #region Messages
        public const string MessageFallback = "Sorry, I don't understand. Type 'help' to see a list of what I can do!";
        public const string MessageTeamQuestion = "What team do you belong to?";
        public const string MessageNoteSaved = "Note saved!";
        public const string MessageSearchNoNotes = "No notes found that matched your query!";
        public const string MessageSearchFoundNotes = "Here what I found:";
        public const string MessageNoUserNotesFound = "You haven't saved any notes yet!";
        public const string MessagePleaseUpgrade = "Please upgrade slack to see this message!";
        public const string MessageUserNotesFound = "Here are your notes:";
        public const string MessageNoteDeleted = @"Note has been deleted!";
        public const string MessageReviewNotesExpired = @"Review Notes listing has expired.";
        public const string MessageHelp =
            @"I'm a bot to help you save, share, and search for tidbits of information!

            * team - set your team 
            * save [note] - save your note
            * find [keyword] - search for notes that match the keywords you provide
            * review - review your past notes

            :rotating_light: NOTICE - THIS APP IS IN BETA. DO NOT SAVE SENSITIVE INFORMATION! :rotating_light: ";
        public static string GetTeamJoinMessage(string team)
        {
            return "You are now sharing notes with the "+ team + " team!";
        }

        public static string GetCannotSaveMessage(string action)
        {
            return @"Wait! :raised_hand: I need to know what team you're on before I can " + action + @" your notes! Please set your team by using the 'team' command!";
        }
        #endregion

    }
}
