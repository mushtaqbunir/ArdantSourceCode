using ArdantOffical.Components.Users;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArdantOffical.Components
{
    //public partial class Select2
    //{
    //}
    public partial class Select2<CMPReportsVm> : InputBase<CMPReportsVm>
    {
        protected AddUser SiteView;
        [Parameter]
        public int TabIndex { get; set; }
        [Parameter]
        public string Id { get; set; }
        //[Inject]
        //public IJSRuntime js { get; set; }
        [Parameter]
        public bool Readonly { get; set; } = true;

        [Parameter]
        public string ModalId { get; set; }

        [Parameter]
        public IEnumerable<SelectListItem> Datasource
        {
            get => _datasource;
            set => _datasource = value;
        }
        private IEnumerable<SelectListItem> _datasource;

        [Parameter]
        public EventCallback<bool> OnAddSucces { get; set; }

        [Parameter]
        public EventCallback<string> OnSendClientID { get; set; }
        [Parameter]
        public string SelectName { get; set; }

        //[Inject]
        //IJSRuntime JSRuntime
        //{
        //    get; set;
        //}

        public DotNetObjectReference<SelectListItem> DotNetRefadduser;
        public DotNetObjectReference<Select2<CMPReportsVm>> DotNetRef;
        protected override bool TryParseValueFromString(string value, out CMPReportsVm result, out string validationErrorMessage)
        {

            try
            {
                if (Nullable.GetUnderlyingType(typeof(CMPReportsVm)) != null)
                {
                    result = (CMPReportsVm)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(CMPReportsVm)));
                }
                else
                {
                    result = (CMPReportsVm)Convert.ChangeType(value, typeof(CMPReportsVm));
                }
                validationErrorMessage = null;
                return true;
            }
            catch (Exception ex)
            {
                validationErrorMessage = ex.ToString();
            }

            throw new InvalidOperationException($"{GetType()} does not support the type '{typeof(CMPReportsVm)}'.");
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            DotNetRef = DotNetObjectReference.Create(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await js.InvokeVoidAsync("select2Component.init", Id, ModalId);
                await js.InvokeVoidAsync("select2Component.onChange", Id, DotNetRef, "Change_SelectWithFilterBase");
            }
        }
        protected override async Task OnParametersSetAsync()
        {

        }
        [JSInvokable("Change_SelectWithFilterBase")]
        public void Change(string value, string key = null)
        {
            try
            {
                string ClientId = "";
                // SiteView.OnCnicUserChange(value);
                if (string.IsNullOrWhiteSpace(value) || value.Contains("Select a"))
                {
                    Value = default;
                    ValueChanged.InvokeAsync(Value);
                }
                else
                {
                    CMPReportsVm temp;
                    if (Nullable.GetUnderlyingType(typeof(CMPReportsVm)) != null)
                    {
                        temp = (CMPReportsVm)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(CMPReportsVm)));
                        //SiteView.OnCnicUserChange(value);
                    }
                    else
                    {
                        temp = (CMPReportsVm)Convert.ChangeType(value, typeof(CMPReportsVm));
                        // SiteView.OnCnicUserChange(value);
                    }
                    if (!(Value?.Equals(temp)).GetValueOrDefault())
                    {
                        Value = temp;

                        // SiteView.OnCnicUserChange(value);
                        ValueChanged.InvokeAsync(Value);
                    }
                    ClientId = value.ToString();
                }
                // OnAddSucces.InvokeAsync(true);
                OnSendClientID.InvokeAsync(ClientId);
            }
            catch (Exception)
            {

            }

            StateHasChanged();
        }



    }
}
