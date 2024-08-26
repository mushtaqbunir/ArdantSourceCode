
//function MyImageUploader() {
//    debugger
//    $('.uploaded-image').imageUploader();
//}
//window.SetFocusToElement = (element) => {
//    debugger
//    element.focus();
//};
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}
function printDiv() {
    var divContents = document.getElementById("PrintDiv").innerHTML;
    var mywindow = window.open('', 'new div', 'height=500,width=600');
    mywindow.document.write('<html><head><title></title>');
    mywindow.document.write('<link href="/css/print.css" media="print" rel="stylesheet" />');
    mywindow.document.write('</head><body >');
    mywindow.document.write(divContents);
    mywindow.document.write('</body></html>');
    mywindow.document.close();
    mywindow.focus();
    setTimeout(function () { mywindow.print(); }, 1000);
    //  mywindow.close();
    return true;
}
function CustomConfirm(title, message, type) {
    return new Promise((resolve) => {
        Swal.fire({
            title: title,
            text: message,
            type: type,
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Confirm Operation'
        }).then((result) => {
            if (result.value) {
                resolve(true);
            } else {
                resolve(false);
            }
        });
    });
}
function ShowToast() {
    debugger
    $("#myToast").toast({
        delay: 3000
    });
    // Show toast
    /*$("#myToast").toast("show");*/
}
function gotoReview(event) {
    debugger
    event.preventDefault();
    var href = event.currentTarget.getAttribute('href')
    window.location = href;
    //$("#Payments").addClass("nav-item nav-item menu-is-opening menu-open");
    //$("#Payments").css("display", "block");
    ////$("#ComplaintGrid").removeClass("col-lg-5 col-md-5 col-sm-12");
}

function gotoReviewGO(event) {
    debugger
    event.preventDefault();
    $("body").addClass("nav-item nav-item menu-is-opening menu-open");
    //$("#Payments").css("display", "block");
    ////$("#ComplaintGrid").removeClass("col-lg-5 col-md-5 col-sm-12");
}

function removeLoader() {
    var element = document.getElementById("loader-father");
    element.classList.remove("spinner-that-is-always-spinning");
    var loaderElement = document.getElementById("loader");
    loaderElement.classList.remove("spinner");
 
}
//function SwalShow() {

//    Swal.fire({
//        title: 'Uploading...',
//        html: 'Please wait...',
//        allowEscapeKey: false,
//        allowOutsideClick: false,
//        didOpen: () => {
//            Swal.showLoading()
//        }
//    });
//}
function Showtooltip() {
    debugger
    $('[data-toggle="tooltip"]').tooltip();
}
function addTooltips() {
    $('[data-toggle="tooltip"]').tooltip({
        trigger: 'hover'
    });
    $('[data-toggle="tooltip"]').on('mouseleave', function () {
        $(this).tooltip('hide');
    });
    $('[data-toggle="tooltip"]').on('click', function () {
        $(this).tooltip('dispose');
    });
}
function nextstep() {
    var active = $('.wizard .nav-tabs li.active');
    active.next().removeClass('disabled');
    nextTab(active);
}
function prevstep() {
    var active = $('.wizard .nav-tabs li.active');
    prevTab(active);
}
function nextTab(elem) {
    $('.nav-tabs li.active').removeClass('active');
    $(elem).next().find('a[data-toggle="tab"]').click();
}
function prevTab(elem) {
    $(elem).prev().find('a[data-toggle="tab"]').click();
}
function navtabs() {
    $('.nav-tabs li.active').removeClass('active');
    $(this).addClass('active');
}
function focusInput(id) {
    document.getElementById(id).focus();
    const element = document.getElementById(id);
    element.focus();
}
function BlazorFocusElement(element) {
    if (element instanceof HTMLElement) {
        element.focus();
    }
}
/*$(document).ready(function () {*/
/*  $(document).on('change', '#uploadMYFiles', function (element) {*/
/*  $('#uploadMYFiles').change(function() {*/
/*function MyUploadTest(instance) {*/
function OnScrollEvent() {
    debugger
    $("#kt_quick_cart3").animate(
        {
            scrollTop: $('#kt_quick_cart3').scrollTop(0)
        },
        "slow"
    );
    //$('html, body').animate({
    //    scrollTop: $("#kt_quick_cart3").offset().top
    //}, 2000);
    //click = "document.getElementById('contact').scrollIntoView({behavior:'smooth'})"
}
function AddCommentOnProfile(dotNetObjectReference, id) {
    debugger
    var Comments =/* "my comments";*/ $('.ck-content').html();
    //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
    dotNetObjectReference.invokeMethodAsync("SubmitComments", Comments, id);
}
function UpdateCommentOnProfile(dotNetObjectReference, id) {
    debugger
    var Comments =/* "my comments";*/ $('.ck-content').html();
    //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
    dotNetObjectReference.invokeMethodAsync("EditComments", Comments, id);
}
function CallCkEditor() {
    debugger
    $('.ck-editor').remove();
    $('#editor').html("");
    ClassicEditor
        .create(document.querySelector('#editor'), {
            // toolbar: [ 'heading', '|', 'bold', 'italic', 'link' ]
        })
        .then(editor => {
            window.editor = editor;
        })
        .catch(err => {
            console.error(err.stack);
        });
}
function ShowComments(comments) {
    debugger
    $('.ck-content').html(comments);
}
function RemoveCkEditor() {

    $('.ck-reset').remove()

}
function MyFileUploadFunction(dotNetObjectReference, id) {
    var UploadId = "#" + id;
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var response;
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    $.ajax({
        type: "POST",
        url: "api/FileUploadsss",
        contentType: false,
        processData: false,
        data: data,
        async: false,
        success: function (message) {
            response = message
            dotNetObjectReference.invokeMethodAsync("ChangeParaContentValue", response);

        },
        error: function (message) {
            dotNetObjectReference.invokeMethodAsync("ErrorinUpload");

        }
    });
   
    //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
}

