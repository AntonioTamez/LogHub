import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { AlertService } from '../../core/services/alert.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);
  private alertService = inject(AlertService);

  name = '';
  email = '';
  password = '';
  loading = false;

  onSubmit(): void {
    this.loading = true;

    this.authService.register({ name: this.name, email: this.email, password: this.password }).subscribe({
      next: () => {
        this.alertService.toast('Account created successfully!', 'success');
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        this.alertService.error('Registration Failed', err.error?.error || 'Could not create account. Please try again.');
        this.loading = false;
      }
    });
  }
}
