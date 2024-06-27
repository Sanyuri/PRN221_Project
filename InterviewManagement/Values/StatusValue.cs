namespace InterviewManagement.Values
{
    public class StatusValue
    {
        public static IDictionary<int, string> CandidateStatus { get; } = new Dictionary<int, string>()
        {
            { 1, "Waiting for interview" },
            { 2, "Cancelled interview" },
            { 3, "Passed" },
            { 4, "Failed interview" },
            { 5, "Open" },
            { 6, "Waiting for approval" },
            { 7, "Approved" },
            { 8, "Rejected" },
            { 9, "Waiting for response" },
            { 10, "Accepted offer" },
            { 11, "Declined offer" },
            { 12, "Cancelled offer" },
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
