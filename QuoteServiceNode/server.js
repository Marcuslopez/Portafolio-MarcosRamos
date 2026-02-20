const express = require("express");
const cors = require("cors");

const app = express();
app.use(cors());
app.use(express.json());

const PYTHON_URL = "http://localhost:8001/calculate";

// Health check
app.get("/health", (req, res) => res.json({ ok: true }));

// Endpoint REST para cotizar
app.post("/api/quote", async (req, res) => {
  try {
    const payload = req.body;

    const pyResp = await fetch(PYTHON_URL, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify(payload),
    });

    if (!pyResp.ok) {
      const errText = await pyResp.text();
      return res.status(502).json({ error: "Python engine error", detail: errText });
    }

    const result = await pyResp.json();
    return res.json(result);
  } catch (e) {
    return res.status(500).json({ error: "Quote service error", detail: e.message });
  }
});

const PORT = 5001;
app.listen(PORT, () => console.log(`QuoteServiceNode running on http://localhost:${PORT}`));
