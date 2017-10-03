import { managerService } from "./managerService";

let clientsLoading = false;
const userButtonAttr = "data-connected-user";

function getRow(el) {
  let kickButtonCode = el.userName ? `<button class="btn btn-danger" ${userButtonAttr}="${el.userName}">Kick</button`: ``;
  let userNameClass = el.userName ? "" : "text-muted";
  let userName = el.userName || "unregistered";
  let row = `<td class="${userNameClass}">${userName}</td><td>${kickButtonCode}</td>`;
  return row;
};

function refreshClients() {
  if (clientsLoading) {
    return;
  }

  clientsLoading = true;
  managerService.getClients().then(function(resp) {
    var rows = resp.reduce(function(sum, el) {
      let row = getRow(el);
      return sum + `<tr>${row}</tr>`;
    }, "");

    $("#usersTable").html(rows);
    clientsLoading = false;
  }, function() {
    console.error("Failed to load clients");
    clientsLoading = false;
  });
};

refreshClients();

$("#usersTable").on("click", `button[${userButtonAttr}]`, function() {
  var userName = $(this).attr("data-connected-user");
  managerService.kickClient(userName).then(function() {
    refreshClients();
  });
});