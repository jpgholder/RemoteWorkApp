"use strict"

const sendButton = document.getElementById("send-button");
const chat = document.getElementById("chat");
const noMessages = document.getElementById("no-messages");
const userId = document.getElementById("user-id").value;
sendButton.disabled = true;

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/TeamChat", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .build();

connection.start().then(function () {
    connection.invoke("JoinTeam");
    sendButton.disabled = false;
}).catch(function (err) {
    return console.error(err);
});

connection.on("ReceiveMessage", function (message) {
    if (noMessages) {
        noMessages.remove();
    }
    if (message.senderId !== userId) {
        const div = document.createElement("div");
        const senderName = document.createElement("p");
        senderName.classList.add("mb-1");
        senderName.textContent = message.senderName;
        const messageContent = document.createElement("p");
        messageContent.classList = "bg-primary text-white p-2 rounded message"
        messageContent.textContent = message.content;
        div.append(senderName, messageContent);
        chat.appendChild(div);
    } else {
        const div = document.createElement("div");
        const messageContent = document.createElement("p");
        messageContent.classList = "bg-light p-2 rounded border message ms-auto";
        messageContent.textContent = message.content;
        div.appendChild(messageContent);
        chat.appendChild(div);
    }
});

sendButton.addEventListener("click", function (event) {
    event.preventDefault();
    const messageInput = document.getElementById("message-input");
    const message = messageInput.value;
    connection.invoke("SendMessage", message);
    messageInput.value = "";
});

