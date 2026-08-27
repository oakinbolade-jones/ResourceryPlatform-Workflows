import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import {
  GetTenantsInput,
  TenantCreateDto,
  TenantDto,
  TenantService,
  TenantUpdateDto,
} from '@abp/ng.tenant-management/proxy';

@Component({
  selector: 'app-account-tenants',
  templateUrl: './account-tenants.component.html',
})
export class AccountTenantsComponent implements OnInit {
  tenants: TenantDto[] = [];
  loading = false;
  modalVisible = false;
  isEditMode = false;
  search = '';
  editingTenantId: string | null = null;
  editingConcurrencyStamp: string | undefined;

  tenantForm = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(64)]],
    adminEmail: ['', [Validators.required, Validators.email]],
    adminPassword: ['', [Validators.minLength(6)]],
    connectionString: [''],
  });

  constructor(
    private fb: FormBuilder,
    private tenantService: TenantService
  ) {}

  ngOnInit(): void {
    this.loadTenants();
  }

  loadTenants(): void {
    const input: GetTenantsInput = {
      skipCount: 0,
      maxResultCount: 100,
      sorting: 'name asc',
      filter: this.search || undefined,
    };

    this.loading = true;
    this.tenantService.getList(input).subscribe({
      next: result => {
        this.tenants = result.items ?? [];
      },
      error: error => {
        console.error('Failed to load tenants', error);
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  applyFilter(): void {
    this.loadTenants();
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.editingTenantId = null;
    this.editingConcurrencyStamp = undefined;
    this.tenantForm.reset({
      name: '',
      adminEmail: '',
      adminPassword: '',
      connectionString: '',
    });
    this.tenantForm.get('adminEmail')?.setValidators([Validators.required, Validators.email]);
    this.tenantForm.get('adminPassword')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.tenantForm.get('adminEmail')?.updateValueAndValidity();
    this.tenantForm.get('adminPassword')?.updateValueAndValidity();
    this.modalVisible = true;
  }

  openEditModal(tenant: TenantDto): void {
    if (!tenant.id) {
      return;
    }

    this.isEditMode = true;
    this.editingTenantId = tenant.id;
    this.editingConcurrencyStamp = tenant.concurrencyStamp;
    this.tenantForm.reset({
      name: tenant.name ?? '',
      adminEmail: '',
      adminPassword: '',
      connectionString: '',
    });

    this.tenantForm.get('adminEmail')?.clearValidators();
    this.tenantForm.get('adminPassword')?.clearValidators();
    this.tenantForm.get('adminEmail')?.updateValueAndValidity();
    this.tenantForm.get('adminPassword')?.updateValueAndValidity();

    this.tenantService.getDefaultConnectionString(tenant.id).subscribe({
      next: value => {
        this.tenantForm.patchValue({ connectionString: value || '' });
      },
      error: error => {
        console.error('Failed to load default connection string', error);
      },
    });

    this.modalVisible = true;
  }

  closeModal(): void {
    this.modalVisible = false;
  }

  save(): void {
    if (this.tenantForm.invalid) {
      this.tenantForm.markAllAsTouched();
      return;
    }

    const value = this.tenantForm.value;

    if (!this.isEditMode) {
      const payload: TenantCreateDto = {
        name: value.name ?? '',
        adminEmailAddress: value.adminEmail ?? '',
        adminPassword: value.adminPassword ?? '',
      };

      this.tenantService.create(payload).subscribe({
        next: tenant => {
          const connectionString = value.connectionString?.trim();
          if (tenant.id && connectionString) {
            this.tenantService.updateDefaultConnectionString(tenant.id, connectionString).subscribe({
              next: () => {
                this.closeModal();
                this.loadTenants();
              },
              error: error => {
                console.error('Failed to save tenant connection string', error);
              },
            });
            return;
          }

          this.closeModal();
          this.loadTenants();
        },
        error: error => {
          console.error('Failed to create tenant', error);
        },
      });
      return;
    }

    if (!this.editingTenantId) {
      return;
    }

    const payload: TenantUpdateDto = {
      name: value.name ?? '',
      concurrencyStamp: this.editingConcurrencyStamp,
    };

    this.tenantService.update(this.editingTenantId, payload).subscribe({
      next: () => {
        const connectionString = value.connectionString?.trim() || '';
        const afterConnectionUpdate = () => {
          this.closeModal();
          this.loadTenants();
        };

        if (connectionString) {
          this.tenantService.updateDefaultConnectionString(this.editingTenantId!, connectionString).subscribe({
            next: () => afterConnectionUpdate(),
            error: error => {
              console.error('Failed to update tenant connection string', error);
            },
          });
          return;
        }

        this.tenantService.deleteDefaultConnectionString(this.editingTenantId!).subscribe({
          next: () => afterConnectionUpdate(),
          error: error => {
            console.error('Failed to clear tenant connection string', error);
          },
        });
      },
      error: error => {
        console.error('Failed to update tenant', error);
      },
    });
  }

  deleteTenant(tenant: TenantDto): void {
    if (!tenant.id) {
      return;
    }

    const confirmed = window.confirm('Delete selected tenant?');
    if (!confirmed) {
      return;
    }

    this.tenantService.delete(tenant.id).subscribe({
      next: () => {
        this.loadTenants();
      },
      error: error => {
        console.error('Failed to delete tenant', error);
      },
    });
  }
}
