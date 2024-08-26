window.createObject = (dotNetHelper) => {
    DotNet = dotNetHelper;
};
var $DotNet;
window.createObjectForLogOut = (dotNetHelper1) => {
    $DotNet = dotNetHelper1;
};
function CountriesWithFlag() {
    $("#data-countries").select2({
        templateResult: formatCountry
        
    });
    var flag = $("#data-countries").next().find(".select2-selection__rendered").attr("title").toLowerCase();
    if (flag != "select country") {
        var span = '<span class="flag-icon flag-icon-{flag} flag-icon-squared"></span> '.replace("{flag}", flag);
        $("#data-countries").next().find(".select2-selection__rendered").prepend(span);
    }
    $("#data-countries").change(function () {
        var country = $(this).val();
        var flag = $(this).next().find(".select2-selection__rendered").attr("title").toLowerCase();
        var span = '<span class="flag-icon flag-icon-{flag} flag-icon-squared"></span> '.replace("{flag}", flag);
        $(this).next().find(".select2-selection__rendered").prepend(span);
        DotNet.invokeMethodAsync('ChangeCountry', country);
    });
}
function NationalitiesWithFlag() {
    $("#data-nationalities").select2({
        templateResult: formatCountry
    });
    var flag = $("#data-nationalities").next().find(".select2-selection__rendered").attr("title").toLowerCase();
    if (flag != "select nationality") {
        var span = '<span class="flag-icon flag-icon-{flag} flag-icon-squared"></span> '.replace("{flag}", flag);
        $("#data-nationalities").next().find(".select2-selection__rendered").prepend(span);
    }
    $("#data-nationalities").change(function () {
        var nationality = $(this).val();
        var flag = $(this).next().find(".select2-selection__rendered").attr("title").toLowerCase();
        var span = '<span class="flag-icon flag-icon-{flag} flag-icon-squared"></span> '.replace("{flag}", flag);
        $(this).next().find(".select2-selection__rendered").prepend(span);
        DotNet.invokeMethodAsync('ChangeNationality', nationality);
    });
}
function formatCountry(country) {
    if (!country.id) { return country.text; }
    var $country = $(
        '<span class="flag-icon flag-icon-' + country.title.toLowerCase() + ' flag-icon-squared"></span>' +
        '<span class="flag-text">' + country.text + "</span>"
    );
    return $country;
};
function Notification(minusId,plusId) {
    NotificationMinus(minusId);
    NotificationPlus(plusId);
    OnBoardingNotificationCount();
    ComplianceApplicationCount();
    OperationApplicationCount();
}
function NotificationMinus(minusId) {
    var notification = $("#" + minusId).text();
    if (!notification) {
        return;
    }
    if (notification == "1") {
        $("#" + minusId).text("");
        return;
    }
    notification = parseInt(notification);
    notification = notification - 1;
    $("#" + minusId).text(notification);
}
function NotificationPlus(plusId) {
    var notification = $("#" + plusId).text();
    if (!notification) {
        $("#" + plusId).text(1);
        return;
    }
    notification = parseInt(notification);
    notification = notification + 1;
    $("#" + plusId).text(notification);
}
function OnBoardingNotificationCount() {
    var RefferApplicationsCount = $("#RefferApplicationsCount").text();
    var ReturnApplicationsCount = $("#ReturnApplicationsCount").text();
    var AwaitingApplicationsCount = $("#AwaitingApplicationsCount").text();
    RefferApplicationsCount = parseInt(RefferApplicationsCount);
    ReturnApplicationsCount = parseInt(ReturnApplicationsCount);
    AwaitingApplicationsCount = parseInt(AwaitingApplicationsCount);
    var OnBoardingNotificationCount = 0;
    if (RefferApplicationsCount) {
        OnBoardingNotificationCount = OnBoardingNotificationCount + RefferApplicationsCount;
    }
    if (ReturnApplicationsCount) {
        OnBoardingNotificationCount = OnBoardingNotificationCount + ReturnApplicationsCount;
    }
    if (AwaitingApplicationsCount) {
        OnBoardingNotificationCount = OnBoardingNotificationCount + AwaitingApplicationsCount;
    }
    if (OnBoardingNotificationCount == 0) {
        $("#OnBoardingApplicationCount").text("");

    }
    else {
        $("#OnBoardingApplicationCount").text(OnBoardingNotificationCount);
    }

}
function ComplianceApplicationCount() {
    var PassApplicationsCount = $("#PassApplicationsCount").text();
    PassApplicationsCount= parseInt(PassApplicationsCount);
    var ComplianceApplicationCount = 0;
    if (PassApplicationsCount) {
        ComplianceApplicationCount = ComplianceApplicationCount + PassApplicationsCount;
    }
    if (ComplianceApplicationCount == 0) {
        $("#ComplianceApplicationCount").text("");
    }
    else {
        $("#ComplianceApplicationCount").text(ComplianceApplicationCount);
    }

}
function OperationApplicationCount() {
    var ApprovedApplicationsCount = $("#ApprovedApplicationsCount").text();
    ApprovedApplicationsCount = parseInt(ApprovedApplicationsCount);
    var OperationApplicationCount = 0;
    if (ApprovedApplicationsCount) {
        OperationApplicationCount = OperationApplicationCount + ApprovedApplicationsCount;
    }
    if (OperationApplicationCount == 0) {
        $("#OperationApplicationCount").text("");
    }
    else {
        $("#OperationApplicationCount").text(OperationApplicationCount);
    }

}
function UncheckCheckBox(c) {
    $('.' + c).prop('checked', false);
}
function Active(id) {
    $("a").removeClass("active-background-color");
    $(id).addClass("active-background-color");
}
function AddingComma(id, type) {
    var val = $("#" + id).val();
    val = val.replace(/[^0-9. ]/g, "");
    val = val.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
    $("#" + id).val(val);
    var num = val.replaceAll(",", "");
    if (num) {
        num = parseFloat(num);
        if (DotNet) {
            DotNet.invokeMethodAsync('PerMonthCommaSeparated', num, type);
        }
    }
}
//function AddingComma(id) {
//    debugger
//    var val = $("#" + id).val();
//    val = val.replace(/[^0-9. ]/g, "");
//    val = val.replace(/\B(?=(\d{3})+(?!\d))/g, ",");
//    $("#" + id).val(val);
//}
function DisabledRiskMatrixAllFileds() {
    $("input").attr("disabled", "disabled");
    $("select").attr("disabled", "disabled");
    $("button").attr("disabled", "disabled");
}
var $idleTime = 0;
function SetIdleTimeLogOut() {
   //Zero the idle timer on mouse movement and key press.
    $(this).mousemove(function (e) {
        $idleTime = 0;
    });
    $(this).keypress(function (e) {
        $idleTime = 0;
    });
    var idleInterval = setInterval(timerIncrement, 60000); // 1 minute
}
function timerIncrement() {
    $idleTime = $idleTime + 1;
    if ($idleTime > 479000) { // 8 hours
        if ($DotNet) {
            $DotNet.invokeMethodAsync('LogOutIdle');
        }
    }
}
function FilesEmpty() {
    $('input[type="file"]').val("");
}
function FilesEmpty(id) {
    $('#'+id).val("");
}
function ActiveApplication(id) {
    $(".dot-action").css("color", "");
    $('#dot-' + id).css("color", "red");
}
function NavigateToUrl(url,target) {
    window.open(url, target);
};
function ManyCountriesWithFlag(id, func, donNet) {
    $("#" + id).change(function () {
        var countryId = $(this).val();
        countryId = countryId.map(Number).filter(s => s !== 0);
        donNet.invokeMethodAsync(func, countryId);
    });
}
function KycShowHide(select) {
    if ($("#" + select).is(":checked")) {
        $("." + select).show();
    }
    else {
        $("." + select).hide();
    }
}

