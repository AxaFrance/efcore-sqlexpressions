namespace Code_Less.DbStore
{
    public class CustomerDemographic
    {
        public CustomerDemographic()
        {
            this.Customers = new HashSet<Customer>();
        }

        public string CustomerTypeId { get; set; } = null!;
        public string? CustomerDesc { get; set; }

        public virtual ICollection<Customer> Customers { get; set; }
    }
}