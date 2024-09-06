namespace ChessAPI.models
{
    public class PlayerSponsor
    {
        public int PlayerId { get; set; }
        public int SponsorId { get; set; }
        public decimal SponsorshipAmount { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
    }

}
