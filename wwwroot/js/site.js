"use strict";

function showMenuDetails(url, title) {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal-menu .modal-body").html(res);
            $("#form-modal-menu .modal-title").html(title);
            $("#form-modal-menu").modal("show");
        }
    });
}

$(document).ready(function () {
    $(".Loader").on("click", function () {
        $("#loader").modal("show");
    });

    $(".HomePageloader").on("click", function () {
        $("#HomePageloader").modal("show");
    });
});

function updateFilteredDecisions(fromValue, toValue) {
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();
    var filteredRows = document.querySelectorAll("#WorkingList tbody tr:not(.d-none)");

    for (var i = 0; i < filteredRows.length; i++) {
        var row = filteredRows[i];
        var matchesFilter = matchesCurrentFilter(row, selectedEmployee, selectedPlant);

        if (matchesFilter) {
            var decisionField = row.querySelector("td.decision-cell select");
            if (decisionField && decisionField.value === fromValue) {
                decisionField.value = toValue;
            }
        }
    }
}

function matchesCurrentFilter(row, selectedEmployee, selectedPlant) {
    if (selectedEmployee === "" && selectedPlant === "") return true;
    if (selectedEmployee !== "" && selectedPlant !== "") {
        return row.classList.contains(selectedEmployee) && row.classList.contains(selectedPlant);
    }
    if (selectedEmployee === "") return row.classList.contains(selectedPlant);
    return row.classList.contains(selectedEmployee);
}

function approveSelected() {
    updateFilteredDecisions("Pending", "Approved");
}

function disableSelected() {
    updateFilteredDecisions("Pending", "Disabled");
}

function pendingSelected() {
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();
    var filteredRows = document.querySelectorAll("#WorkingList tbody tr:not(.d-none)");

    for (var i = 0; i < filteredRows.length; i++) {
        var row = filteredRows[i];
        if (matchesCurrentFilter(row, selectedEmployee, selectedPlant)) {
            var decisionField = row.querySelector("td.decision-cell select");
            if (decisionField && decisionField.value !== "Pending") {
                decisionField.value = "Pending";
            }
        }
    }
}

function filterTable() {
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();
    var selectedStatus = $("#filterStatus").val();

    if (selectedEmployee === "" && selectedPlant === "" && selectedStatus === "") {
        $("#WorkingList tbody tr").show();
    } else {
        $("#WorkingList tbody tr").hide();
        $("#WorkingList tbody tr").filter(function () {
            return (selectedEmployee === "" || $(this).hasClass(selectedEmployee)) &&
                (selectedPlant === "" || $(this).hasClass(selectedPlant)) &&
                (selectedStatus === "" || $(this).hasClass(selectedStatus));
        }).show();
    }
}

function filterEmployees() {
    var selectedPlant = document.getElementById("filterPlant").value;
    var employeeSelect = document.getElementById("filterEmployee");
    var allOption = employeeSelect.querySelector("option[value='']");
    employeeSelect.selectedIndex = 0;

    for (var i = 0; i < employeeSelect.options.length; i++) {
        var option = employeeSelect.options[i];
        if (option.getAttribute("data-plant") === selectedPlant || selectedPlant === "") {
            option.style.display = "block";
        } else {
            option.style.display = "none";
        }
    }
    allOption.style.display = "block";
}
