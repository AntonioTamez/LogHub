import { Injectable, inject } from '@angular/core';
import Swal, { SweetAlertResult } from 'sweetalert2';
import { ThemeService } from './theme.service';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  private themeService = inject(ThemeService);

  private getThemeColors() {
    const theme = this.themeService.currentTheme();
    return {
      background: theme.colors.card,
      color: theme.colors.text,
      confirmButtonColor: theme.colors.primary,
      cancelButtonColor: theme.colors.error,
      iconColor: theme.colors.primary,
    };
  }

  private getBaseConfig() {
    const colors = this.getThemeColors();
    return {
      background: colors.background,
      color: colors.color,
      confirmButtonColor: colors.confirmButtonColor,
      cancelButtonColor: colors.cancelButtonColor,
      customClass: {
        popup: 'swal-cyber-popup',
        title: 'swal-cyber-title',
        htmlContainer: 'swal-cyber-content',
        confirmButton: 'swal-cyber-confirm',
        cancelButton: 'swal-cyber-cancel',
      },
    };
  }

  success(title: string, message?: string): Promise<SweetAlertResult> {
    const colors = this.getThemeColors();
    return Swal.fire({
      ...this.getBaseConfig(),
      icon: 'success',
      title,
      text: message,
      iconColor: this.themeService.currentTheme().colors.success,
      timer: 3000,
      timerProgressBar: true,
      showConfirmButton: false,
    });
  }

  error(title: string, message?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      ...this.getBaseConfig(),
      icon: 'error',
      title,
      text: message,
      iconColor: this.themeService.currentTheme().colors.error,
    });
  }

  warning(title: string, message?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      ...this.getBaseConfig(),
      icon: 'warning',
      title,
      text: message,
      iconColor: this.themeService.currentTheme().colors.warning,
    });
  }

  info(title: string, message?: string): Promise<SweetAlertResult> {
    return Swal.fire({
      ...this.getBaseConfig(),
      icon: 'info',
      title,
      text: message,
      iconColor: this.themeService.currentTheme().colors.info,
    });
  }

  async confirm(
    title: string,
    message?: string,
    confirmText: string = 'Confirm',
    cancelText: string = 'Cancel'
  ): Promise<boolean> {
    const result = await Swal.fire({
      ...this.getBaseConfig(),
      icon: 'question',
      title,
      text: message,
      showCancelButton: true,
      confirmButtonText: confirmText,
      cancelButtonText: cancelText,
      reverseButtons: true,
    });
    return result.isConfirmed;
  }

  async confirmDanger(
    title: string,
    message?: string,
    confirmText: string = 'Delete',
    cancelText: string = 'Cancel'
  ): Promise<boolean> {
    const theme = this.themeService.currentTheme();
    const result = await Swal.fire({
      ...this.getBaseConfig(),
      icon: 'warning',
      title,
      text: message,
      showCancelButton: true,
      confirmButtonText: confirmText,
      cancelButtonText: cancelText,
      confirmButtonColor: theme.colors.error,
      iconColor: theme.colors.error,
      reverseButtons: true,
    });
    return result.isConfirmed;
  }

  toast(title: string, icon: 'success' | 'error' | 'warning' | 'info' = 'success'): void {
    const theme = this.themeService.currentTheme();
    const iconColors = {
      success: theme.colors.success,
      error: theme.colors.error,
      warning: theme.colors.warning,
      info: theme.colors.info,
    };

    Swal.fire({
      toast: true,
      position: 'top-end',
      icon,
      title,
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true,
      background: theme.colors.card,
      color: theme.colors.text,
      iconColor: iconColors[icon],
      customClass: {
        popup: 'swal-cyber-toast',
      },
    });
  }

  loading(title: string = 'Loading...'): void {
    const theme = this.themeService.currentTheme();
    Swal.fire({
      title,
      allowOutsideClick: false,
      allowEscapeKey: false,
      showConfirmButton: false,
      background: theme.colors.card,
      color: theme.colors.text,
      didOpen: () => {
        Swal.showLoading();
      },
      customClass: {
        popup: 'swal-cyber-popup',
      },
    });
  }

  closeLoading(): void {
    Swal.close();
  }
}
