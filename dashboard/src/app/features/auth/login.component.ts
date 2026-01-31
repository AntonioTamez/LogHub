import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AlertService } from '../../core/services/alert.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './login.component.html'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private alertService = inject(AlertService);

  email = '';
  password = '';
  loading = false;

  onSubmit(): void {
    this.loading = true;

    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: () => {
        this.alertService.toast('Welcome back!', 'success');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.alertService.error('Authentication Failed', err.error?.error || 'Invalid credentials. Please try again.');
        this.loading = false;
      }
    });
  }
}
