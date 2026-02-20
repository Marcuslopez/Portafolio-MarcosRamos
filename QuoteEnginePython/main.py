

from fastapi import FastAPI
from pydantic import BaseModel
from typing import List, Optional

app = FastAPI(title="Quote Engine", version="1.0.0")

class QuoteItem(BaseModel):
    idProducto: int
    nombre: str
    precio: float
    cantidad: int
    idCategoria: Optional[int] = None

class QuoteRequest(BaseModel):
    items: List[QuoteItem]
    descuentoPct: float = 0.0   # 0.05 = 5%
    impuestoPct: float = 0.07   # 0.07 = 7%

class QuoteResponse(BaseModel):
    subtotal: float
    descuentoMonto: float
    impuestoMonto: float
    total: float
    items: List[dict]

@app.post("/calculate", response_model=QuoteResponse)
def calculate(req: QuoteRequest):
    # Subtotales por item
    items_out = []
    subtotal = 0.0

    for it in req.items:
        if it.cantidad <= 0:
            continue
        line_sub = round(it.precio * it.cantidad, 2)
        subtotal += line_sub
        items_out.append({
            "idProducto": it.idProducto,
            "nombre": it.nombre,
            "precio": it.precio,
            "cantidad": it.cantidad,
            "subtotal": line_sub
        })

    subtotal = round(subtotal, 2)

    descuento_pct = max(0.0, min(req.descuentoPct, 0.90))
    impuesto_pct = max(0.0, min(req.impuestoPct, 0.50))

    descuento_monto = round(subtotal * descuento_pct, 2)
    base_imponible = round(subtotal - descuento_monto, 2)
    impuesto_monto = round(base_imponible * impuesto_pct, 2)
    total = round(base_imponible + impuesto_monto, 2)

    return QuoteResponse(
        subtotal=subtotal,
        descuentoMonto=descuento_monto,
        impuestoMonto=impuesto_monto,
        total=total,
        items=items_out
    )