function MyFileUploadFunctionForOnBoarding(dotNetObjectReference, id) {
    debugger
    var UploadId = "#" + id;
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var response;
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    $.ajax({
        type: "POST",
        url: "api/FileUploadsss/UploadFilesOnboarding",
        contentType: false,
        processData: false,
        data: data,
        async: false,
        success: function (message) {
            debugger
            response = message
            dotNetObjectReference.invokeMethodAsync("ChangeParaContentValueForOnBoarding", response);
        },
        error: function (message) {
            debugger
            dotNetObjectReference.invokeMethodAsync("ErrorinUpload");
        }
    });

    //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
}
function MyFileUploadFunctionn(dotNetObjectReference,CallingFun, id) {
    debugger
    var UploadId = "#" + id;
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var response;
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
    }
    $.ajax({
        type: "POST",
        url: "api/FileUploadsss",
        contentType: false,
        processData: false,
        data: data,
        async: false,
        success: function (message) {
            debugger
            response = message
        },
        error: function () {
        }
    });
     
    dotNetObjectReference.invokeMethodAsync(CallingFun, response);
}



function EditExternalSearchesUpload(dotNetObjectReference, BuinsessProfileID, id, AttachementId) {
    debugger

    var UploadId = "#" + id;
    var PreViewId = "#" + id + "_prev";
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var DisplayNames;
    var result;
    var response;
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
        //  $(PreViewId).append("<span class='filenamedis temp'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p  class='discardupload2'></p></span>");
    }
    data.append("buisnessprofileid", BuinsessProfileID);
    data.append("ExternalSearchesId", AttachementId);
    $.ajax({
        type: "POST",
        url: "Home/UploadExternalFiles",
        contentType: false,
        processData: false,
        data: data,
        async: false,
        success: function (message) {
            debugger
            response = message.myFileArray;
            DisplayNames = message.displayFileNames;
            result = message.attachementIds;
            //if (result.length > 0) {
            //    $(PreViewId).find('.temp').remove();
            //}
            //for (var i = 0; i < DisplayNames.length; i++) {
            //    $(PreViewId).append("<span class='filenamedis'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p id=" + result[i] + " class='discardcharityupload'>(x)</p></span>");
            //}
        },
        error: function () {
        }
    });

    dotNetObjectReference.invokeMethodAsync("ChangeParaContentValue", response, DisplayNames);
}

