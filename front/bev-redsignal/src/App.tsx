import React, { useEffect, useState } from "react";
import { toast } from "react-toastify";
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
  HubConnectionState,
} from "@microsoft/signalr";

import "react-toastify/dist/ReactToastify.css";

function App() {
  const [hubStatus, setHubStatus] = useState("");
  const [ad, setAd] = useState("");

  useEffect(() => {
    createConnection();


  }, []);

  function createConnection() {
    const urlParams = new URLSearchParams(window.location.search);
    const ad = urlParams.get("ad") ?? "Strange";

    try {
      const hubConnection: HubConnection = new HubConnectionBuilder()
        .withUrl(`http://localhost:5000/redhub?ad=${ad}`, {
          accessTokenFactory: () =>
            "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJTdXBlclVzZXIiLCJuYmYiOjE2NDM1MDAzNTAsImV4cCI6MTY0MzkzMjM0OSwiaWF0IjoxNjQzNTAwMzUwfQ.HZKZi92CNOTaVKpW26KEvrcvcPh58IlD38L-ClwOYv3AB2IBS6RXMrdd5FAyEz5VCG-1NGSKE1xzzEcAVUQKFQ",
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.None)
        .build();

      setHubStatus(hubConnection.state);

      hubConnection.start().then(() => {
        if (hubConnection.state === HubConnectionState.Connected) {
          hubConnection.invoke("Hi");

          setHubStatus(hubConnection.state);
          setAd(ad);


          //https://docs.microsoft.com/en-us/aspnet/core/signalr/streaming?view=aspnetcore-6.0
          hubConnection.stream("Counter", 10, 500).subscribe({
            next: (item) => {
              console.log("Item: " + item);
            },
            complete: () => {
              console.info("Stream completed");
            },
            error: (error) => {
              console.error(error);
            },
          });
        }
      });

      hubConnection.on("ReceiveMessage", (message: string) => {
        toast.success(message, {
          position: "top-center",
          autoClose: 3000,
          hideProgressBar: false,
          closeOnClick: true,
          pauseOnHover: false,
          draggable: true,
          progress: undefined,
        });
      });

      hubConnection.onclose((error) => {
        setHubStatus(hubConnection.state);
      });

      hubConnection.onreconnected((connectionId) => {
        setHubStatus(hubConnection.state);
      });

      hubConnection.onreconnecting((error) => {
        setHubStatus(hubConnection.state);
      });
    } catch (error) {}
  }

  return (
    <div className="App">
      Hub Connection Status: {hubStatus}
      <hr />
      <br />
      {ad && (
        <span>
          Advisory <strong>{ad}</strong> is connected...
        </span>
      )}
    </div>
  );
}

export default App;
