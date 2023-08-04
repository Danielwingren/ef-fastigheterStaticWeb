namespace fastigheter.Models
{
    public class ErrorReport
    {
        public string? AppartmentNumber { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Mail { get; set; }
        public string? ErrorDescription { get; set; }
        public bool? AccessWithKeyYes { get; set; }
        public bool? AccessWithKeyNo { get; set; }

        public DateTime TimeOfSubmit = DateTime.Now;
        public string Id = DateTime.Now.GetHashCode().ToString();

    }
}
