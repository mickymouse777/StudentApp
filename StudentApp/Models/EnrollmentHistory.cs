namespace StudentApp.Models
{
    public class EnrollmentHistory
    {
        public int EnrollmentHistoryId { get; set; }
        public int EnrollmentId { get; set; }
        public DateTime ChangeDate { get; set; }
        
        public Enrollment? Enrollment { get; set; }
    }
}