function scrollToValidationMessage() {
    var validationMessageElements = document.getElementsByClassName('validation-message');
    if (validationMessageElements.length > 0) {
        validationMessageElements[0].scrollIntoView({ behavior: 'smooth', block: 'center', inline: 'center' });
    }
}

function editJob() {
    var inputs = document.querySelectorAll('input[readonly]');
    inputs.forEach(function (input) {
        input.removeAttribute('readonly');
    });

    var textareas = document.querySelectorAll('textarea[readonly]');
    textareas.forEach(function (textarea) {
        textarea.removeAttribute('readonly');
    });

    // Remove disabled attribute from dropdowns
    var dropdowns = document.querySelectorAll('select[disabled]');
    dropdowns.forEach(function (dropdown) {
        dropdown.removeAttribute('disabled');
    });

    // Disable the edit button
    var editButton = document.getElementById('btnedit'); // Assuming the ID of the edit button is 'editButton'
    editButton.disabled = true;
}

window.downloadPDF = function (base64String) {
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);
    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }
    const byteArray = new Uint8Array(byteNumbers);
    const blob = new Blob([byteArray], { type: 'application/pdf' });
    const link = document.createElement('a');
    link.href = window.URL.createObjectURL(blob);
    link.download = 'Invoice.pdf';
    link.click();
}

