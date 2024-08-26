using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArdantOffical.Models
{
    public class MenuItem
    {
        [Key]
        public int MenuItemID { get; set; }
        public string MenuName { get; set; }
        public string ActionLink { get; set; }
        public string Icons { get; set; }
        public bool IsParent { get; set; }
        public bool IsDelete { get; set; }
        public int? Level { get; set; }
        public string Type { get; set; }

        public int? MenuItemParentID { get; set; }
        [ForeignKey("MenuItemParentID")]
        public MenuItem MenuItems { get; set; }
        public virtual ICollection<MenuItem> MenuItemChild { get; set; }
        public int SortOrder { get; set; }
    }
}
