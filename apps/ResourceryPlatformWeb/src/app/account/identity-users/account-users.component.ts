import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import {
  GetIdentityUsersInput,
  IdentityRoleDto,
  IdentityUserCreateDto,
  IdentityUserDto,
  IdentityUserService,
  IdentityUserUpdateDto,
} from '@abp/ng.identity/proxy';

@Component({
  selector: 'app-account-users',
  templateUrl: './account-users.component.html',
})
export class AccountUsersComponent implements OnInit {
  users: IdentityUserDto[] = [];
  availableRoles: IdentityRoleDto[] = [];
  selectedRoleNames: string[] = [];
  loading = false;
  modalVisible = false;
  permissionModalVisible = false;
  permissionProviderKey = '';
  permissionEntityDisplayName = '';
  isEditMode = false;
  search = '';
  editingUserId: string | null = null;
  editingConcurrencyStamp: string | undefined;

  userForm = this.fb.group({
    userName: ['', [Validators.required, Validators.maxLength(64)]],
    name: ['', [Validators.required, Validators.maxLength(64)]],
    surname: ['', [Validators.required, Validators.maxLength(64)]],
    email: ['', [Validators.required, Validators.email]],
    phoneNumber: [''],
    password: ['', [Validators.minLength(6)]],
    isActive: [true],
    lockoutEnabled: [true],
  });

  constructor(
    private fb: FormBuilder,
    private identityUserService: IdentityUserService
  ) {}

  ngOnInit(): void {
    this.loadUsers();
    this.loadAssignableRoles();
  }

  loadUsers(): void {
    const input: GetIdentityUsersInput = {
      skipCount: 0,
      maxResultCount: 100,
      sorting: 'userName asc',
      filter: this.search || undefined,
    };

    this.loading = true;
    this.identityUserService.getList(input).subscribe({
      next: result => {
        this.users = result.items ?? [];
      },
      error: error => {
        console.error('Failed to load users', error);
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  loadAssignableRoles(): void {
    this.identityUserService.getAssignableRoles().subscribe({
      next: result => {
        this.availableRoles = result.items ?? [];
      },
      error: error => {
        console.error('Failed to load assignable roles', error);
      },
    });
  }

  applyFilter(): void {
    this.loadUsers();
  }

  openCreateModal(): void {
    this.isEditMode = false;
    this.editingUserId = null;
    this.editingConcurrencyStamp = undefined;
    this.selectedRoleNames = [];
    this.userForm.reset({
      userName: '',
      name: '',
      surname: '',
      email: '',
      phoneNumber: '',
      password: '',
      isActive: true,
      lockoutEnabled: true,
    });
    this.userForm.get('password')?.setValidators([Validators.required, Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();
    this.modalVisible = true;
  }

  openEditModal(user: IdentityUserDto): void {
    if (!user.id) {
      return;
    }

    this.isEditMode = true;
    this.editingUserId = user.id;
    this.editingConcurrencyStamp = user.concurrencyStamp;
    this.userForm.reset({
      userName: user.userName ?? '',
      name: user.name ?? '',
      surname: user.surname ?? '',
      email: user.email ?? '',
      phoneNumber: user.phoneNumber ?? '',
      password: '',
      isActive: user.isActive,
      lockoutEnabled: user.lockoutEnabled,
    });
    this.userForm.get('password')?.clearValidators();
    this.userForm.get('password')?.setValidators([Validators.minLength(6)]);
    this.userForm.get('password')?.updateValueAndValidity();

    this.identityUserService.getRoles(user.id).subscribe({
      next: result => {
        this.selectedRoleNames = (result.items ?? []).map(x => x.name || '').filter(Boolean);
      },
      error: error => {
        console.error('Failed to load user roles', error);
        this.selectedRoleNames = [];
      },
    });

    this.modalVisible = true;
  }

  closeModal(): void {
    this.modalVisible = false;
  }

  onRoleToggle(roleName: string, checked: boolean): void {
    if (checked) {
      if (!this.selectedRoleNames.includes(roleName)) {
        this.selectedRoleNames = [...this.selectedRoleNames, roleName];
      }
      return;
    }

    this.selectedRoleNames = this.selectedRoleNames.filter(x => x !== roleName);
  }

  isRoleSelected(roleName: string | undefined): boolean {
    if (!roleName) {
      return false;
    }
    return this.selectedRoleNames.includes(roleName);
  }

  openPermissionsModal(user: IdentityUserDto): void {
    this.permissionProviderKey = user.id ?? '';
    this.permissionEntityDisplayName = user.userName ?? '';
    this.permissionModalVisible = true;
  }

  save(): void {
    if (this.userForm.invalid) {
      this.userForm.markAllAsTouched();
      return;
    }

    const value = this.userForm.value;

    if (!this.isEditMode) {
      const payload: IdentityUserCreateDto = {
        userName: value.userName ?? '',
        name: value.name ?? '',
        surname: value.surname ?? '',
        email: value.email ?? '',
        phoneNumber: value.phoneNumber ?? '',
        password: value.password ?? '',
        isActive: value.isActive ?? true,
        lockoutEnabled: value.lockoutEnabled ?? true,
        roleNames: this.selectedRoleNames,
      };

      this.identityUserService.create(payload).subscribe({
        next: () => {
          this.closeModal();
          this.loadUsers();
        },
        error: error => {
          console.error('Failed to create user', error);
        },
      });

      return;
    }

    if (!this.editingUserId) {
      return;
    }

    const payload: IdentityUserUpdateDto = {
      userName: value.userName ?? '',
      name: value.name ?? '',
      surname: value.surname ?? '',
      email: value.email ?? '',
      phoneNumber: value.phoneNumber ?? '',
      password: value.password || undefined,
      isActive: value.isActive ?? true,
      lockoutEnabled: value.lockoutEnabled ?? true,
      roleNames: this.selectedRoleNames,
      concurrencyStamp: this.editingConcurrencyStamp,
    };

    this.identityUserService.update(this.editingUserId, payload).subscribe({
      next: () => {
        this.closeModal();
        this.loadUsers();
      },
      error: error => {
        console.error('Failed to update user', error);
      },
    });
  }

  deleteUser(user: IdentityUserDto): void {
    if (!user.id) {
      return;
    }

    const confirmed = window.confirm('Delete selected user?');
    if (!confirmed) {
      return;
    }

    this.identityUserService.delete(user.id).subscribe({
      next: () => {
        this.loadUsers();
      },
      error: error => {
        console.error('Failed to delete user', error);
      },
    });
  }
}
