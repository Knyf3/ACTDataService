namespace ACTDataService.Models
{
    public class UserFileModel
    {
        public int UserNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int CardNumber { get; set; }
        public byte[]? Photo { get; set; }

    }
}
