using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArdantOffical.Data.ModelVm.Users
{
    public class MenuItemVM
    {
        public int MenuItemID { get; set; }
        [Required(ErrorMessage = "Menu Name is required!")]
        public string MenuName { get; set; }
        public string ActionLink { get; set; }
        public string Icons { get; set; }
        public bool IsParent { get; set; }
        public bool IsDelete { get; set; }
        public int Level { get; set; }
        public int? MenuItemParentID { get; set; } = 0;
        public int? MenuItemChildId { get; set; }
        public int? MenuItemGrantParentId { get; set; }
        public int UserId { get; set; }
        public List<string> ListOfMenuItemNames { get; set; }
        public MenuItemChildrenList ListOfChildMenuItemNames { get; set; }
        [Required(ErrorMessage = "Menu Item Type required!")]
        public string MenuItemType { get; set; }


    }

    public class MenuItemUseForTable
    {
        public List<MenuItemVM> MenuItemList { get; set; }

        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    public class MenuItemChildrenList
    {

        public string MenuItemParentName { get; set; }
        public List<string> ChildMenuName { get; set; }

    }
    public class MenuItemFormModel
    {

        public string MenuItemName { get; set; }
        public int UserId { get; set; }
        public int? MenuItemParentID { get; set; }
        public bool IsDelete { get; set; }
        public List<ListOfMenuItemFormModel> ListOfMenuItems { get; set; }


    }
    public class ListOfMenuItemFormModel
    {
        public int? MenuItemParentID { get; set; }
        public string MenuName { get; set; }
        public bool AllChecked { get; set; }

    }



    public class RemoveAllCheckedAndUncheckedModel
    {

        public int? ParentID { get; set; }
        public string MenuName { get; set; }
        public int Counter { get; set; } = 0;

    }
}
