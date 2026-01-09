const express = require("express");
const http = require("http");
const cors = require("cors");
const { Server } = require("socket.io");

const app = express();
app.use(cors());
app.use(express.json());

const server = http.createServer(app);

const io = new Server(server, {
  cors: { origin: "*" }
});

io.on("connection", (socket) => {
  console.log("Cliente conectado:", socket.id);

  socket.on("disconnect", () => {
    console.log("Cliente desconectado:", socket.id);
  });
});

// Endpoint para recibir eventos desde tu API .NET
app.post("/events", (req, res) => {
  const evt = req.body; // { type, message, data }
  io.emit("event", evt); // envia a todos los clientes conectados
  res.json({ ok: true });
});

const PORT = 4000;
server.listen(PORT, () => {
  console.log(`Realtime service on http://localhost:${PORT}`);
});
