using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Invoices.Model
{
    public class Invoice
    {
        [Key]
        public int ID { get; set; }

        [Column("issue_date")]
        public DateTime IssueDate { get; set; }

        public string Subject { get; set; }

        [Column("customer_id")]
        public int CustomerID { get; set; }

        [Column("due_date")]
        public DateTime DueDate { get; set; }

        public int Status { get; set; }

        [Column("sub_total")]
        public double SubTotal { get; set; }

        [Column("grand_total")]
        public double GrandTotal { get; set; }

        //[ForeignKey("CustomerID")]
        //public Customer Customer { get; set; }

        public List<InvoiceDetail> InvoiceDetail { get; set; }

        [NotMapped]
        public int TotalItem { get; set; }

        public double Tax { get; set; }

        [NotMapped]
        public string InvoiceNumber { get; set; }
    }

    public class InvoiceDetail
    {
        [Key]
        public int ID { get; set; }

        [Column("invoice_id")]
        public int InvoiceID { get; set; }

        [Column("item_id")]
        public int ItemID { get; set; }

        public int Qty { get; set; }

        //[ForeignKey("ItemID")]
        //public Item Item { get; set; }

        [NotMapped]
        public double Amount { get; set; }
    }

}
