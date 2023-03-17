namespace Code_Flexibility.DbStore
{
    public class Shipper
    {
        public Shipper()
        {
            this.Orders = new HashSet<Order>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; } = null!;
        public string? Phone { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}