using DinkToPdf;
using DinkToPdf.Contracts;
using FGCCore.Data;
using FGCCore.Data.ModelVm;
using FGCCore.Data.ModelVm.ClientVM;
using FGCCore.Data.ModelVm.Treasury;
using FGCCore.Helpers;
using FGCCore.Helpers.Extensions;
using FGCCore.IService;
using FGCCore.Models;
using FGCCore.Models.KYC;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace FGCCore.Services
{
    public class TreasuryServices : ControllerBase, ITreasuryServices
    {
        private IConverter _converter;
        private readonly FGCDbContext context;
        private IWebHostEnvironment Environment { get; set; }
        private IEmailSender _emailsender { get; set; }
        public string rootFolder;
        public string templateFolder;
        private readonly AuthenticationStateProvider UserauthenticationStateProvider;
        CurrentUserInfoVM Userinfo = new CurrentUserInfoVM();
        public EmailService _emailService;
        public TreasuryServices(FGCDbContext context, AuthenticationStateProvider _UserauthenticationStateProvider, IConverter converter, IWebHostEnvironment _Environment, IEmailSender emailSender)
        {
            this.context = context;
            //var gg = myConfiguration.Value.token;
            this.UserauthenticationStateProvider = _UserauthenticationStateProvider;
            _converter = converter;
            Environment = _Environment;
            rootFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/UploadImages");
            templateFolder = Path.Combine(Environment.ContentRootPath, "wwwroot/tpl/");
            _emailsender = emailSender;
            _emailService = new EmailService();
            
        }
        public async Task<Exception> DeleteFXTransaction(int transactionId)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXTransactions Fxtransaction = context.FXTransactions.Find(transactionId);
                    Fxtransaction.IsDelete = true;
                    //string status = "Fx Transaction with FGC Reference :" + Fxtransaction.ConfirmationReference + " Deleted" ;
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("Transaction Details: <br><br> Client Name: " + Fxtransaction.ClientName + " <br> Beneficiary Name: " + Fxtransaction.BeneficiaryName + " <br>");
                    //sb.Append(" Deal Date: " + Fxtransaction.DealDate + " <br>  Amount : " + Fxtransaction.Ct_Amount1);

                    //TblFgclog FGCLog = new TblFgclog
                    //{
                    //    Object = "Treasury",
                    //    Action = "Transaction Delete",
                    //    Status = status,
                    //    Remarks = sb.ToString(),
                    //    PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                    //    UserId = Userinfo.UserId,
                    //    DatePosted = DateTime.Now.DateTime_UK()

                    //};
                    //await context.TblFgclogs.AddAsync(FGCLog);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<Exception> UndoFxTransaction(int transactionId)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXTransactions Fxtransaction = context.FXTransactions.Find(transactionId);
                    Fxtransaction.IsDelete = false;
                    string status = "Fx Transaction Reverted Back";
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Transaction Details: <br><br> Client Name: " + Fxtransaction.ClientName + " <br> Beneficiary Name: " + Fxtransaction.BeneficiaryName + " <br>");
                    sb.Append(" Deal Date: " + Fxtransaction.DealDate + " <br>  Amount : " + Fxtransaction.Ct_Amount1);

                    TblFgclog FGCLog = new TblFgclog
                    {
                        Object = "Treasury",
                        Action = "Transaction Undo",
                        Status = status,
                        Remarks = sb.ToString(),
                        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                        UserId = Userinfo.UserId,
                        DatePosted = DateTime.Now.DateTime_UK()

                    };
                    await context.TblFgclogs.AddAsync(FGCLog);
                    await context.SaveChangesAsync();
                    
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<FXTransactionsVMVmUseForTable> FXTransactionsSearch([FromQuery] PaginationDTO pagination, SearchFilter searchParam)
        {
            FXTransactionsVMVmUseForTable listOfFxTransactions = new();
            // Default by Date range
            // SearchParams = t => t.Date >= searchParam.FromDate && t.Date <= searchParam.ToDate;
            IQueryable<FXTransactionsVM> queryable = (LoadSearchResult());
            //var queryable = q1.AsQueryable();
            if (!string.IsNullOrEmpty(searchParam.ClientID))
            {
                queryable = queryable.ToList().Where(x => x.ClientID == searchParam.ClientID).ToList().AsQueryable();
            }
            if (!string.IsNullOrEmpty(searchParam.FxBenfID))
            {
                queryable = queryable.ToList().Where(x => x.FxBenfID == searchParam.FxBenfID).ToList().AsQueryable();
            }
            if (searchParam?.FromDate.ToString() != "1/1/0001 12:00:00 AM" && searchParam?.ToDate.ToString() != "1/1/0001 12:00:00 AM")
            {
                queryable = queryable.ToList().Where(x => x.DealDate.Value.Date >= searchParam.FromDate.Date && x.DealDate.Value.Date <= searchParam.ToDate.Date).ToList().AsQueryable();
            }
            if(!string.IsNullOrEmpty(searchParam.Currency) && searchParam.Currency !="All")
            {
                queryable = queryable.Where(s => s.SourceCurrency == searchParam.Currency);
            }
            
            listOfFxTransactions.TotalPages = HttpContext.InsertPaginationParameterInResponseDate(queryable, pagination.QuantityPerPage);
            listOfFxTransactions.TxnList = queryable.Paginate(pagination).ToList();
            // To hold all the searched records for export to excel/csv
            listOfFxTransactions.TxnListForsearch = queryable.ToList();
            listOfFxTransactions.TotalCount = queryable.Count();
            return await Task.FromResult(listOfFxTransactions);

            IQueryable<FXTransactionsVM> LoadSearchResult()
            {
                return from t in context.FXTransactions
                       orderby t.DealDate descending
                       select new FXTransactionsVM
                       {
                           ID = t.ID,
                           ClientID = t.ClientID.ToString(),
                           ClientName = t.ClientName + "-"+ t.BankAccount + "(" +(context.TblClientAccounts.Where(x=>x.BankAccount==t.BankAccount && x.IsDelete==false).Select(x=>x.BankName).FirstOrDefault()) + ")",
                           ClientAccount=t.BankAccount,
                           DealDate = t.DealDate,
                           FxBenfID = t.FxBenfID.ToString(),
                           BeneficiaryName = t.BeneficiaryName,
                           FXProviderID = t.FXProviderID,
                           FxProvider = t.FxProvider,
                           ConfirmationReference = t.ConfirmationReference,
                           DealReference = t.DealReference,
                           CurrencyPair = t.CurrencyPair,
                           SourceCurrency = t.SourceCurrency,
                           DestinationCurrency = t.DestinationCurrency,
                           BookingRate = Math.Round(t.BookingRate, 8).ToString(),
                           Mt_Amount2 = t.Mt_Amount2.ToString(),
                           Mt_TxCharges = t.Mt_TxCharges.ToString(),
                           ClientRate = Math.Round(t.ClientRate, 8).ToString(),
                           Ct_TxCharges = t.Ct_TxCharges.ToString(),
                           ProfitFee = t.ProfitFee.ToString(),
                           IsDelete = t.IsDelete
                       };
            }
        }

        public async Task<FXTransactionsVM> GetFXTransactionByID(int id)
        {
            try
            {
                FXTransactionsVM queryable = (from t in context.FXTransactions
                                              where t.ID == id
                                              select new FXTransactionsVM
                                              {
                                                  ID = t.ID,
                                                  ClientID = t.ClientID.ToString(),
                                                  ClientName = t.ClientName,
                                                  ClientAccount=t.BankAccount,
                                                  DealDate = t.DealDate,
                                                  FxBenfID = t.FxBenfID.ToString(),
                                                  BeneficiaryName = t.BeneficiaryName,
                                                  FXProviderID = t.FXProviderID,
                                                  FxProvider = t.FxProvider,
                                                  ConfirmationReference = t.ConfirmationReference,
                                                  DealReference = t.DealReference,
                                                  CurrencyPair = t.CurrencyPair,
                                                  SourceCurrency = t.SourceCurrency,
                                                  DestinationCurrency = t.DestinationCurrency,
                                                  BookingRate = Math.Round(t.BookingRate, 8).ToString(),
                                                  Mt_Amount2 = t.Mt_Amount2.ToString(),
                                                  Mt_TxCharges = t.Mt_TxCharges.ToString(),
                                                  ClientRate = Math.Round(t.ClientRate, 8).ToString(),
                                                  Ct_TxCharges = t.Ct_TxCharges.ToString(),
                                                  ProfitFee = t.ProfitFee.ToString(),
                                                  IntroducerID = t.IntroducerId,
                                                  EmailList = t.ClientEmail,
                                                  //  ClientEmail = t.ClientEmail,
                                                  //  Filenames = context.TreasuryAttachments.Where(x => x.FxTransactionID == id).Select(x => x.Filename).ToList(),
                                                  CommentbyFilenames = context.TreasuryAttachments.Where(x => x.FxTransactionID == id).Select(x => new CommentbyFilenames { fileName = x.Filename, Id = x.ID }).ToList(),
                                                  IsDelete = t.IsDelete

                                              }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FXTransactionsVMVmUseForTable> GetFXTransactions([FromQuery] PaginationDTO pagination)
        {
            try
            {
                FXTransactionsVMVmUseForTable lstFXTransactions = new FXTransactionsVMVmUseForTable();
                IQueryable<FXTransactionsVM> queryable = (from t in context.FXTransactions
                                                          orderby t.DealDate descending
                                                          select new FXTransactionsVM
                                                          {
                                                              ID = t.ID,
                                                              ClientID = t.ClientID.ToString(),
                                                              ClientName = t.ClientName,
                                                              DealDate = t.DealDate,
                                                              FxBenfID = t.FxBenfID.ToString(),
                                                              BeneficiaryName = t.BeneficiaryName,
                                                              FXProviderID = t.FXProviderID,
                                                              FxProvider = t.FxProvider,
                                                              ConfirmationReference = t.ConfirmationReference,
                                                              DealReference = t.DealReference,
                                                              CurrencyPair = t.CurrencyPair,
                                                              SourceCurrency = t.SourceCurrency,
                                                              DestinationCurrency = t.DestinationCurrency,
                                                              BookingRate = Math.Round(t.BookingRate, 8).ToString(),
                                                              Mt_Amount2 = t.Mt_Amount2.ToString(),
                                                              Mt_TxCharges = t.Mt_TxCharges.ToString(),
                                                              ClientRate = Math.Round(t.ClientRate, 8).ToString(),
                                                              Ct_TxCharges = t.Ct_TxCharges.ToString(),
                                                              ProfitFee = t.ProfitFee.ToString(),
                                                              IntroducerID = t.IntroducerId,
                                                              EmailList = t.ClientEmail,
                                                              // ClientEmail=t.ClientEmail,
                                                              // Filenames = context.TreasuryAttachments.Where(x => x.FxTransactionID == t.ID).Select(x => x.Filename).ToList(),
                                                              CommentbyFilenames = context.TreasuryAttachments.Where(x => x.FxTransactionID == t.ID).Select(x => new CommentbyFilenames { fileName = x.Filename, Id = x.ID }).ToList(),
                                                              IsDelete = t.IsDelete

                                                          }).AsQueryable();
                lstFXTransactions.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstFXTransactions.TxnList = await queryable.Paginate(pagination).ToListAsync();
                lstFXTransactions.TxnListForsearch = queryable.ToList();
                lstFXTransactions.TotalCount = queryable.Count();
                return lstFXTransactions;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> PreviewDocument(FXTransactionsVM t)
        {
            try
            {
                string htmlcontent = PopulateBody(t);
                return htmlcontent;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<Exception> SaveFXTransaction(FXTransactionsVM t)
        {
            string filename = "Transaction_Receipt" + "_" + DateTime.Now.ToString("ddmmyyyyhhmmss") + ".pdf";
            List<TreasuryAttachments> ListOfattachment = new List<TreasuryAttachments>();
            int introducerId = context.TblClients.Where(x => x.ClientId.ToString() == t.ClientID).Select(x => x.IntroducerId).FirstOrDefault();
            var introducer = context.Introducers.Where(x => x.Id == introducerId).FirstOrDefault();
            double CostOfSale = 0;
            double IntroducerCommissionRate = 0;
            string IntroducerName = string.Empty;
            if (introducer != null)
            {
                CostOfSale = Convert.ToDouble(introducer.CostOfSale / 100);
                IntroducerCommissionRate = Convert.ToDouble(introducer.FxCommissionRate / 100);
                IntroducerName = introducer.Name;
            }
            //using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            //{
                try
                {
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.FXTransactions record = new();
                    record.ClientID = t.ClientID != null ? Convert.ToInt32(t.ClientID) : 0;
                    record.ClientName = t.ClientName;
                    record.BankAccount = t.ClientAccount;
                    record.DealDate = t.DealDate;
                    record.FxBenfID = t.FxBenfID != null ? Convert.ToInt32(t.FxBenfID) : 0;
                    record.BeneficiaryName = t.BeneficiaryName;
                    record.FXProviderID = t.FXProviderID;
                    record.FxProvider = t.FxProvider;
                    record.ConfirmationReference = t.ConfirmationReference;
                    record.DealReference = t.DealReference;
                    record.CurrencyPair = t.CurrencyPair;
                    record.SourceCurrency = t.SourceCurrency;
                    record.DestinationCurrency = t.DestinationCurrency;
                    record.BookingRate = Math.Round(Convert.ToDouble(t.BookingRate), 8);
                    record.Mt_Amount2 = Convert.ToDecimal(t.Mt_Amount2);
                    record.Mt_TxCharges = Convert.ToDecimal(t.Mt_TxCharges);
                    record.Mt_Total = Convert.ToDecimal(t.Mt_Total);
                    record.Ct_Amount1 = Convert.ToDecimal(t.Ct_Amount1);
                    record.ClientRate = Math.Round(Convert.ToDouble(t.ClientRate), 8);
                    record.Ct_Amount2 = Convert.ToDecimal(t.Ct_Amount2);
                    record.Ct_TxCharges = Convert.ToDecimal(t.Ct_TxCharges);
                    record.Ct_Total = Convert.ToDecimal(t.Ct_Total);
                    record.FxTradeProfit = Convert.ToDecimal(t.FxTradeProfit);
                    record.ProfitFee = Convert.ToDecimal(t.ProfitFee);
                    record.RefCounter = t.RefCounter;
                    record.IntroducerId = introducerId;
                    record.CostOfSale = CostOfSale;
                    record.IntroducerCommissionRate = IntroducerCommissionRate;
                    record.ClientEmail = t.EmailList;
                    record.TotalDealProfit = Convert.ToDecimal(t.TotalDealProfit);
                    record.PostedBy = Userinfo.FullName;
                    record.UserID = Userinfo.UserId;
                    record.DatePosted = DateTime.Now.DateTime_UK();
                    context.FXTransactions.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.FXTransactions.AddAsync(record);
                    string status = "New Fx Transaction Added";
                    StringBuilder sb1 = new StringBuilder();
                    sb1.Append("Transaction Details: <br><br> Client Name: " + record.ClientName + " <br> Beneficiary Name: " + record.BeneficiaryName + " <br>");
                    sb1.Append(" Deal Date: " + record.DealDate + " <br>  Amount : " + record.Ct_Amount1);

                    TblFgclog FGCLog = new TblFgclog
                    {
                        Object = "Treasury",
                        Action = "New Fx Transaction Added",
                        Status = status,
                        Remarks = sb1.ToString(),
                        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                        UserId = Userinfo.UserId,
                        DatePosted = DateTime.Now.DateTime_UK()

                    };
                    await context.TblFgclogs.AddAsync(FGCLog);
                string current_email = string.Empty;
                bool IsErrorOccured=false;
                if (context.SaveChanges() > 0)
                    {
                        string htmlcontent = PopulateBody(t);
                        await CreatePDFAsync(htmlcontent, filename);
                        if (t.Filenames == null)
                        {
                            t.Filenames = new List<string>();
                        }
                        t.Filenames.Add(filename);
                        if (t.Filenames != null)
                        {
                            foreach (string item in t.Filenames)
                            {
                                TreasuryAttachments AddTreasuryAttachment = new TreasuryAttachments
                                {
                                    Filename = item,
                                    FxTransactionID = record.ID,
                                    DatePosted = DateTime.Now.DateTime_UK()
                                };
                                ListOfattachment.Add(AddTreasuryAttachment);
                            }
                            await context.TreasuryAttachments.AddRangeAsync(ListOfattachment);
                        }

                     
                        //  Send Email
                        if (t.ClientEmail != null)
                        {
                            string BankName = context.TblClientAccounts.Where(x => x.BankAccount == t.ClientAccount && x.IsDelete == false).Select(x => x.BankName).FirstOrDefault();
                            string subject = "Fx Transaction-" + t.BeneficiaryName + " (" + t.SourceCurrency + "" + t.Ct_Total + ")";
                            string filepath = Path.Combine(rootFolder, filename);
                            StringBuilder sb = new StringBuilder();
                            sb.AppendLine("Dear " + t.ClientName + "! <br/> <br />");
                            sb.AppendLine("We have booked a trade for you. Please find the attached confirmation of the payment. <br/> <br/>");
                        if (BankName != null && BankName == "LHV")
                        {
                            if (t.SourceCurrency.Equals("GBP"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("04-03-01", "01340163", "LHVBGB2L", "GB84 LHVB 0403 0101 3401 63"));
                            }
                            else if (t.SourceCurrency.Equals("EUR"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("", "", "LHVBEE20", "EE637777000133266511"));
                            }
                        }
                        else if (BankName != null && BankName == "Modulr")
                        {
                            if (t.SourceCurrency.Equals("GBP"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("04-00-72", "24196843", "", ""));
                            }
                            else if (t.SourceCurrency.Equals("EUR"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("", "", "MODRGB23XXX", "GB42 MODR 0400 7408 5798 49"));
                            }
                        }
                        else if (BankName != null && BankName == "Banking Circle")
                        {
                            if (t.SourceCurrency.Equals("GBP"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("608382", "26373491", "SAPYGB2L", "GB73 SAPY 6083 8226 3734 91"));
                            }
                            else if (t.SourceCurrency.Equals("EUR"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("", "", "BCIRLULL", "LU464080000026373600"));
                            }
                            else if (t.SourceCurrency.Equals("USD"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("608382", "26373491", "SAPYGB2L", "GB73 SAPY 6083 8226 3734 91"));
                            }
                        }
                        ////if (BankName != null && BankName == "LHV")
                        ////{
                        ////    if (t.SourceCurrency.Equals("GBP"))
                        ////    {
                        ////        sb.AppendLine("Could you also please transfer the funds to the below FGC treasury account. <br/><br/>");
                        ////        sb.AppendLine("FGC CLIENT TREASURY ACCOUNT <br/><br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'> Sort Code </span>            04-03-01 <br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'>Account number</span>         01340163  <br/>");
                        ////    }
                        ////    else if (t.SourceCurrency.Equals("EUR"))
                        ////    {

                        ////        sb.AppendLine("Could you also please transfer the funds to the below FGC treasury account. <br/><br/>");
                        ////        sb.AppendLine(" FGC CLIENT TREASURY ACCOUNT <br/><br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'>Swift Code/BIC</span>          LHVBGB2L <br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'>IBAN </span>               GB84 LHVB 0403 0101 3401 63 <br/>");
                        ////    }

                        ////}
                        ////else
                        ////{
                        ////    if (t.SourceCurrency.Equals("GBP"))
                        ////    {
                        ////        sb.AppendLine("Could you also please transfer the funds to the below FGC treasury account. <br/><br/>");
                        ////        sb.AppendLine("FGC CLIENT TREASURY ACCOUNT <br/><br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'> Sort Code </span>            04-00-72 <br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'>Account number</span>         24196843  <br/>");
                        ////    }
                        ////    else if (t.SourceCurrency.Equals("EUR"))
                        ////    {

                        ////        sb.AppendLine("Could you also please transfer the funds to the below FGC treasury account. <br/><br/>");
                        ////        sb.AppendLine(" FGC CLIENT TREASURY ACCOUNT <br/><br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'>Swift Code/BIC</span>          MODRGB23XXX <br/>");
                        ////        sb.AppendLine(" <span style='width:150px;'>IBAN </span>               GB42 MODR 0400 7408 5798 49 <br/>");
                        ////    }
                        ////}
                        sb.AppendLine("<br/> <br />");
                            sb.AppendLine();
                            sb.AppendLine("Regards <br/> FGC Operation Team <br />");
                            sb.AppendLine("<img src='https://www.fgcerp.com/images/fgcLogo.png' />");
                        
                        try
                        {
                            foreach (string email in t.ClientEmail)
                            {
                                current_email = email;
                                await _emailService.SendEmailAsync(email.Trim(), subject, sb.ToString(), filepath);
                            }
                        }
                        catch (Exception)
                        {
                            IsErrorOccured = true;
                          
                        }                      

                        
                        }

                    }
                    await context.SaveChangesAsync();
                return IsErrorOccured ? new Ok("2:" + current_email) : (Exception)new Ok("1");
                //transaction.Commit();

            }
                catch (Exception ex)
                {
                   // transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            //}
        }

        // Define a function to generate the FGC treasury account information
        private string GetFGCTreasuryAccount(string sortCode, string accountNumber, string swiftCode, string iban)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Could you also please transfer the funds to the below FGC treasury account. <br/><br/>");
            sb.AppendLine("FGC CLIENT TREASURY ACCOUNT <br/><br/>");           
            
            if (!string.IsNullOrEmpty(sortCode))
            {
                sb.AppendLine($" <span style='width:150px;'> Sort Code </span>            {sortCode} <br/>");
            }
            if (!string.IsNullOrEmpty(accountNumber))
            {
                sb.AppendLine($" <span style='width:150px;'>Account number</span>         {accountNumber}  <br/>");
            }
            if (!string.IsNullOrEmpty(swiftCode))
            {
                sb.AppendLine($" <span style='width:150px;'>Swift Code/BIC</span>          {swiftCode} <br/>");
            }
            if (!string.IsNullOrEmpty(iban))
            {
                sb.AppendLine($" <span style='width:150px;'>IBAN </span>               {iban} <br/>");
            }
            return sb.ToString();
        }
        public async Task<Exception> UpdateFXTransaction(FXTransactionsVM t, int transactionId)
        {
            string filename = "Transaction_Receipt" + "_" + DateTime.Now.ToString("ddmmyyyyhhmmss") + ".pdf";
            int introducerId = context.TblClients.Where(x => x.ClientId.ToString() == t.ClientID).Select(x => x.IntroducerId).FirstOrDefault();
            var introducer = context.Introducers.Where(x => x.Id == introducerId).FirstOrDefault();
            double CostOfSale = 0;
            double IntroducerCommissionRate = 0;
            string IntroducerName = string.Empty;
            if (introducer != null)
            {
                CostOfSale = Convert.ToDouble(introducer.CostOfSale / 100);
                IntroducerCommissionRate = Convert.ToDouble(introducer.FxCommissionRate / 100);
                IntroducerName = introducer.Name;
            }
            List<TreasuryAttachments> ListOfattachment = new List<TreasuryAttachments>();
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.FXTransactions record = context.FXTransactions.Find(transactionId);
                    record.ClientID = t.ClientID != null ? Convert.ToInt32(t.ClientID) : 0;
                    record.ClientName = t.ClientName;
                    record.BankAccount = t.ClientAccount;
                    record.DealDate = t.DealDate;
                    record.FxBenfID = t.FxBenfID != null ? Convert.ToInt32(t.FxBenfID) : 0;
                    record.BeneficiaryName = t.BeneficiaryName;
                    record.FXProviderID = t.FXProviderID;
                    record.FxProvider = t.FxProvider;
                    record.ConfirmationReference = t.ConfirmationReference;
                    record.DealReference = t.DealReference;
                    record.CurrencyPair = t.CurrencyPair;
                    record.SourceCurrency = t.SourceCurrency;
                    record.DestinationCurrency = t.DestinationCurrency;
                    record.BookingRate = Math.Round(Convert.ToDouble(t.BookingRate), 8);
                    record.Mt_Amount2 = Convert.ToDecimal(t.Mt_Amount2);
                    record.Mt_TxCharges = Convert.ToDecimal(t.Mt_TxCharges);
                    record.Mt_Total = Convert.ToDecimal(t.Mt_Total);
                    record.Ct_Amount1 = Convert.ToDecimal(t.Ct_Amount1);
                    record.ClientRate = Math.Round(Convert.ToDouble(t.ClientRate), 8);
                    record.Ct_Amount2 = Convert.ToDecimal(t.Ct_Amount2);
                    record.Ct_TxCharges = Convert.ToDecimal(t.Ct_TxCharges);
                    record.Ct_Total = Convert.ToDecimal(t.Ct_Total);
                    record.FxTradeProfit = Convert.ToDecimal(t.FxTradeProfit);
                    record.ProfitFee = Convert.ToDecimal(t.ProfitFee);
                    record.RefCounter = t.RefCounter;
                    record.IntroducerId = introducerId;
                    record.IntroducerCommissionRate = IntroducerCommissionRate;
                    record.CostOfSale = CostOfSale;
                    record.ClientEmail = t.EmailList;
                    record.TotalDealProfit = Convert.ToDecimal(t.TotalDealProfit);
                    record.PostedBy = Userinfo.FullName;
                    record.UserID = Userinfo.UserId;
                    record.DatePosted = DateTime.Now.DateTime_UK();
                    //context.FXTransactions.Add(record);
                    context.Entry(record).State = EntityState.Modified;
                    string filepath = string.Empty;
                    ////CommentbyFilenames txn_confirmationDoc = t.CommentbyFilenames.Where(x => x.fileName.Contains("Transaction_Receipt") && string.IsNullOrEmpty(x.IsDeleted)).FirstOrDefault();
                    ////filepath = txn_confirmationDoc != null ? Path.Combine(rootFolder, txn_confirmationDoc.fileName) : Path.Combine(rootFolder, filename);
                    ////string htmlcontent = PopulateBody(t);
                    ////await CreatePDFAsync(htmlcontent, filepath);
                    string htmlcontent = PopulateBody(t);
                    await CreatePDFAsync(htmlcontent, filename);
                    if (t.Filenames == null)
                    {
                        t.Filenames = new List<string>();
                    }
                    t.Filenames.Add(filename);
                    //  await context.FXTransactions.AddAsync(record);
                    if (t.Filenames != null)
                    {
                        foreach (string item in t.Filenames)
                        {
                            TreasuryAttachments AddTreasuryAttachment = new TreasuryAttachments
                            {
                                Filename = item,
                                FxTransactionID = transactionId,
                                DatePosted = DateTime.Now.DateTime_UK()
                            };
                            ListOfattachment.Add(AddTreasuryAttachment);
                        }
                        await context.TreasuryAttachments.AddRangeAsync(ListOfattachment);
                    }

                    List<TblFgclog> ListOfTblFgclog = new();
                    context.ChangeTracker.DetectChanges();
                    StringBuilder sb1 = new StringBuilder();
                    sb1.Append("Transaction Details: <br><br> Client Name: " + record.ClientName + " <br> Beneficiary Name: " + record.BeneficiaryName + " <br>");
                    sb1.Append(" Deal Date: " + record.DealDate + " <br>  Amount : " + record.Ct_Amount1);
                    foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<FXTransactions> entry in context.ChangeTracker.Entries<FXTransactions>().Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified))
                    {
                        // Gets all properties from the changed entity by reflection.
                        foreach (System.Reflection.PropertyInfo entityProperty in entry.Entity.GetType().GetProperties())
                        {
                            string propertyName = entityProperty.Name;
                            string currentValue = entry.Property(propertyName).CurrentValue?.ToString();
                            string originalValue = entry.Property(propertyName).OriginalValue?.ToString();
                            if (currentValue != originalValue)
                            {
                                if (string.IsNullOrEmpty(currentValue))
                                {
                                    currentValue = "-";
                                }

                                if (string.IsNullOrEmpty(originalValue))
                                {
                                    originalValue = "-";
                                }

                                TblFgclog FGCLog = new TblFgclog
                                {
                                    Object = "Treasury",
                                    Action = "Fx Transaction updated",
                                    Remarks = $" {propertyName} field  updated for Fx Transaction <br/>  <span style='font-weight:bold'>({sb1.ToString()}) </span>  ",
                                    OldValue = originalValue?.ToString(),
                                    NewValue = currentValue?.ToString(),
                                    PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                                    UserId = Userinfo.UserId,
                                    DatePosted = DateTime.Now.DateTime_UK()
                                };
                                ListOfTblFgclog.Add(FGCLog);
                            }
                        }
                    }

                    // Send Email
                    //  Send Email
                    if (t.ClientEmail != null)
                    {
                        string BankName = context.TblClientAccounts.Where(x => x.BankAccount == t.ClientAccount && x.IsDelete == false).Select(x => x.BankName).FirstOrDefault();
                        string subject = "Fx Transaction-" + t.BeneficiaryName + " (" + t.SourceCurrency + "" + t.Ct_Total + ")";
                        filepath = Path.Combine(rootFolder, filename);
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("Dear " + t.ClientName + "! <br/> <br />");
                        sb.AppendLine("We have booked a trade for you. Please find the attached confirmation of the payment. <br/> <br/>");
                        if (BankName != null && BankName == "LHV")
                        {
                            if (t.SourceCurrency.Equals("GBP"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("04-03-01", "01340163", "LHVBGB2L", "GB84 LHVB 0403 0101 3401 63"));
                            }
                            else if (t.SourceCurrency.Equals("EUR"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("", "", "LHVBEE20", "EE637777000133266511"));
                            }
                        }
                        else if(BankName !=null && BankName== "Modulr")
                        {   
                            if (t.SourceCurrency.Equals("GBP"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("04-00-72", "24196843", "", ""));
                            }
                            else if (t.SourceCurrency.Equals("EUR"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("", "", "MODRGB23XXX", "GB42 MODR 0400 7408 5798 49"));
                            }
                        } else if(BankName !=null && BankName== "Banking Circle")
                        {
                            if (t.SourceCurrency.Equals("GBP"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("608382", "26373491", "SAPYGB2L", "GB73 SAPY 6083 8226 3734 91"));
                            }
                            else if (t.SourceCurrency.Equals("EUR"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("", "", "BCIRLULL", "LU464080000026373600"));
                            } else if(t.SourceCurrency.Equals("USD"))
                            {
                                sb.AppendLine(GetFGCTreasuryAccount("608382", "26373491", "SAPYGB2L", "GB73 SAPY 6083 8226 3734 91"));
                            }
                        }

                        sb.AppendLine("<br/> <br />");
                        sb.AppendLine();
                        sb.AppendLine("Regards <br/> FGC Operation Team <br />");
                        sb.AppendLine("<img src='https://www.fgcerp.com/images/fgcLogo.png' />");
                        foreach (string email in t.ClientEmail)
                        {
                            await _emailService.SendEmailAsync(email, subject, sb.ToString(), filepath);
                        }
                    }
                    await context.TblFgclogs.AddRangeAsync(ListOfTblFgclog);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public bool DeleteAttachment(int Id)
        {
            try
            {
                TreasuryAttachments TreasuryAttachments = (from s in context.TreasuryAttachments
                                                           where s.ID == Id && s.IsDeleted == false
                                                           select s).FirstOrDefault();
                if (TreasuryAttachments != null)
                    TreasuryAttachments.IsDeleted = true;
                // context.TreasuryAttachments.Remove(TreasuryAttachments);
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<SelectListItem> ddCurrencyPair()
        {
            IQueryable<SelectListItem> DropdownList;

            DropdownList = (from d in context.FXCurrencyPairs
                            where d.IsDelete == false
                            select new SelectListItem
                            {
                                Text = d.CurrencyPair,
                                Value = d.CurrencyPair
                            });
            return DropdownList.ToList();
        }
        public async Task<FxBeneficiaryVMVmUseForTable> GetBeneficiaries([FromQuery] PaginationDTO pagination, int CustomerID)
        {
            try
            {
                FxBeneficiaryVMVmUseForTable lstBeneficiaries = new FxBeneficiaryVMVmUseForTable();
                IQueryable<FxBeneficiaryVM> queryable = (from t in context.FXBeneficiaries
                                                         join u in context.TblUsers on t.UserID equals u.UserId
                                                         into posted from postedBy in posted.DefaultIfEmpty()
                                                         where t.CustomerID == CustomerID && t.IsDelete == false
                                                         orderby t.Name
                                                         select new FxBeneficiaryVM
                                                         {
                                                             ID = t.FxBenfID,
                                                             CustomerID = t.CustomerID,
                                                             Name = t.Name,
                                                             AccountNumber = t.AccountNumber,
                                                             Bank = t.Bank,
                                                             Branch = t.Branch,
                                                             SwiftCode = t.SwiftCode,
                                                             Reference = t.Reference,
                                                             Comment = t.Comment,
                                                             PostedBy=postedBy.Firstname+" "+postedBy.Lastname,
                                                             PostedByShortName=postedBy.ShortName,
                                                             DatePosted=t.DatePosted.ToString("dd-MMM-yyyy"),
                                                             Filenames = context.KYCTreasuryBeneficiaries.Where(x => x.TreasuryBeneficiariesId == t.FxBenfID && x.IsDeleted == false).Select(x => x.Path).ToList(),
                                                             CommentbyFilenames = context.KYCTreasuryBeneficiaries.Where(x => x.TreasuryBeneficiariesId == t.FxBenfID && x.IsDeleted == false).Select(x => new CommentbyFilenames { Id = x.Id, fileName = x.Path }).ToList()
                                                         }).AsQueryable();

                lstBeneficiaries.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstBeneficiaries.BeneficiaryList = await queryable.Paginate(pagination).ToListAsync();
                lstBeneficiaries.BeneficiaryListForsearch = queryable.ToList();
                lstBeneficiaries.TotalCount = queryable.Count();
                return lstBeneficiaries;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<FxBeneficiaryVM> GetBeneficiaryByID(int RowID)
        {
            try
            {
                FxBeneficiaryVM queryable = (from t in context.FXBeneficiaries
                                             where t.FxBenfID == RowID && t.IsDelete == false
                                             select new FxBeneficiaryVM
                                             {
                                                 ID = t.FxBenfID,
                                                 CustomerID = t.CustomerID,
                                                 Name = t.Name,
                                                 AccountNumber = t.AccountNumber,
                                                 Bank = t.Bank,
                                                 Branch = t.Branch,
                                                 SwiftCode = t.SwiftCode,
                                                 Reference = t.Reference,
                                                 Comment=t.Comment,
                                                 // Filenames = context.KYCTreasuryBeneficiaries.Where(x => x.TreasuryBeneficiariesId == t.FxBenfID && x.IsDeleted == false).Select(x => x.Path).ToList(),
                                                 CommentbyFilenames = context.KYCTreasuryBeneficiaries.Where(x => x.TreasuryBeneficiariesId == t.FxBenfID && x.IsDeleted == false).Select(x => new CommentbyFilenames { Id = x.Id, fileName = x.Path }).ToList()
                                             }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<FxBeneficiaryVM> GetBeneficiaryCommentByID(int RowID)
        {
            try
            {
                FxBeneficiaryVM queryable = (from t in context.FXBeneficiaries
                                             join u in context.TblUsers
                                             on t.UserID equals u.UserId
                                             join ur in context.UserRoles
                                             on t.UserID equals ur.UserId
                                             join r in context.Roles
                                             on ur.RoleId equals r.Id
                                             where t.FxBenfID == RowID && t.IsDelete == false
                                             select new FxBeneficiaryVM
                                             {
                                                 PostedBy = u.Firstname+" "+u.Lastname,
                                                 PostedByShortName=u.ShortName,
                                                 DatePosted=t.DatePosted.ToString("dd-MMM-yyyy hh:mm:tt"),
                                                 PostedRole=r.Name,
                                                 Comment = t.Comment,
                                             }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Exception> SaveFxBeneficiary(FxBeneficiaryVM beneficiary)
        {
            List<KYCTblTreasuryBeneficiaries> ListOfattachment = new List<KYCTblTreasuryBeneficiaries>();
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.FXBeneficiary record = new();
                    record.CustomerID = beneficiary.CustomerID;
                    record.Name = beneficiary.Name;
                    record.AccountNumber = beneficiary.AccountNumber;
                    record.Bank = beneficiary.Bank;
                    record.Branch = beneficiary.Branch;
                    record.SwiftCode = beneficiary.SwiftCode;
                    record.Reference = beneficiary.Reference;
                    record.Comment = beneficiary.Comment;
                    record.PostedBy = Userinfo.FullName;
                    record.UserID = Userinfo.UserId;
                    record.DatePosted = DateTime.Now.DateTime_UK();
                    context.FXBeneficiaries.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.SaveChangesAsync();
                    if (beneficiary.Filenames != null)
                    {
                        foreach (string item in beneficiary.Filenames)
                        {
                            KYCTblTreasuryBeneficiaries AddTblFgcattachment = new KYCTblTreasuryBeneficiaries
                            {
                                Path = item,
                                ClientId = beneficiary.CustomerID,
                                TreasuryBeneficiariesId = record.FxBenfID,
                                PostedBy = Userinfo.FullName,
                                PostedDate = DateTime.Now.DateTime_UK(),
                            };
                            ListOfattachment.Add(AddTblFgcattachment);
                        }
                        await context.KYCTreasuryBeneficiaries.AddRangeAsync(ListOfattachment);
                        await context.SaveChangesAsync();
                    }
                    int MaxRecordID = context.FXBeneficiaries.Max(x => x.FxBenfID);
                    transaction.Commit();
                    return new Ok(MaxRecordID.ToString());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<Exception> UpdateBeneficiary(FxBeneficiaryVM beneficiary, int FxBenfID)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<KYCTblTreasuryBeneficiaries> ListOfattachment = new List<KYCTblTreasuryBeneficiaries>();
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.FXBeneficiary record = context.FXBeneficiaries.Find(FxBenfID);
                    record.CustomerID = beneficiary.CustomerID;
                    record.Name = beneficiary.Name;
                    record.AccountNumber = beneficiary.AccountNumber;
                    record.Bank = beneficiary.Bank;
                    record.Branch = beneficiary.Branch;
                    record.SwiftCode = beneficiary.SwiftCode;
                    record.Reference = beneficiary.Reference;
                    record.Comment = beneficiary.Comment;
                    record.PostedBy = Userinfo.FullName;
                    record.UserID = Userinfo.UserId;
                    record.DatePosted = DateTime.Now.DateTime_UK();
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync();


                    if (beneficiary.Filenames != null)
                    {
                        foreach (string item in beneficiary.Filenames)
                        {
                            KYCTblTreasuryBeneficiaries AddTblFgcattachment = new KYCTblTreasuryBeneficiaries
                            {
                                Path = item,
                                ClientId = beneficiary.CustomerID,
                                TreasuryBeneficiariesId = record.FxBenfID,
                                PostedBy = Userinfo.FullName,
                                PostedDate = DateTime.Now.DateTime_UK(),
                            };
                            ListOfattachment.Add(AddTblFgcattachment);
                        }
                        await context.KYCTreasuryBeneficiaries.AddRangeAsync(ListOfattachment);
                        await context.SaveChangesAsync();
                    }

                    // int MaxRecordID = context.FXBeneficiaries.Max(x => x.FxBenfID);
                    transaction.Commit();
                    return new Ok(FxBenfID.ToString());
                    //  return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }


        public async Task<Exception> SaveClienUsers(ClientUsersVM userclient)
        {
            List<KYC_ClientUsers> ListOfattachment = new List<KYC_ClientUsers>();
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.KYC.ClientUsers record = new();
                    record.ClientId = userclient.ClientId;
                    record.ApplicationId = userclient.ApplicationId;
                    record.Name = userclient.Name;
                    record.Email = userclient.Email;
                    record.Address = userclient.Address;
                    record.Type = userclient.Type;
                    record.ContactNumber = userclient.ContactNumber;
                    record.PostedBy = Userinfo.FullName;
                    record.PostedById = Userinfo.UserId;
                    record.PostedDate = DateTime.Now.DateTime_UK();
                    context.ClientUsers.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.SaveChangesAsync();
                    if (userclient.Filenames != null)
                    {
                        if (userclient.Filenames.Count() > 0)
                        {
                            foreach (string item in userclient.Filenames)
                            {
                                KYC_ClientUsers AddTblFgcattachment = new KYC_ClientUsers
                                {
                                    Path = item,
                                    ClientId = userclient.ClientId,
                                    ApplicationId = userclient.ApplicationId,
                                    ClientUserId = record.Id,
                                    Type = 0,
                                    PostedBy = Userinfo.FullName,
                                    PostedById = Userinfo.UserId,
                                    PostedDate = DateTime.Now.DateTime_UK(),
                                };
                                ListOfattachment.Add(AddTblFgcattachment);
                            }
                            await context.KYC_ClientUsers.AddRangeAsync(ListOfattachment);
                            await context.SaveChangesAsync();
                        }

                    }

                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }


        public async Task<Exception> UpdateUserClient(ClientUsersVM userclient, int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    List<KYC_ClientUsers> ListOfattachment = new List<KYC_ClientUsers>();
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.KYC.ClientUsers record = context.ClientUsers.Find(Id);

                    record.ClientId = userclient.ClientId;
                    record.ApplicationId = userclient.ApplicationId;
                    record.Name = userclient.Name;
                    record.Email = userclient.Email;
                    record.Address = userclient.Address;
                    record.Type = userclient.Type;
                    record.ContactNumber = userclient.ContactNumber;
                    record.PostedBy = Userinfo.FullName;
                    record.PostedDate = DateTime.Now.DateTime_UK();
                    record.PostedById = Userinfo.UserId;
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    if (userclient.Filenames != null)
                    {
                        foreach (string item in userclient.Filenames)
                        {
                            KYC_ClientUsers AddTblFgcattachment = new KYC_ClientUsers
                            {
                                Path = item,
                                ClientId = userclient.ClientId,
                                ApplicationId = userclient.ApplicationId,
                                ClientUserId = record.Id,
                                Type = 0,
                                PostedBy = Userinfo.FullName,
                                PostedDate = DateTime.Now.DateTime_UK(),
                            };
                            ListOfattachment.Add(AddTblFgcattachment);
                        }
                        await context.KYC_ClientUsers.AddRangeAsync(ListOfattachment);
                        await context.SaveChangesAsync();
                    }
                    // int MaxRecordID = context.FXBeneficiaries.Max(x => x.FxBenfID);
                    transaction.Commit();
                    return new Ok("2");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<ClientUsersVM> GetUserClientByID(int RowID)
        {
            try
            {
                ClientUsersVM queryable = (from t in context.ClientUsers
                                           where t.Id == RowID
                                           select new ClientUsersVM
                                           {
                                               Id = t.Id,
                                               ClientId = t.ClientId,
                                               ApplicationId = t.ApplicationId,
                                               Type = t.Type,
                                               Name = t.Name,
                                               Email = t.Email,
                                               Address = t.Address,
                                               ContactNumber = t.ContactNumber,
                                               PostedBy = t.PostedBy,
                                               PostedDate = t.PostedDate,
                                               // Filenames = context.KYCTreasuryBeneficiaries.Where(x => x.TreasuryBeneficiariesId == t.FxBenfID && x.IsDeleted == false).Select(x => x.Path).ToList(),
                                               CommentbyFilenames = context.KYC_ClientUsers.Where(x => x.ClientUserId == t.Id && x.IsDelete==false && x.IsArchive==false).Select(x => new CommentbyFilenames { Id = x.Id, fileName = x.Path }).ToList()
                                           }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }



        public async Task<ClientUsersVMUseForTable> GetUserClients([FromQuery] PaginationDTO pagination, int ClientId)
        {
            try
            {
                ClientUsersVMUseForTable LisOfUserClients = new ClientUsersVMUseForTable();
                IQueryable<ClientUsersVM> queryable = (from t in context.ClientUsers
                                                       join u in context.TblUsers on t.PostedById equals u.UserId
                                                       into posted from postedBy in posted.DefaultIfEmpty()
                                                       where t.ClientId == ClientId
                                                       select new ClientUsersVM
                                                       {
                                                           Id = t.Id,
                                                           Type = t.Type,
                                                           Name = t.Name,
                                                           Address = t.Address,
                                                           Email = t.Email,
                                                           ContactNumber = t.ContactNumber,
                                                           PostedBy = postedBy.Firstname+" "+postedBy.Lastname,
                                                           PostedByShortName=postedBy.ShortName,
                                                           PostedDate = t.PostedDate,
                                                           Filenames = context.KYC_ClientUsers.Where(x => x.ClientUserId == t.Id).Select(x => x.Path).ToList(),
                                                           CommentbyFilenames = context.KYC_ClientUsers.Where(x => x.ClientUserId == t.Id).Select(x => new CommentbyFilenames { Id = x.Id, fileName = x.Path }).ToList(),
                                                           IsInActive=t.IsDelete
                                                       }).OrderBy(s=>s.IsInActive).AsQueryable();
                LisOfUserClients.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                LisOfUserClients.ClientUsersList = await queryable.Paginate(pagination).ToListAsync();
                LisOfUserClients.ClientUsersListForsearch = queryable.ToList();
                LisOfUserClients.TotalCount = queryable.Count();
                return LisOfUserClients;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ClientUsersVMUseForTable> GetUserClientsOnBoarding([FromQuery] PaginationDTO pagination, int ApplicationId)
        {
            try
            {
                ClientUsersVMUseForTable LisOfUserClients = new ClientUsersVMUseForTable();
                IQueryable<ClientUsersVM> queryable = (from t in context.ClientUsers
                                                       join u in context.TblUsers on t.PostedById equals u.UserId
                                                       into posted from postedBy in posted.DefaultIfEmpty()
                                                       where t.ApplicationId == ApplicationId
                                                       select new ClientUsersVM
                                                       {
                                                           Id = t.Id,
                                                           ApplicationId = t.ApplicationId,
                                                           Type = t.Type,
                                                           Name = t.Name,
                                                           Address = t.Address,
                                                           Email = t.Email,
                                                           ContactNumber = t.ContactNumber,
                                                           PostedBy = postedBy.Firstname+" "+postedBy.Lastname,
                                                           PostedByShortName=postedBy.ShortName,
                                                           PostedDate = t.PostedDate,
                                                           Filenames = context.KYC_ClientUsers.Where(x => x.ClientUserId == t.Id && x.IsDelete==false && x.IsArchive==false).Select(x => x.Path).ToList(),
                                                           CommentbyFilenames = context.KYC_ClientUsers.Where(x => x.ClientUserId == t.Id && x.IsDelete==false && x.IsArchive==false).Select(x => new CommentbyFilenames { Id = x.Id, fileName = x.Path }).ToList(),
                                                           IsInActive=t.IsDelete
                                                       }).OrderBy(s=>s.IsInActive).AsQueryable();
                LisOfUserClients.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                LisOfUserClients.ClientUsersList = await queryable.Paginate(pagination).ToListAsync();
                LisOfUserClients.ClientUsersListForsearch = queryable.ToList();
                LisOfUserClients.TotalCount = queryable.Count();
                return LisOfUserClients;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool DeleteClientApplicationDocuments(int Id)
        {
            try
            {
                var DetaRecord = (from s in context.ClientApplicationDocuments
                                  where s.Id == Id
                                  select s).FirstOrDefault();
                if (DetaRecord != null)
                {
                    DetaRecord.IsDelete = true;
                    //context.ClientApplicationDocuments.Remove(DetaRecord);
                    CurrentUserInfoVM Userinfo = UserauthenticationStateProvider.CurrentUser().ConfigureAwait(false).GetAwaiter().GetResult();
                    var ClientName = GetClientName(DetaRecord.ClientId);
                    var FGCLog = new TblFgclog
                    {
                        Object = "Customers (Client)",
                        Action = "Client User Application Documents deleted",
                        Remarks = $"Client User profile of  <span style='font-weight:bold; margin-right:5px;'>({ClientName}) </span>  has deleted. Client User Application Documents deleted",
                        IP = FGCExtensions.GetIpAddress(),
                        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                        UserId = Userinfo.UserId,
                        DatePosted = DateTime.Now.DateTime_UK()
                    };
                    context.TblFgclogs.Add(FGCLog);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool DeleteClientUserDocuments(int Id)
        {
            try
            {
                var DetaRecord = (from s in context.KYC_ClientUsers
                                  where s.Id == Id
                                  select s).FirstOrDefault();
                if (DetaRecord != null)
                {
                    DetaRecord.IsDelete = true;
                    //context.KYC_ClientUsers.Remove(DetaRecord);
                    CurrentUserInfoVM Userinfo = UserauthenticationStateProvider.CurrentUser().ConfigureAwait(false).GetAwaiter().GetResult();
                    var ClientName = GetClientName(DetaRecord.ClientId);
                    var FGCLog = new TblFgclog
                    {
                        Object = "Customers (Client)",
                        Action = "Client User Application Documents deleted",
                        Remarks = $"Client User profile of  <span style='font-weight:bold; margin-right:5px;'>({ClientName}) </span>  has deleted. Client User Application Documents deleted",
                        IP = FGCExtensions.GetIpAddress(),
                        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                        UserId = Userinfo.UserId,
                        DatePosted = DateTime.Now.DateTime_UK()
                    };
                    context.TblFgclogs.Add(FGCLog);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<bool> DeleteClientUserDocumentsOnBoarding(int Id, int option)
        {
            try
            {
                var DetaRecord =await  (from s in context.KYC_ClientUsers
                                  where s.Id == Id
                                  select s).FirstOrDefaultAsync();
                string Action = string.Empty, Remarks = string.Empty;
                var ClientName = GetBusinessName(DetaRecord.ApplicationId);
                if (option == 1)
                {
                    DetaRecord.IsArchive = true;
                    Action = "Client User Document Archived";
                    Remarks = "Client User document named " + DetaRecord.Path + " of the client '" + ClientName + "' moved to archive folder";

                }
                else if (option == 2)
                {
                    DetaRecord.IsDelete = true;
                    Action = "Client User Document Deleted";
                    Remarks = "Client User  document named " + DetaRecord.Path + " of the client '" + ClientName + "' deleted";

                }
                if (DetaRecord != null)
                {
                    CurrentUserInfoVM Userinfo = UserauthenticationStateProvider.CurrentUser().ConfigureAwait(false).GetAwaiter().GetResult();
                    var FGCLog = new TblFgclog
                    {
                        Object = "Client Users (Client)",
                        Action = Action,
                        Remarks = Remarks,
                        IP = FGCExtensions.GetIpAddress(),
                        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                        UserId = Userinfo.UserId,
                        DatePosted = DateTime.Now.DateTime_UK()
                    };
                    context.TblFgclogs.Add(FGCLog);
                    context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public async Task<Exception> DeleteClientUser(int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.KYC.ClientUsers clientUser = context.ClientUsers.Find(Id);
                    if (clientUser != null)
                    {
                        clientUser.IsDelete = true;
                        CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                        var FGCLog = new TblFgclog
                        {
                            Object = "Client Profile (Client User)",
                            Action = "Client user deleted",
                            Remarks = $"Client user deleted <b>({clientUser.Name})</b>",
                            IP = FGCExtensions.GetIpAddress(),
                            PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                            UserId = Userinfo.UserId,
                            DatePosted = DateTime.Now.DateTime_UK()
                        };
                        context.TblFgclogs.Add(FGCLog);
                        await context.SaveChangesAsync();
                        transaction.Commit();
                    }
                    return new Ok("3");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public string GetClientName(int? ClientId)
        {

            return context.TblClients.Where(x => x.ClientId == ClientId && string.IsNullOrEmpty(x.IsDeleted)).Select(x => x.Name).FirstOrDefault();
        }
        public string GetBusinessName(int? applicationId)
        {

            return context.BusinessProfiles.Where(x => x.Id == applicationId).Select(x => x.BusinessName).FirstOrDefault();
        }



        public async Task<Exception> DeleteBeneficiary(int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXBeneficiary Beneficiary = context.FXBeneficiaries.Find(Id);
                    if (Beneficiary != null)
                        Beneficiary.IsDelete = true;

                    if (Beneficiary != null)
                    {
                        var ListOfKcy = await context.KYCTreasuryBeneficiaries.Where(x => x.TreasuryBeneficiariesId == Beneficiary.FxBenfID).ToListAsync();
                        ListOfKcy.ForEach(x => x.IsDeleted = true);
                    }


                    //  context.FXBeneficiaries.Remove(Beneficiary);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> DeleteBeneficiaryFile(int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                   var file= await context.KYCTreasuryBeneficiaries.Where(x => x.Id == Id).FirstOrDefaultAsync();
                    if (file != null)
                    {
                        file.IsDeleted = true;
                    }
                    CurrentUserInfoVM Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    var FGCLog = new TblFgclog
                    {
                        Object = "Client Profile (Treasury Benf)",
                        Action = "Treasury Benf file deleted",
                        Remarks = $"Treasury Benf file deleted <b>({file.Path})</b>",
                        IP = FGCExtensions.GetIpAddress(),
                        PostedBy = Userinfo.FullName + $" ({Userinfo.Role})",
                        UserId = Userinfo.UserId,
                        DatePosted = DateTime.Now.DateTime_UK()
                    };
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        // Customers Management Events
        public async Task<FxClientEmailsVMUseForTable> GetFxCustomers([FromQuery] PaginationDTO pagination, int ClientID)
        {
            try
            {
                FxClientEmailsVMUseForTable lstCustomerEmails = new FxClientEmailsVMUseForTable();
                IQueryable<FxClientEmailsVM> queryable = (from t in context.ClientEmails
                                                          where t.ClientID == ClientID
                                                          orderby t.Email
                                                          select new FxClientEmailsVM
                                                          {
                                                              ID = t.ID,
                                                              ClientID = t.ClientID,
                                                              Emails = t.Email

                                                          }).AsQueryable();
                lstCustomerEmails.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstCustomerEmails.CustomersEmailsList = await queryable.Paginate(pagination).ToListAsync();
                lstCustomerEmails.CustomersEmailsListForsearch = queryable.ToList();
                lstCustomerEmails.TotalCount = queryable.Count();
                return lstCustomerEmails;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<FxClientEmailsVM> GetCustomerByID(int RowID)
        {
            try
            {
                FxClientEmailsVM queryable = (from t in context.ClientEmails
                                              where t.ID == RowID
                                              select new FxClientEmailsVM
                                              {
                                                  ID = t.ID,
                                                  ClientID = t.ClientID,
                                                  Emails = t.Email

                                              }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool CheckDuplicateEmail(FxClientEmailsVM customerEmail)
        {
            try
            {
                var IsEmailExist = context.ClientEmails.Where(x => x.Email == customerEmail.Emails && x.ClientID == customerEmail.ClientID).FirstOrDefault();
                if (IsEmailExist != null)
                    return true;
                else
                    return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Exception> SaveFxCustomers(FxClientEmailsVM customerEmail)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    //Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.ClientEmails record = new();
                    record.ClientID = customerEmail.ClientID;
                    record.Email = customerEmail.Emails;
                    context.ClientEmails.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.SaveChangesAsync();
                    // int MaxRecordID = context.FxCustomers.Max(x => x.CustomerID);
                    transaction.Commit();
                    return new Ok("1");
                    // return new Ok(MaxRecordID.ToString());
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<Exception> UpdateFxCustomer(FxClientEmailsVM email, int ID)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Userinfo = await UserauthenticationStateProvider.CurrentUser();
                    Models.ClientEmails record = context.ClientEmails.Find(ID);
                    record.Email = email.Emails;
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> DeleteFxCustomer(int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.ClientEmails email = context.ClientEmails.Find(Id);
                    context.ClientEmails.Remove(email);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        // End Customer Management Events
        public async Task<FxProviderVMVmUseForTable> GetProviders([FromQuery] PaginationDTO pagination)
        {
            try
            {
                FxProviderVMVmUseForTable lstProviders = new FxProviderVMVmUseForTable();
                IQueryable<FxProviderVM> queryable = (from t in context.FXProviders
                                                      where t.IsDelete == false
                                                      orderby t.Name
                                                      select new FxProviderVM
                                                      {
                                                          ID = t.FXProviderID,
                                                          ProviderName = t.Name

                                                      }).AsQueryable();
                lstProviders.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstProviders.ProvidersList = await queryable.Paginate(pagination).ToListAsync();
                lstProviders.ProvidersListForsearch = queryable.ToList();
                lstProviders.TotalCount = queryable.Count();
                return lstProviders;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<FxProviderVM> GetProviderByID(int RowID)
        {
            try
            {
                FxProviderVM queryable = (from t in context.FXProviders
                                          where t.FXProviderID == RowID && t.IsDelete == false
                                          select new FxProviderVM
                                          {
                                              ID = t.FXProviderID,
                                              ProviderName = t.Name

                                          }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Exception> SaveFxProvider(FxProviderVM provider)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXProviders record = new();
                    record.Name = provider.ProviderName;
                    context.FXProviders.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> UpdateFxProvider(FxProviderVM provider, int ProviderID)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXProviders record = context.FXProviders.Find(ProviderID);
                    record.Name = provider.ProviderName;
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> DeleteFxProvider(int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXProviders provider = context.FXProviders.Find(Id);
                    // context.FXProviders.Remove(provider);
                    if (provider != null)
                        provider.IsDelete = true;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }

        public async Task<CurrencyPairVMVmUseForTable> GetCurrencyPairs([FromQuery] PaginationDTO pagination)
        {
            try
            {
                CurrencyPairVMVmUseForTable lstCurrencyPair = new CurrencyPairVMVmUseForTable();
                IQueryable<CurrencyPairVM> queryable = (from t in context.FXCurrencyPairs
                                                        where t.IsDelete == false
                                                        orderby t.ID descending
                                                        select new CurrencyPairVM
                                                        {
                                                            ID = t.ID,
                                                            CurrencyPair = t.CurrencyPair,
                                                            BaseCurrency = t.SourceCurrency,
                                                            QuoteCurrency = t.DestinationCurrency

                                                        }).AsQueryable();
                lstCurrencyPair.TotalPages = await HttpContext.InsertPaginationParameterInResponse(queryable, pagination.QuantityPerPage);
                lstCurrencyPair.CurrencyPairList = await queryable.Paginate(pagination).ToListAsync();
                lstCurrencyPair.CurrencyPairForsearch = queryable.ToList();
                lstCurrencyPair.TotalCount = queryable.Count();
                return lstCurrencyPair;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<CurrencyPairVM> GetCurrencyPairID(int RowID)
        {
            try
            {
                CurrencyPairVM queryable = (from t in context.FXCurrencyPairs
                                            where t.ID == RowID && t.IsDelete == false
                                            select new CurrencyPairVM
                                            {
                                                ID = t.ID,
                                                CurrencyPair = t.CurrencyPair,
                                                BaseCurrency = t.SourceCurrency,
                                                QuoteCurrency = t.DestinationCurrency

                                            }).FirstOrDefault();

                return queryable;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Exception> SaveFxCurrecnyPair(CurrencyPairVM pair)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXCurrencyPair record = new();
                    record.CurrencyPair = pair.CurrencyPair;
                    record.SourceCurrency = pair.BaseCurrency;
                    record.DestinationCurrency = pair.QuoteCurrency;
                    context.FXCurrencyPairs.Add(record);
                    context.Entry(record).State = EntityState.Added;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> UpdateFxCurrencyPair(CurrencyPairVM pair, int PairID)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXCurrencyPair record = context.FXCurrencyPairs.Find(PairID);
                    record.CurrencyPair = pair.CurrencyPair;
                    record.SourceCurrency = pair.BaseCurrency;
                    record.DestinationCurrency = pair.QuoteCurrency;
                    context.Entry(record).State = EntityState.Modified;
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public async Task<Exception> DeleteCurrencyPair(int Id)
        {
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = context.Database.BeginTransaction())
            {
                try
                {
                    Models.FXCurrencyPair currencypair = context.FXCurrencyPairs.Find(Id);
                    context.FXCurrencyPairs.Remove(currencypair);
                    await context.SaveChangesAsync();
                    transaction.Commit();
                    return new Ok("1");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new BadRequestException(ex.Message.ToString());
                }
            }
        }
        public string PopulateBody(FXTransactionsVM t)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(Path.Combine(templateFolder, "email.html")))
            {
                body = reader.ReadToEnd();
            }
            int BenfID = 0;
            if (t.FxBenfID != null)
                BenfID = Convert.ToInt32(t.FxBenfID);
            int CustomerID = 0;
            if (t.ClientID != null)
                CustomerID = Convert.ToInt32(t.ClientID);
            decimal Fee = 0;
            if (t.ProfitFee != null)
                Fee = Convert.ToDecimal(t.ProfitFee);
            // Get Benefciary Info
            FXBeneficiary fxBeneficiary = context.FXBeneficiaries.Where(b => b.FxBenfID == BenfID && b.IsDelete == false).FirstOrDefault();
            TblClient fxCustomer = context.TblClients.Where(c => c.ClientId == CustomerID && c.IsDeleted != "Yes").FirstOrDefault();
            if (Fee < 0)
                Fee = 0;

            body = body.Replace("{#FGCRef}", t.ConfirmationReference);
            body = body.Replace("{#Date1}", t.DealDate?.DateTime_UK().ToString("dd-MM-yyyy hh:mm"));
            body = body.Replace("{#AccountNo}", t.ClientAccount);
            body = body.Replace("{#SecondCurrency}", t.DestinationCurrency);
            body = body.Replace("{#BaseCurrency}", t.SourceCurrency);
            body = body.Replace("{#Ref}", t.DealReference);
            body = body.Replace("{#Amount2}", t.Ct_Amount2 == null ? "" : Convert.ToDecimal(t.Ct_Amount2).ToString("N"));
            body = body.Replace("{#BookingRate}", t.ClientRate == null ? "" : t.ClientRate.ToString());
            body = body.Replace("{#Amount1}", t.Ct_Amount1 == null ? "" : Convert.ToDecimal(t.Ct_Amount1).ToString("N"));
            body = body.Replace("{#Fee}", t.Ct_TxCharges);
            body = body.Replace("{#Total}", t.Ct_Total == null ? "" : Convert.ToDecimal(t.Ct_Total).ToString("N"));
            if (fxBeneficiary != null)
            {
                body = body.Replace("{#Bname}", fxBeneficiary.Name);
                body = body.Replace("{#BAccount}", fxBeneficiary.AccountNumber);
                body = body.Replace("{#Bankname}", fxBeneficiary.Bank);
                body = body.Replace("{#Baddress}", fxBeneficiary.Branch);
                body = body.Replace("{#SwiftCode}", fxBeneficiary.SwiftCode);
                body = body.Replace("{#Bref}", fxBeneficiary.Reference);
            }

            body = body.Replace("{#Sname}", t.ClientName);
            body = body.Replace("{#SAccount}", t.ClientAccount);
            if (fxCustomer != null)
            {
                body = body.Replace("{#Saddress}", fxCustomer.Address);
                body = body.Replace("{#City}", fxCustomer.City);
                body = body.Replace("{#PostCode}", fxCustomer.PostalCode);
                body = body.Replace("{#Country}", fxCustomer.Country);
            }

            return body;
        }
        public async Task<IActionResult> CreatePDFAsync(string htmlContent, string filename)
        {

            string path = Path.Combine(rootFolder, filename);
            GlobalSettings globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 20, Bottom = 20, Left = 10 },
                DocumentTitle = "FxTransaction",
                Out = path  //USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            ObjectSettings objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = htmlContent,
                //Page = "~/PdfTemplate/ViewPdf/?ApplicationId=" + ApplicationId, //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { DefaultEncoding = "utf-8", MinimumFontSize = 12 },
                //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = false },
                FooterSettings = { FontName = "Lucida Sans", FontSize = 9, Right = "Page [page]" }
            };
            HtmlToPdfDocument pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            //_converter.Convert(pdf); IF WE USE Out PROPERTY IN THE GlobalSettings CLASS, THIS IS ENOUGH FOR CONVERSION
            byte[] file = _converter.Convert(pdf);
            //return Ok("Successfully created PDF document.");
            // return File(file, "application/pdf", "FxTransaction.pdf");
            return File(file, "application/pdf");

        }


        public async Task<List<CommentbyFilenames>> GetKycTransactionBeneficiary(int FxBenfID)
        {
            List<CommentbyFilenames> listofPrclog = new List<CommentbyFilenames>();
            try
            {
                // List<CommentbyFilenames> ListOfCommentFilenames
                listofPrclog = await (from c in context.KYCTreasuryBeneficiaries
                                      where c.TreasuryBeneficiariesId == FxBenfID && c.IsDeleted==false
                                      select new CommentbyFilenames
                                      {
                                          Id = c.Id,
                                          fileName = c.Path,
                                          ClientId = c.ClientId
                                      }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return listofPrclog;
        }

        public async Task<List<CommentbyFilenames>> GetClientApplicationDocuments(int ClientUserId)
        {
            List<CommentbyFilenames> listofPrclog = new List<CommentbyFilenames>();
            try
            {
                // List<CommentbyFilenames> ListOfCommentFilenames
                listofPrclog = await (from c in context.KYC_ClientUsers
                                      where c.ClientUserId == ClientUserId && c.IsDelete==false && c.IsArchive==false
                                      select new CommentbyFilenames
                                      {
                                          Id = c.Id,
                                          fileName = c.Path,
                                      }).ToListAsync();
            }
            catch (Exception)
            {
                return null;
            }
            return listofPrclog;
        }



    }
}