function ExternalSearchesUpload(dotNetObjectReference, BuinsessProfileID, id, AttachementId) {
    debugger

    var UploadId = "#" + id;
    var PreViewId = "#" + id + "_prev";
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var DisplayNames;
    var result;
    var response;
    for (var i = 0; i < files.length; i++) {
        data.append(files[i].name, files[i]);
      //  $(PreViewId).append("<span class='filenamedis temp'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p  class='discardupload2'></p></span>");
    }
    data.append("buisnessprofileid", BuinsessProfileID);
    data.append("ExternalSearchesId", AttachementId);
    $.ajax({
        type: "POST",
        url: "Home/UploadExternalFiles",
        contentType: false,
        processData: false,
        data: data,
        async: false,
        success: function (message) {
            debugger
            response = message.myFileArray;
            DisplayNames = message.displayFileNames;
            result = message.attachementIds;
            //if (result.length > 0) {
            //    $(PreViewId).find('.temp').remove();
            //}
            //for (var i = 0; i < DisplayNames.length; i++) {
            //    $(PreViewId).append("<span class='filenamedis'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p id=" + result[i] + " class='discardcharityupload'>(x)</p></span>");
            //}
        },
        error: function () {
        }
    });

    dotNetObjectReference.invokeMethodAsync("ChangeParaContentValue", response, DisplayNames);
}

