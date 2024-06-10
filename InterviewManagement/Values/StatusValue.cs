namespace InterviewManagement.Values
{
    public class StatusValue
    {
        public static ICollection<string> CanidateStatus { get; } = new HashSet<string>() {
        "Waiting for interview",
        "Cancelled interview",
        "Passed",
        "Failed interview",
        "Open",
        "Waiting for approval",
        "Approved",
        "Rejected",
        "Waiting for response",
        "Accepted offer",
        "Declined offer",
        "Cancelled offer",
        "Banned"
        };

        public static ICollection<string> UserStatus = new HashSet<string>() {
        "Active",
        "InActive"
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
