namespace Code_Flexibility.DbStore
{
    public class Territory
    {
        public Territory()
        {
            this.Employees = new HashSet<Employee>();
        }

        public Territory(string territoryDescription) : this()
        {
            this.TerritoryDescription = territoryDescription;
        }

        public string TerritoryId { get; set; } = null!;
        public string TerritoryDescription { get; set; } = null!;
        public int RegionId { get; set; }

        public virtual Region Region { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; }
    }
}