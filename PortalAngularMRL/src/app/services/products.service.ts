import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Producto } from '../models/product.models';
import { firstValueFrom } from 'rxjs';
import { Products } from '../pages/products/products';

@Injectable({ providedIn: 'root' })
export class ProductsService {
  constructor(private http: HttpClient) { }

  async list(): Promise<Producto[]> {
    const url = `${environment.apiBaseUrl}/api/producto`;
    return await firstValueFrom(this.http.get<Producto[]>(url));
  }
}
