import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  email = '';
  password = '';
  error: string | null = null;
  loading = false;

  constructor(private auth: AuthService, private router: Router) { }

  async onLogin() {
    this.error = null;
    this.loading = true;
    try {
      await this.auth.login({ email: this.email, password: this.password });
      await this.router.navigateByUrl('/products');
    } catch (e: any) {
      this.error = 'Credenciales incorrectas o API no disponible.';
    } finally {
      this.loading = false;
    }
  }
}
