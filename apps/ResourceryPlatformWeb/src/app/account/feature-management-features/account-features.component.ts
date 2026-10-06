import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import {
  FeatureDto,
  FeatureGroupDto,
  FeaturesService,
  UpdateFeatureDto,
} from '@abp/ng.feature-management/proxy';

type FeatureRow = {
  groupName: string;
  groupDisplayName: string;
  feature: FeatureDto;
};

@Component({
  selector: 'app-account-features',
  templateUrl: './account-features.component.html',
})
export class AccountFeaturesComponent implements OnInit {
  providerName = 'T';
  providerKey = '';
  loading = false;
  modalVisible = false;
  saving = false;

  groups: FeatureGroupDto[] = [];
  rows: FeatureRow[] = [];
  filteredRows: FeatureRow[] = [];
  search = '';
  editingFeature: FeatureDto | null = null;

  featureForm = this.fb.group({
    value: ['', [Validators.required]],
    boolValue: [false],
  });

  constructor(
    private fb: FormBuilder,
    private featuresService: FeaturesService
  ) {}

  ngOnInit(): void {
    this.loadFeatures();
  }

  loadFeatures(): void {
    this.loading = true;
    this.featuresService.get(this.providerName, this.providerKey).subscribe({
      next: result => {
        this.groups = result.groups ?? [];
        this.rows = [];
        this.groups.forEach(group => {
          (group.features ?? []).forEach(feature => {
            this.rows.push({
              groupName: group.name ?? '',
              groupDisplayName: group.displayName ?? group.name ?? '',
              feature,
            });
          });
        });
        this.applyFilter();
      },
      error: error => {
        console.error('Failed to load features', error);
      },
      complete: () => {
        this.loading = false;
      },
    });
  }

  applyFilter(): void {
    const term = this.search.trim().toLowerCase();
    if (!term) {
      this.filteredRows = [...this.rows];
      return;
    }

    this.filteredRows = this.rows.filter(x => {
      const name = x.feature.name ?? '';
      const display = x.feature.displayName ?? '';
      const group = x.groupDisplayName ?? '';
      return `${name} ${display} ${group}`.toLowerCase().includes(term);
    });
  }

  openEditModal(row: FeatureRow): void {
    this.editingFeature = row.feature;
    const isToggle = this.isToggleFeature(row.feature);
    this.featureForm.reset({
      value: row.feature.value ?? '',
      boolValue: (row.feature.value ?? '').toLowerCase() === 'true',
    });

    if (isToggle) {
      this.featureForm.get('value')?.clearValidators();
    } else {
      this.featureForm.get('value')?.setValidators([Validators.required]);
    }

    this.featureForm.get('value')?.updateValueAndValidity();
    this.modalVisible = true;
  }

  closeModal(): void {
    this.modalVisible = false;
    this.editingFeature = null;
  }

  isToggleFeature(feature: FeatureDto): boolean {
    return (feature.valueType?.name ?? '').includes('ToggleStringValueType');
  }

  saveFeature(): void {
    if (!this.editingFeature) {
      return;
    }

    const isToggle = this.isToggleFeature(this.editingFeature);

    if (!isToggle && this.featureForm.invalid) {
      this.featureForm.markAllAsTouched();
      return;
    }

    const nextValue = isToggle
      ? String(this.featureForm.value.boolValue ?? false)
      : this.featureForm.value.value ?? '';

    const payload: { features: UpdateFeatureDto[] } = {
      features: [
        {
          name: this.editingFeature.name,
          value: nextValue,
        },
      ],
    };

    this.saving = true;
    this.featuresService.update(this.providerName, this.providerKey, payload).subscribe({
      next: () => {
        this.closeModal();
        this.loadFeatures();
      },
      error: error => {
        console.error('Failed to update feature', error);
      },
      complete: () => {
        this.saving = false;
      },
    });
  }
}
