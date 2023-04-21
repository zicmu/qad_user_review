// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


showMenuDetails = (url, title) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $("#form-modal-menu .modal-body").html(res);
            $("#form-modal-menu .modal-title").html(title);
            $("#form-modal-menu").modal('show');


        }

    })

};

$(document).ready(function () {
    $(".Loader").on("click", function () {

        $.ajax({

            beforeSend: function () {
                $("#loader").modal('show');
            },
            /* complete: function () {
                 $("#loader").hide();
             },*/

            success: function (data) {
                var output = "Success";

                $("#results").html(output);
            },
            error: function () {
                $("#result").html("Failed to Fetch Data");
            }

        });

    });

});

$(document).ready(function () {
    $(".Loader").on("click", function () {

        $.ajax({

            beforeSend: function () {
                $("#loader").modal('show');
            },
            /* complete: function () {
                 $("#loader").hide();
             },*/

            success: function (data) {
                var output = "Success";

                $("#results").html(output);
            },
            error: function () {
                $("#result").html("Failed to Fetch Data");
            }

        });

    });

});

$(document).ready(function () {
    $(".HomePageloader").on("click", function () {

        $.ajax({

            beforeSend: function () {
                $("#HomePageloader").modal('show');
            },
            /* complete: function () {
                 $("#loader").hide();
             },*/

            success: function (data) {
                var output = "Success";

                $("#results").html(output);
            },
            error: function () {
                $("#result").html("Failed to Fetch Data");
            }

        });

    });

});




function approveSelected() {
    
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();

  // Get all the filtered rows in the table
    let filteredRows = document.querySelectorAll("#WorkingList tbody tr:not(.d-none)");

    // Loop through each filtered row
    for (let i = 0; i < filteredRows.length; i++) {
        // Get the current filtered row
        let filteredRow = filteredRows[i];

        if (selectedEmployee === "" && selectedPlant === "") {
            
                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Approved";
                }
            }
        
        // Check if the filtered row belongs to all two selected classes
       else if (selectedEmployee !== "" && selectedPlant !== "") {
            if (filteredRow.classList.contains(selectedEmployee) &&
                filteredRow.classList.contains(selectedPlant)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Approved";
                }
            }
        } else if (selectedEmployee === "") {
            if (filteredRow.classList.contains(selectedPlant)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Approved";
                }
            }
        } else {
            if (filteredRow.classList.contains(selectedEmployee)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Approved";
                }
            }
        }
    }
};

function disableSelected() {
    // Get all the filtered rows in the table
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();
    
    let filteredRows = document.querySelectorAll("#WorkingList tbody tr:not(.d-none)");

    // Loop through each filtered row
    for (let i = 0; i < filteredRows.length; i++) {
        // Get the current filtered row
        let filteredRow = filteredRows[i];
        if (selectedEmployee === "" && selectedPlant === "") {

            // Get the Decision field for the current filtered row
            let decisionField = filteredRow.querySelector("td#DropdownDecision select");

            // If the Decision field is "Open"
            if (decisionField.value === "Open") {
                // Update the Decision field to "Approved"
                decisionField.value = "Disable";
            }
        }

        // Check if the filtered row belongs to all two selected classes
        else if (selectedEmployee !== "" && selectedPlant !== "") {
            if (filteredRow.classList.contains(selectedEmployee) &&
                filteredRow.classList.contains(selectedPlant)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Disable";
                }
            }
        } else if (selectedEmployee === "") {
            if (filteredRow.classList.contains(selectedPlant)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Disable";
                }
            }
        } else {
            if (filteredRow.classList.contains(selectedEmployee)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value === "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Disable";
                }
            }
        }
    }
};

function OpenSelected() {
    // Get all the filtered rows in the table
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();

    let filteredRows = document.querySelectorAll("#WorkingList tbody tr:not(.d-none)");

    // Loop through each filtered row
    for (let i = 0; i < filteredRows.length; i++) {
        // Get the current filtered row
        let filteredRow = filteredRows[i];
        if (selectedEmployee === "" && selectedPlant === "") {

            // Get the Decision field for the current filtered row
            let decisionField = filteredRow.querySelector("td#DropdownDecision select");

            // If the Decision field is "Open"
            if (decisionField.value !== "Open") {
                // Update the Decision field to "Approved"
                decisionField.value = "Open";
            }
        }

        // Check if the filtered row belongs to all two selected classes
        else if (selectedEmployee !== "" && selectedPlant !== "") {
            if (filteredRow.classList.contains(selectedEmployee) &&
                filteredRow.classList.contains(selectedPlant)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value !== "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Open";
                }
            }
        } else if (selectedEmployee === "") {
            if (filteredRow.classList.contains(selectedPlant)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value !== "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Open";
                }
            }
        } else {
            if (filteredRow.classList.contains(selectedEmployee)) {

                // Get the Decision field for the current filtered row
                let decisionField = filteredRow.querySelector("td#DropdownDecision select");

                // If the Decision field is "Open"
                if (decisionField.value !== "Open") {
                    // Update the Decision field to "Approved"
                    decisionField.value = "Open";
                }
            }
        }
    }
};


function filterTable() {
    var selectedEmployee = $("#filterEmployee").val();
    var selectedPlant = $("#filterPlant").val();
    var selectedDecision = $("#filterDecision").val();

    if (selectedEmployee === "" && selectedPlant === "" && selectedDecision === "") {
        // Show all rows
        $("#WorkingList tbody tr").show();
    } else {
        //Hide all rows
        $("#WorkingList tbody tr").hide();
        //Filter by each selected option
        $("#WorkingList tbody tr").filter(function () {
            return (selectedEmployee === "" || $(this).hasClass(selectedEmployee)) &&
                (selectedPlant === "" || $(this).hasClass(selectedPlant)) &&
                (selectedDecision === "" || $(this).hasClass(selectedDecision));
        }).show();
    }
};


function filterEmployees() {
    var selectedPlant = document.getElementById("filterPlant").value;
    var employeeSelect = document.getElementById("filterEmployee");
    var allEmployeesOption = employeeSelect.querySelector("option[value='']");
    employeeSelect.selectedIndex = 0;

    for (var i = 0; i < employeeSelect.options.length; i++) {
        if (employeeSelect.options[i].getAttribute("data-plant") == selectedPlant || selectedPlant == "") {
            employeeSelect.options[i].style.display = "block";
        } else {
            employeeSelect.options[i].style.display = "none";
        }
    }
    allEmployeesOption.style.display = "block";
}
