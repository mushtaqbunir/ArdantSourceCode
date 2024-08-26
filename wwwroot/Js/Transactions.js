function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
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


    $("#myToast").toast({
        delay: 20000
    });

    // Show toast
    $("#myToast").toast("show");
}
 
function GetTransactionByType(type) {
            //$.ajax({
            //            type: "GET",
            //    url: "https://localhost:44366/Transactions/LoadTransactionAll",
            //            data: { "Type": type},
            //            success: function (response) {
            //                alert("Hello");
            //            },
            //            failure: function (response) {
            //                alert(response.responseText);
            //            },
            //            error: function (response) {
            //                alert(response.responseText);
            //            }
            //        });    
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

//function Swalclose() {

//    swal.close();
//}
 

