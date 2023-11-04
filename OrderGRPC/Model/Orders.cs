using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Order.Model
{
    public class Orders
    {
        [Key]
        public int ID { get; set; }

        [Column("issue_date")]
        public DateTime IssueDate { get; set; }

        public string Subject { get; set; }

        [Column("customer_id")]
        public int CustomerID { get; set; }

        [Column("due_date")]
        public DateTime DeliveryDate { get; set; }

        public int Status { get; set; }

        [Column("sub_total")]
        public double SubTotal { get; set; }

        [Column("grand_total")]
        public double GrandTotal { get; set; }

        //[ForeignKey("CustomerID")]
        //public Customer Customer { get; set; }

        public List<OrdersDetail> OrdersDetail { get; set; }

        [NotMapped]
        public int TotalItem { get; set; }

        public double Tax { get; set; }

        [NotMapped]
        public string? OrdersNumber { get; set; }
    }

    public class OrdersDetail
    {
        [Key]
        public int ID { get; set; }

        [Column("Orders_id")]
        public int OrdersID { get; set; }

        [Column("item_id")]
        public int ItemID { get; set; }

        public int Qty { get; set; }

        //[ForeignKey("ItemID")]
        //public Item Item { get; set; }

        [NotMapped]
        public double Amount { get; set; }
    }
}
