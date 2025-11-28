namespace WebStore.Models
{
    public class Report
    {
        public int Id { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public decimal TotalEarnings { get; set; }   
        public int? TopProductId { get; set; }      
    }
}
