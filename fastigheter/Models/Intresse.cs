namespace fastigheter.Models
{
    public class Intresse
    {
        public int? AmountOfResidents { get; set; }
        public string? NameAndSocialSecurity { get; set; }
        public string? PrimaryPhone { get; set; }
        public string? PrimaryEmail { get; set; }
        public string? SecondaryPhone { get; set; }
        public string? SecondaryEmail { get; set; }
        public string? Occupation { get; set; }
        public string? CurrentLiving { get; set; }
        public string? Reference { get; set; }
        public bool? PaymentNote { get; set; }
        public string? SizeAndComments { get; set; }
        public bool Skärstad { get; set; }
        public bool Ölmstad { get; set; }
        public string? DateToMove { get; set; }
        public bool AgreeToTerms { get; set; }

        public DateTime TimeOfSubmit = DateTime.Now;
        public string Id = DateTime.Now.GetHashCode().ToString();
        
    }
}