window.initSignaturePad = (canvasElement) => {
    var canvas = canvasElement;

    // Create a new SignaturePad instance
    var signaturePad = new SignaturePad(canvas, {
        backgroundColor: 'rgb(255, 255, 255)', // Background color of the canvas
        penColor: 'rgb(0, 0, 0)', // Color of the drawing ink
        minWidth: 1, // Minimum width of a line
        maxWidth: 4, // Maximum width of a line
        throttle: 16, // Maximum time in milliseconds to spend on rendering the next frame, lower = smoother lines
        // Other SignaturePad options can go here
    });

    // Additional initialization code if needed
};

window.initializeSignaturePad = (canvas) => {
    var signaturePad = new SignaturePad(canvas);
}

window.clearSignature = (canvas) => {
    var signaturePad = new SignaturePad(canvas);
    signaturePad.clear();
}

window.getSignatureImageData = () => {
    if (signaturePad.isEmpty()) {
        return null; // Return null if signature pad is empty
    } else {
        return signaturePad.toDataURL();
    }
}

var getid = document.getElementById('myPopover');

// Function to hide the popover
function hidePopover() {
    if (getid.style.display === 'block') {
        getid.style.display = 'none';
    }
}

// Event listener to hide the popover when clicking anywhere on the document
document.body.addEventListener('click', function (event) {
    var targetElement = event.target; // Element that was clicked

    // Check if the clicked element is not the popover or a child of the popover
    if (targetElement !== getid && !getid.contains(targetElement)) {
        hidePopover(); // Hide the popover
    }
});




window.attachNavLinksEventListeners = function () {
    
    var sidebar = document.querySelector('.main-sidebar');
    var toggleButton = document.querySelector('.navbar-nav .nav-link[data-widget="pushmenu"]');
    var sidebarOverlay = document.getElementById('sidebar-overlay');
    
    // Function to toggle sidebar visibility
    function toggleSidebar() {
        if (window.innerWidth <= 768) { // Check if it's a small device
            if (sidebar.style.display === 'none' || sidebar.style.display === '') {
                sidebar.style.display = 'block';
                sidebarOverlay.style.display = 'block'; // Show overlay when sidebar is shown
            }
            else {
                sidebar.style.display = 'none';
                sidebarOverlay.style.display = 'none !important'; // Hide overlay when sidebar is hidden
            }
        }
    }

    // Attach event listener to toggle button
    if (toggleButton && sidebar) {
        toggleButton.addEventListener('click', function (event) {
            event.preventDefault(); // Prevent default anchor behavior
            toggleSidebar();
        });
    }

    // Attach event listeners to nav links
    if (sidebar) {
        var navLinks = sidebar.querySelectorAll('.nav-link-sm');
        navLinks.forEach(function (link) {
            link.addEventListener('click', function () {
                toggleSidebar(); // Toggle sidebar on nav link click
            });
        });
    }
};



    window.setupPopover = function() {
        var popoverButton = document.getElementById('popoverButton');
    var popover = document.getElementById('myPopover');

    popoverButton.addEventListener('click', function(event) {
        popover.style.display = (popover.style.display === 'none') ? 'block' : 'none';
    event.stopPropagation(); // Prevents the click event from bubbling up
        });

    document.addEventListener('click', function(event) {
            if (event.target !== popoverButton && !popover.contains(event.target)) {
        popover.style.display = 'none';
            }
        });
    };
