function addCommasOld(str) {
    debugger
    return str.replace(/^0+/, '').replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function addCommas(str) {
  
   // arr = str.toString().split(".");
   // arr[0] = arr[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
   // return arr.join(".");
    return str.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
   // return str.replace(/^0+/, '').replace(/\D/g, "").replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}
function Number_formatter_comma(id) {
   
    var val = document.getElementById(id).value;    
    document.getElementById(id).value = addCommas(val);
   // document.getElementById(id).value = val.toLocaleString("en-US");
}
function CaricaDataTablessr(table) {
  
    $(table).DataTable();

}
function BlazorFocusElement(element) {
    debugger
    if (element instanceof HTMLElement) {
        element.focus();
    }
}
function ShowModal(id) {
    var ShowId = "#" + id;
    $(ShowId).click();
}
function GetAddress() {
    var script = document.createElement("script");
    script.type = "text/javascript";
    script.src = "https://api.ipify.org?format=jsonp&callback=DisplayIP";
    document.getElementsByTagName("head")[0].appendChild(script);
};
function DisplayIP(response) {
    document.getElementById("text").innerHTML = "Your IP Address is " + response.ip;
}
function MatchPassword(dotNetObjectReference, pass, cpass) {
    debugger
    var pass = "#" + pass;
    var Cpass = "#" + cpass;
    var Matching = "";
    if ($(pass).val() != $(Cpass).val()) {
        Matching = "0";
    }
    else {
        Matching = "1";
    }
    dotNetObjectReference.invokeMethodAsync("checkMatching", Matching);
}

function ShowPassword(id) {
    debugger
    var Idd = "#" + id;
    var PasswordField = $(Idd);

    if (PasswordField[0].attributes[0].value == "password") {
        PasswordField[0].attributes[0].value = "text";
        $(Idd).closest('.input-group').find('#icon').attr('class', '');
        $(Idd).closest('.input-group').find('#icon').attr('class', 'fa fa-eye-slash');
    }
    else {
        PasswordField[0].attributes[0].value = "password";
        $(Idd).closest('.input-group').find('#icon').attr('class', '');
        $(Idd).closest('.input-group').find('#icon').attr('class', 'fa fa-eye');
    }
}
function ChangePassworFields() {
    debugger
    $('.input-group').find('.valid').attr('class', 'form-control');

}
function scrollIntoView(elementId) {
    var elem = document.getElementById(elementId);
    if (elem) {
        elem.scrollIntoView();
        window.location.hash = elementId;
    }
}
function NavLinkTochagne(text) {
    debugger
    $('.mynavlinktochange').html(text);
}
//$('.toastsDefaultSuccess').click(function () {
//    debugger
//    $(document).Toasts('create', {
//        class: 'bg-success',
//        title: 'Toast Title',
//        subtitle: 'Subtitle',
//        body: 'Lorem ipsum dolor sit amet, consetetur sadipscing elitr.'
//    })
//});
function OnBoardingUploadFunction(dotNetObjectReference, id, buisnessprofileid, BuisnessTypeId) {
    debugger
    var UploadId = "#" + id;
    var PreViewId = "#" + id + "_prev";
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var response;
    var DisplayNames;
    var result; var extensionRulebreached = false;
    for (var i = 0; i < files.length; i++) {
        var extension = files[i].name.split('.').pop();
        extension = extension.toLowerCase();
        if (!"png, pdf, jpg,jpeg, Doc, docx, xls, xlsx".match(extension)) {
            extensionRulebreached = true;
            dotNetObjectReference.invokeMethodAsync("ErrorinUpload");
            break;
        }
    }
    if (!extensionRulebreached) {
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
            $(PreViewId).append("<span class='filenamedis temp'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p  class='discardupload2'></p></span>");
            //  data.append(files[i].name, files[i]);
        }
        data.append("buisnessprofileid", buisnessprofileid);
        data.append("BuisnessTypeId", BuisnessTypeId);
        data.append("documenttype", id);
        $.ajax({
            type: "POST",
            url: "Home/UploadFiles",
            cache: false,
            contentType: false,
            processData: false,
            data: data,
            async: true,
            success: function (message) {
                debugger
                response = message.myFileArray;
                DisplayNames = message.displayFileNames;
                result = message.attachementIds;

                if (result.length > 0) {
                    $(PreViewId).find('.temp').remove();
                }
                for (var i = 0; i < DisplayNames.length; i++) {
                    $(PreViewId).append("<span class='filenamedis'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p id=" + result[i] + " class='discardcharityupload'>(x)</p></span>");
                }
            },
            error: function () {
            }
        });
        //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
        dotNetObjectReference.invokeMethodAsync("AttachmentContent", response, id, DisplayNames);
    }
}

function OnBoardingPersonalUploadFunction(dotNetObjectReference, id, buisnessprofileid, BuisnessTypeId) {
    debugger
    var UploadId = "#" + id;
    var PreViewId = "#" + id + "_prev";
    var fileUpload = $(UploadId).get(0);
    var files = fileUpload.files;
    var data = new FormData();
    var response;
    var DisplayNames;
    var result;
    var extensionRulebreached = false;
    for (var i = 0; i < files.length; i++) {
        var extension = files[i].name.split('.').pop();
        if (!"png, pdf, jpg, JPEG, Doc, docx, xls, xlsx".match(extension)) {
            extensionRulebreached = true;
            dotNetObjectReference.invokeMethodAsync("ErrorinUpload");
            break;
        }
    }
    if (!extensionRulebreached) {
        for (var i = 0; i < files.length; i++) {
            data.append(files[i].name, files[i]);
            $(PreViewId).append("<span class='filenamedis'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p  class='discardupload2'></p></span>");
            //  data.append(files[i].name, files[i]);
        }
        data.append("buisnessprofileid", buisnessprofileid);
        data.append("BuisnessTypeId", BuisnessTypeId);
        data.append("documenttype", id);
        $.ajax({
            type: "POST",
            url: "Home/UploadPersonalFiles",
            cache: false,
            contentType: false,
            processData: false,
            data: data,
            async: true,
            success: function (message) {
                debugger
                response = message.myFileArray;
                DisplayNames = message.displayFileNames;
                result = message.attachementIds;
                if (result.length > 0) {
                    $(PreViewId).html("");
                }
                for (var i = 0; i < DisplayNames.length; i++) {
                    $(PreViewId).append("<span class='filenamedis'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p id=" + result[i] + " class='discardcharityupload'>(x)</p></span>");
                }
            },
            error: function () {
            }
        });
        //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
        dotNetObjectReference.invokeMethodAsync("AttachmentContent", response, id, DisplayNames);
    }
}

function ShowRepModel() {
    $('body').append("<div class='modal-backdrop fade show'></div>");
    // $('.RepModel').css('display','block');
}

function RemoveRep() {
    debugger


}
//function CloseBuisnessProfileModel() {
//    $('.mymodel').css('display', 'none');
//    $('.mymodel').removeClass('show');
//}

function CloseBuisnessProfileModelAfterUpdate() {
    debugger
    $('.BPClose').click();
    /*  $('.modal-backdrop').removeClass('show');*/
}
function CloseCurrencyModelAfterUpdate() {
    debugger
    $('.CurrencyClose').click();
    /*  $('.modal-backdrop').removeClass('show');*/
}
function CloseClientAccountModelAfterUpdate() {
    debugger
    $('.ClientAccountClose').click();
    $('.ClientAccountClose').click();
    /*  $('.modal-backdrop').removeClass('show');*/
}
function CloseRepresentativesModelAfterUpdate() {
    debugger
    $('body').find('.modal-backdrop').remove();
    $('.AuhtorizedClose').click();

    /*  $('.modal-backdrop').removeAttr('class');*/
    /* $('.modal-backdrop').remove();*/
    /* $('.modal-backdrop').removeClass('show');*/
}
function CloseOwnerAfterUpdate() {
    $('.OwnerClose').click();
    /*  $('.modal-backdrop').removeAttr('class');*/
    /* $('.modal-backdrop').removeClass('show');*/
}
function CloseBuisnessInfoAfterUpdate() {
    $('.BuisnesssinfoClose').click();
    $('body').find('.modal-backdrop').remove();
    $('.modal-open').removeAttr('style');
    $('.modal-open').removeClass('modal-open');
    /*  $('.modal-backdrop').removeAttr('class');*/
    /* $('.modal-backdrop').removeClass('show');*/
}
function CloseFiancialInfoAfterUpdate() {
    $('.FinancialinfoClose').click();
    $('body').find('.modal-backdrop').remove();
    $('.modal-open').removeAttr('style');
    $('.modal-open').removeClass('modal-open');
    /*  $('.modal-backdrop').removeAttr('class');*/
    /* $('.modal-backdrop').removeClass('show');*/
}

function CloseDirectorsAfterUpdate() {
    $('body').find('.modal-backdrop').remove();
    $('.DirectorsClose').click();
    /*  $('.modal-backdrop').removeAttr('class');*/
    /* $('.modal-backdrop').removeClass('show');*/
}
function CloseTrusteeAfterUpdate() {
    $('body').find('.modal-backdrop').remove();
    $('.TrusteeClose').click();
    /*  $('.modal-backdrop').removeAttr('class');*/
    /* $('.modal-backdrop').removeClass('show');*/
}

function downloadBase64File(contentType, base64Data, fileName) {
    const linkSource = `data:${contentType};base64,${base64Data}`;
    const downloadLink = document.createElement("a");
    downloadLink.href = linkSource;
    downloadLink.download = fileName;
    downloadLink.target
    downloadLink.click();
}

triggerFileDownload = (fileName, url) => {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
}

function DownloadFromUrl(fileName) {
    fetch('https://www.fgconboarding.com/UploadedDocs/')
        .then(resp => resp.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            // the filename you want
            a.download = fileName;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            alert('your file has downloaded!'); // or you know, something with better UX...
        })
        .catch(() => alert('oh no!'));
};


