$(document).ready(function () {
    $("#Previous").click(function () {
        if (CalculateAndSetPage("Previous"))
            $("form#page").submit();
    });
    $("#Next").click(function () {
        if (CalculateAndSetPage("Next"))
            $("form#page").submit();
    });
});

function CalculateAndSetPage(movingType) {
    var currentPage = parseInt($("#CurrentPage").val());
    var lastPage = parseInt($("#LastPage").val());

    if (currentPage == 1 && movingType == "Previous") {
        return false;
    }
    if (currentPage == lastPage && movingType == "Next") {
        return false;
    }

    if (movingType == "Previous") {
        currentPage--;
    }
    else if (movingType == "Next") {
        currentPage++;
    }
    else {
        alert("Somethins is wrong");
    }

    $("#CurrentPage").val(currentPage);
    return true;
}