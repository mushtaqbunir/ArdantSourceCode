
function ImportCSV() {
    //debugger;
    //alert("hello");    
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.csv|.txt)$/;
    if (regex.test($("#fileUpload").val().toLowerCase())) {
        if (typeof (FileReader) != "undefined") {
            var reader = new FileReader();
            reader.onload = function (e) {
                var table = $("<table />");
                var rows = e.target.result.split("\n");
                for (var i = 0; i < rows.length; i++) {
                    var row = $("<tr />");
                    var cells = rows[i].split(",");
                    if (cells.length > 1) {
                        for (var j = 0; j < cells.length; j++) {
                            var cell = $("<td />");
                            cell.html(cells[j]);
                            row.append(cell);
                        }
                        table.append(row);
                    }
                }
                $("#dvCSV").html('');
                $("#dvCSV").append(table);
                //$.ajax({
                //    type: "POST",
                //    url: "https://localhost:44366/api/Transaction/ImportCSVFile",                    
                //    data: { BPMonitoring: rows },
                //    contentType: 'application/json; charset=utf-8',
                //    success: function (response) {
                //        alert("Hello");
                //    },
                //    failure: function (response) {
                //        alert(response.responseText);
                //    },
                //    error: function (response) {
                //        alert(response.responseText);
                //    }
                //});
            }
            reader.readAsText($("#fileUpload")[0].files[0]);
        } else {
            alert("This browser does not support HTML5.");
        }
    } else {
        alert("Please upload a valid CSV file.");
    }
    }


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

 

