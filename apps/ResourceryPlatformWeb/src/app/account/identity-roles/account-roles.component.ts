import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import {
  GetIdentityRolesInput,
  IdentityRoleCreateDto,
  IdentityRoleDto,
  IdentityRoleService,
  IdentityRoleUpdateDto,
} from '@abp/ng.identity/proxy';

@Component({
  selector: 'app-account-roles',
  templateUrl: './account-roles.component.html',
})
export class AccountRolesComponent implements OnInit {
  roles: IdentityRoleDto[] = [];
  totalCount = 0;
  loading = false;
  modalVisible = false;
  permissionModalVisible = false;
  permissionProviderKey = '';
  permissionEntityDisplayName = '';
  isEditMode = false;
  editingRoleId: string | null = null;
  editingConcurrencyStamp: string | undefined;
  search = '';

  roleForm = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(64)]],
    isDefault: [false],
    isPublic: [true],
  });

  constructor(
    private fb: FormBuilder,
    private identityRoleService: IdentityRoleService
  ) {}

  ngOnInit(): void {
    this.loadRoles();
  }

  loadRoles(): void {
    const input: GetIdentityRolesInput = {
      skipCount: 0,
      maxResultCount: 100,
      sorting: 'name asc',
      filter: this.search || undefined,
    };

    this.loading = true;
    this.identityRoleService.getList(input).subscribe({
      next: result => {
        this.roles = result.items ?? [];
        this.totalCount = result.totalCount ?? 0;
      },
      error: error => {
        console.error('Failed to load roles', error);
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.editingRoleId = null;
    this.editingConcurrencyStamp = undefined;
    this.roleForm.reset({
      name: '',
      isDefault: false,
      isPublic: true,
    });
    this.modalVisible = true;
  }

  openEditModal(role: IdentityRoleDto): void {
    this.isEditMode = true;
    this.editingRoleId = role.id ?? null;
    this.editingConcurrencyStamp = role.concurrencyStamp;
    this.roleForm.reset({
      name: role.name ?? '',
      isDefault: role.isDefault,
      isPublic: role.isPublic,
    });
    this.modalVisible = true;
  }

  closeModal(): void {
    this.modalVisible = false;
  }

  applyFilter(): void {
    this.loadRoles();
  }

  openPermissionsModal(role: IdentityRoleDto): void {
    this.permissionProviderKey = role.name ?? '';
    this.permissionEntityDisplayName = role.name ?? '';
    this.permissionModalVisible = true;
  }

  save(): void {
    if (this.roleForm.invalid) {
      this.roleForm.markAllAsTouched();
      return;
    }

    const value = this.roleForm.value;

    if (!this.isEditMode) {
      const payload: IdentityRoleCreateDto = {
        name: value.name ?? '',
        isDefault: value.isDefault ?? false,
        isPublic: value.isPublic ?? true,
      };

      this.identityRoleService.create(payload).subscribe({
        next: () => {
          this.closeModal();
          this.loadRoles();
        },
        error: error => {
          console.error('Failed to create role', error);
        },
      });
      return;
    }

    if (!this.editingRoleId) {
      return;
    }

    const payload: IdentityRoleUpdateDto = {
      name: value.name ?? '',
      isDefault: value.isDefault ?? false,
      isPublic: value.isPublic ?? true,
      concurrencyStamp: this.editingConcurrencyStamp,
    };

    this.identityRoleService.update(this.editingRoleId, payload).subscribe({
      next: () => {
        this.closeModal();
        this.loadRoles();
      },
      error: error => {
        console.error('Failed to update role', error);
      },
    });
  }

  deleteRole(role: IdentityRoleDto): void {
    if (!role.id) {
      return;
    }

    const confirmed = window.confirm('Delete selected role?');
    if (!confirmed) {
      return;
    }

    this.identityRoleService.delete(role.id).subscribe({
      next: () => {
        this.loadRoles();
      },
      error: error => {
        console.error('Failed to delete role', error);
      },
    });
  }
}
