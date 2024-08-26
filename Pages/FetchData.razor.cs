using ArdantOffical.Helpers;

namespace ArdantOffical.Pages
{
    public partial class FetchData
    {
        public string Activeclass1 { get; set; } = "active";
        public string Activeclass2 { get; set; }
        public string Activeclass3 { get; set; }
        public string Activeclass4 { get; set; }
        //js.Wizardnextstep();
        public void ActiveClassChange(int type)
        {
            js.Wizardnextstep();
            if (type == 1)
            {
                Activeclass1 = "active";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
            else if (type == 2)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "active";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
            else if (type == 3)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "disabled";
                Activeclass3 = "active";
                Activeclass4 = "disabled";
            }
            else if (type == 4)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "active";
            }
            else
            {
                Activeclass1 = "active";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
        }
        public void prevstep(int type)
        {
            js.prevstep();
            if (type == 1)
            {
                Activeclass1 = "active";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
            else if (type == 2)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "active";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
            else if (type == 3)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "disabled";
                Activeclass3 = "active";
                Activeclass4 = "disabled";
            }
            else if (type == 4)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "active";
            }
            else
            {
                Activeclass1 = "active";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
        }
        public void ActiveClick(int type)
        {

            if (type == 1)
            {
                Activeclass1 = "active";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
            else if (type == 2)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "active";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
            else if (type == 3)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "disabled";
                Activeclass3 = "active";
                Activeclass4 = "disabled";
            }
            else if (type == 4)
            {
                Activeclass1 = "disabled";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "active";
            }
            else
            {
                Activeclass1 = "active";
                Activeclass2 = "disabled";
                Activeclass3 = "disabled";
                Activeclass4 = "disabled";
            }
        }
    }
}
