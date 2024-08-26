namespace ArdantOffical.Data.ModelVm.Users
{
    public class MenuItemCheckboxVM
    {
        //Dashboard
        public bool Dashboard { get; set; }
        //Compliance
        public bool Compliance { get; set; }
        public bool TransactionsMonitoring { get; set; }
        public bool NonFlagtxns { get; set; }
        public bool FlagTxns { get; set; }
        public bool Payments { get; set; }
        public bool Deposits { get; set; }
        public bool InternationalPayments { get; set; }

    }
}
