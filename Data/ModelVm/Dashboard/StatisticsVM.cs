namespace ArdantOffical.Data.ModelVm.Dashboard
{
    public class StatisticsVM
    {
        private int totalClients;

        public int MonthlyFlagPayments { get; set; } = 0;
        public int YearlyFlagPayments { get; set; } = 0;
        public int TodayFlagPayments { get; set; } = 0;

        public int MonthlyFlagDeposits { get; set; } = 0;
        public int YearlyFlagDeposits { get; set; } = 0;
        public int TodayFlagDeposits { get; set; } = 0;

        public int MonthlyFxCount { get; set; } = 0;
        public int YearlyFxCount { get; set; } = 0;
        public int TodayFxCount { get; set; } = 0;

        public int TotalInternationalPayments { get; set; } = 0;
        public int TotalUsers { get; set; } = 0;

        public int ActiveClients { get; set; } = 0;
        public int DormantClients { get; set; } = 0;
        public int ClosedClients { get; set; } = 0;
        public int BlockedClients { get; set; } = 0;
        public int NewClients { get; set; } = 0;
        public int NoStatusClientAccount { get; set; } = 0;
        public int TotalClients { get => ActiveClients + DormantClients + ClosedClients + BlockedClients + NewClients+ NoStatusClientAccount; set => totalClients = value; }
        public int TotalClientProfiles { get; set; }
        public int TotalDepositors { get; set; } = 0;
        public int TotalBeneficiary { get; set; } = 0;

        public int TodayNFPayments { get; set; }
        public int MonthlyNFPayments { get; set; }
        public int YearlyNFPayments { get; set; }
        // public int TotalNFPayments { get; set; }

        public int TodayNFDeposits { get; set; }
        public int MonthlyNFDeposits { get; set; }
        public int YearlyNFDeposits { get; set; }
        public int TotalNFDeposits { get; set; }


    }
}
