using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace ArdantOffical.Data.ModelVm
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 2;
        public int CurrentPage { get; set; } = 1;
    }
    public static class PaginationData
    {
        public static List<SelectListItem> ListOfTablePages = new List<SelectListItem>() {
            new SelectListItem() { Text = "25", Value = "25" },
            new SelectListItem() { Text = "50", Value = "50" },
            new SelectListItem() { Text = "75", Value = "75" },
            new SelectListItem() { Text = "100", Value = "100"},
            new SelectListItem() { Text = "200", Value = "200"}
        };

    }
}