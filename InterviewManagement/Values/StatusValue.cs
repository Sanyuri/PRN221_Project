namespace InterviewManagement.Values
{
    public class StatusValue
    {
        public static IDictionary<int, string> CandidateStatus { get; } = new Dictionary<int, string>()
        {
            { 1, "Waiting for interview" },
            { 2, "Waiting for approval" },
            { 3, "Waiting for response" },
            { 4, "Open" },
            { 5, "Passed" },
            { 6, "Approved" },
            { 7, "Rejected" },
            { 8, "Accepted offer" },
            { 9, "Declined offer" },
            { 11, "Failed interview" },
            { 10, "Cancelled offer" },
            { 12, "Cancelled interview" },
            { 13, "Banned" }
        };

        public static IDictionary<int, string> UserStatus = new Dictionary<int, string>() {
         { 1, "Active" },
         { 2, "InActive" }
        };

        public static ICollection<string> JobStatus = new HashSet<string>()
        {
            "Draft",
            "Open",
            "Closed",
        };

        public static ICollection<string> ScheduleStatus = new HashSet<string>() {
            "New",
            "Invited",
            "Interviewed",
            "Cancelled"
        };

        public static ICollection<string> OfferStatus = new HashSet<string>() {
            "Waiting for approval",
            "Approved",
            "Rejected",
            "Waiting for response",
            "Accepted offer",
            "Declined offer",
            "Cancelled"
        };
    }
}
