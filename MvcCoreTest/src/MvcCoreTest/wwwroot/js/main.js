import "bootstrap-sass";
import "../css/main.scss";
import { hubConnection } from "signalr-no-jquery";

const connection = hubConnection();
const hubProxy = connection.createHubProxy("UsersHub"); 

hubProxy.on("reset", function(reset) {
  connection.stop();
  window.location.reload();
});

connection.start({ jsonp: true })
.done(function(){ console.log("Now connected, connection ID=" + connection.id); })
.fail(function(){ console.log("Could not connect"); });;