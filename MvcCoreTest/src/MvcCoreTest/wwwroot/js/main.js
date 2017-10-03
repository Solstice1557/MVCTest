import "bootstrap-sass";
import "../css/main.scss";
import { hubConnection } from "signalr-no-jquery";

const connection = hubConnection();
const hubProxy = connection.createHubProxy("UsersHub");
const updateEvent = new Event("clientsUpdated");

hubProxy.on("reset", function() {
  connection.stop();
  window.location.reload();
});

hubProxy.on("update", function() {
  window.document.dispatchEvent(updateEvent);
});

connection.start({ jsonp: true })
  .done(function(){ console.log("Now connected, connection ID=" + connection.id); })
  .fail(function(){ console.log("Could not connect"); });