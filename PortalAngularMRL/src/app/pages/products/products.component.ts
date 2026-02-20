import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProductsService } from '../../services/products.service';
import { Producto } from '../../models/product.models';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './products.component.html'
})
export class ProductsComponent {
  loading = true;
  error: string | null = null;
  items: Producto[] = [];

  constructor(
    private products: ProductsService,
    private auth: AuthService,
    private router: Router
  ) { }

  async ngOnInit() {
    await this.load();
  }

  async load() {
    this.loading = true;
    this.error = null;
    try {
      this.items = await this.products.list();
    } catch (e: any) {
      // Si es 401, probablemente token inválido/expirado
      if (e?.status === 401) {
        this.auth.logout();
        await this.router.navigateByUrl('/login');
        return;
      }
      this.error = 'No se pudieron cargar los productos.';
    } finally {
      this.loading = false;
    }
  }

  async logout() {
    this.auth.logout();
    await this.router.navigateByUrl('/login');
  }
}