function downloader(url, name) {
    fetch(url)
        .then(resp => resp.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.style.display = 'none';
            a.href = url;
            // the filename you want
            a.download = name;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
        })
        .catch(() => alert('An error sorry'));
}

//$(document).on('click', '.BuisnessPersdiscardupload', function () {
//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);
//    $(this).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteBuisnessPersonalFiles',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: true,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//})
//$(document).on('click', '.Buisnessdiscardupload', function () {
//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);
//    $(this).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteBuisnessFile1',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: true,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//})



//$(document).on('click', '.deleteupload', function () {

//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);


//    $(this).closest('.prifile').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteCharityFile1',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: false,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//})


//$(document).on('click', '.deleteuploadPers', function () {

//    debugger
//    var DocumentId = $(this).attr('id');


//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);


//    $(this).closest('.prifile').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeletePersonalFile',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId},
//            async: false,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//})


//$(document).on('click', '.discardcharityupload', function () {

//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);


//    $(this).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteCharityFile1',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: true,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//})



//function DeleteUpload(dotNetObjectReference, id,component) {

//    debugger
//    var compo = "." + component;
//    var DocumentId = $(compo).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);


//    $(compo).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: 'Home/DeleteCharityFile1',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: false,
//            success: function (result) {
//                // alert(result)
//                dotNetObjectReference.invokeMethodAsync("UploadDeleteUpdate");
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//}
//$(document).on('click', '.Companydiscardupload', function () {
//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);
//    $(this).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteCompanyFile1',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: true,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }
//})
//$(document).on('click', '.discardsolo', function () {
//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);
//    $(this).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteSolePersonalFiles',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: true,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }

//})
//$(document).on('click', '.charitydiscardupload', function () {
//    debugger
//    var DocumentId = $(this).attr('id');
//    //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//    // var files = $('#FormFile1').prop('files');
//    //  FilesList1.push(FileName);
//    $(this).closest('.filenamedis').remove()
//    if (DocumentId != undefined) {
//        $.ajax({
//            type: "POST",
//            url: '/Home/DeleteCharityPersonalFiles',
//            //contentType: false,
//            //processData: false,
//            data: { Documentid: DocumentId },
//            async: false,
//            success: function (result) {
//                // alert(result)
//            },
//            error: function (xhr, status, p3, p4) {
//                alert('Something is going to wrong please try again!');
//            }
//        });
//    }

//})

//$(document).on('click', '.GeneratePDf', function () {
//    debugger


//  /*  $('.form').css('display', '');*/
//    /* window.jsPDF = window.jspdf.jsPDF;*/

//    //html2canvas($("#generateToPdf")[0], {
//    //    onrendered: function (canvas) {
//    //        var imgData = canvas.toDataURL(
//    //            'image/png');
//    //        var doc = new jsPDF('p', 'pt', 'a4');
//    //        var Dataimg = 'data:image/png;base64,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';
//    //        doc.addImage(Dataimg, 'PNG', 250, 40, 70, 25);

//    //        doc.addImage(imgData, 'PNG', 10, 10);

//    //      /*    doc.save('sample-file.pdf');*/
//    //          let newWindow = window.open('/');
//    //fetch(doc.output('datauristring')).then(res => res.blob()).then(blob => {
//    //    newWindow.location = URL.createObjectURL(blob);
//    //})
//    //    }
//    //});


//    var doc = new jsPDF('p', 'pt', 'a4');


//    /*var html = document.querySelector('#generateToPdf')*/
//    var Dataimg = 'data:image/png;base64,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';
//    doc.addImage(Dataimg, 'PNG', 270, 40, 70, 25);

//    doc.setProperties({
//        title: "Application"
//    });

//    doc.html(document.getElementById('generateToPdf'), {
//        html2canvas: {
//            // insert html2canvas options here, e.g.
//            scale: 0.7,
//            letterRendering: true


//        },

//        callback: function () {


//          /*  //pdf.save('test.pdf');*/
//          //  window.open(doc.output('bloburl')); // to debug
//              let newWindow = window.open('/');
//    fetch(doc.output('datauristring')).then(res => res.blob()).then(blob => {
//        newWindow.location = URL.createObjectURL(blob);
//    })

//        },

//        x: 18,
//        y: 100,

//    });
//    var pageCount = doc.internal.getNumberOfPages(); //Total Page Number
//    for (i = 0; i < pageCount; i++) {
//        doc.setPage(i);
//        let pageCurrent = doc.internal.getCurrentPageInfo().pageNumber; //Current Page
//        doc.setFontSize(12);
//        doc.text('page: ' + pageCurrent + '/' + pageCount, 210-20,297-30,null,null);
//    }
//    //var specialElementHandlers = {
//    //    '#editor': function (element, renderer) {
//    //        return true;
//    //    }
//    //};

//    //doc.fromHTML(html, 30, 80) 
//    //doc.setProperties({
//    //    title: "Report"
//    //});
//    //doc.fromHTML($('#generateToPdf').html(), 15, 15, {
//    //    'width': 170,
//    //    'elementHandlers': specialElementHandlers,
//    //});
//    /*let dataSrc = doc.output("datauristring");*/

//    //let newWindow = window.open('/');
//    //fetch(doc.output('datauristring')).then(res => res.blob()).then(blob => {
//    //    newWindow.location = URL.createObjectURL(blob);
//    //})


