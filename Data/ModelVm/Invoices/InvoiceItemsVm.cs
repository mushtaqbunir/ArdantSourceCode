using ArdantOffical.Data.ModelVm.OT;
using ArdantOffical.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.Invoices
{
    public class InvoiceItemsVm
    {
        public string Id { get; set; }
        public string InvoiceId { get; set; }
        public int SNo { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public DateTime? Date { get; set; } = DateTime.Now;
        public decimal? Hours { get; set; }
        public decimal? Minutes { get; set; }
        public decimal? KM { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerSubUrb { get; set; }
        public string CustomerPostCode { get; set; }
        public DateTime? InvoiceDate { get; set; }
        public string InvoiceStatus { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? NetAmount { get; set; }
        public string QuantityTime
        {
            get
            {
                if (Hours.HasValue && Minutes.HasValue)
                {
                    int totalMinutes = (int)((Hours.Value * 60) + Minutes.Value);
                    return $"{totalMinutes} minutes";
                }
                else
                {
                    return "N/A";
                }
            }
        }

        // UOQ is unit of quantity
        public string UOQ { get; set; }
        // UOM is unit of measurement
        public string UOM { get; set; }
        public decimal? UnitPrice { get; set; }
        public string Unit { get; set; }
        public decimal? Total { get; set; }
        public string Rate { get; set; } = "193.99";
        [Required(ErrorMessage = "Please select tax option")]
        public string Tax { get; set; } = "0.0";
        public decimal TaxAmount
        {
            get
            {
                return Amount != null && Tax !=null ? Amount.Value * Convert.ToDecimal(Tax) / 100 : 0;
            }
        }

        public decimal TotalAmount {
            get
            {
                return TaxAmount + Amount.Value;
            }
        }
        public decimal? Amount
        {
            get
            {
                if (Rate != null)
                {
                    int TotalTime = 0;
                    decimal? PerMinuteRate = Convert.ToDecimal(Rate) / 60;
                    decimal KmRate = KM !=null ? KM.Value * Convert.ToDecimal(0.95) : 0;
                    if (Hours.HasValue && Minutes.HasValue)
                    {
                        TotalTime = (int)((Hours.Value * 60) + Minutes.Value);
                        return  Math.Round((decimal)((PerMinuteRate * TotalTime)+KmRate), 2);
                    } else if(!Hours.HasValue && Minutes.HasValue)
                    {
                        TotalTime = (int)(Minutes.Value);
                        return Math.Round((decimal)((PerMinuteRate * TotalTime) + KmRate), 2);
                    } else if (Hours.HasValue && !Minutes.HasValue)
                    {
                        TotalTime = (int)((Hours.Value * 60));
                        return Math.Round((decimal)((PerMinuteRate * TotalTime) + KmRate), 2);
                    } else    
                    {
                        return 0;
                    }
                    
                } else
                {
                    return 0;
                }

                   
               
            }
        }

        public List<InvoiceItemsVm> InvoiceItemList { get; set; }
    }
}