//    //let win = window.open("", "myWindow");
//    //win.document.write("<html><head><title>jsPDF</title></head><body><embed src=" +
//    //    dataSrc + "></embed></body></html>");
//  /*  doc.output('dataurlnewwindow');*/
//    /*  doc.save('sample-file.pdf');*/

//    //$('.form').css('display', 'none');
//})
function IntroducerSearchMenu() {
    debugger
    var input, filter, ul, li, a, i;
    filter = $("#mySearch").val().toUpperCase();
    ul = document.getElementById("myMenu");
    li = $('#myMenu').find("a");
    if (filter != "") {
        for (i = 0; i < li.length; i++) {
            a = li[i];
            if (a.innerText.toUpperCase().indexOf(filter) > -1) {
                a.parentNode.style.display = "";
                if (a.parentNode.parentNode.localName == "ul") {
                    $(a).parent('li').parent('ul').css('display', 'block');
                    $(a).parent('li').parent('ul').parent('li').addClass('menu-open')
                    $(a).parent('li').parent('ul').parent('li').css('display', 'block')
                }

            } else {
                a.parentNode.style.display = "none";
            }
        }
    }
    else {
        for (i = 0; i < li.length; i++) {
            a = li[i];
            a.parentNode.style.display = "";
            if (a.parentNode.parentNode.localName == "ul") {
                $(a).parent('li').parent('ul').css('display', '');
                $(a).parent('li').parent('ul').parent('li').removeClass('menu-open')
                $(a).parent('li').parent('ul').parent('li').css('display', '')
            }
        }
    }
}
//function ExternalSearchesUpload(dotNetObjectReference, BuinsessProfileID, id, AttachementId) {
//    debugger
    
//    var UploadId = "#" + id;
//    var PreViewId = "#" + id + "_prev";
//    var fileUpload = $(UploadId).get(0);
//    var files = fileUpload.files;
//    var data = new FormData();
//    var DisplayNames;
//    var result;
//    for (var i = 0; i < files.length; i++) {
//        data.append(files[i].name, files[i]);
//        $(PreViewId).append("<span class='filenamedis temp'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p  class='discardupload2'></p></span>");
//    }
//    data.append("buisnessprofileid", BuinsessProfileID);
//    data.append("ExternalSearchesId", AttachementId);
//    $.ajax({
//        type: "POST",
//        url: "Home/UploadExternalFiles",
//        cache: false,
//        contentType: false,
//        processData: false,
//        data: data,
//        async: true,
//        success: function (message) {
//            debugger
//            response = message.myFileArray;
//            DisplayNames = message.displayFileNames;
//            result = message.attachementIds;
//            if (result.length > 0) {
//                $(PreViewId).find('.temp').remove();
//            }
//            for (var i = 0; i < DisplayNames.length; i++) {
//                $(PreViewId).append("<span class='filenamedis'>" + files[i].name + "<p id='filename' style='display:none'>" + files[i].name + "</p><p id=" + result[i] + " class='discardcharityupload'>(x)</p></span>");
//            }
//        },
//        error: function () {
//        }
//    });


//    $(document).on('click', '.discardcharityupload', function () {
//        debugger
//        var DocumentId = $(this).attr('id');
//        //  var FileName = $(this).closest('.filenamedis').find('#filename').html();
//        // var files = $('#FormFile1').prop('files');
//        //  FilesList1.push(FileName);
//        $(this).closest('.filenamedis').remove()
//        if (DocumentId != undefined) {
//            $.ajax({
//                type: "POST",
//                url: '/Home/DeleteExternalAttachment',
//                //contentType: false,
//                //processData: false,
//                data: { AttachmentId: DocumentId },
//                async: true,
//                success: function (result) {
//                    // alert(result)
//                },
//                error: function (xhr, status, p3, p4) {
//                    alert('Something is going to wrong please try again!');
//                }
//            });
//        }
//    })

//    //  DotNet.invokeMethodAsync('ArdantOffical', "ChangeParaContentValue", response);
//    //dotNetObjectReference.invokeMethodAsync("AttachmentContent", response, id, DisplayNames);
//}

