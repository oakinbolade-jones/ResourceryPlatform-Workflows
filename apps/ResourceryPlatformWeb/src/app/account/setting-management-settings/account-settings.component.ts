import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import {
  EmailSettingsService,
  TimeZoneSettingsService,
  UpdateEmailSettingsDto,
} from '@abp/ng.setting-management/proxy';

type TimeZoneOption = {
  name?: string;
  value?: string;
};

@Component({
  selector: 'app-account-settings',
  templateUrl: './account-settings.component.html',
})
export class AccountSettingsComponent implements OnInit {
  loading = false;
  savingEmail = false;
  savingTimeZone = false;
  sendingTest = false;

  timezones: TimeZoneOption[] = [];

  emailSettingsForm = this.fb.group({
    smtpHost: [''],
    smtpPort: [587, [Validators.required]],
    smtpUserName: [''],
    smtpPassword: [''],
    smtpDomain: [''],
    smtpEnableSsl: [true],
    smtpUseDefaultCredentials: [false],
    defaultFromAddress: ['', [Validators.required, Validators.email]],
    defaultFromDisplayName: ['', [Validators.required]],
  });

  timeZoneForm = this.fb.group({
    timeZone: ['', [Validators.required]],
  });

  testEmailForm = this.fb.group({
    senderEmailAddress: ['', [Validators.required, Validators.email]],
    targetEmailAddress: ['', [Validators.required, Validators.email]],
    subject: ['', [Validators.required]],
    body: [''],
  });

  constructor(
    private fb: FormBuilder,
    private emailSettingsService: EmailSettingsService,
    private timeZoneSettingsService: TimeZoneSettingsService
  ) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll(): void {
    this.loading = true;

    this.emailSettingsService.get().subscribe({
      next: data => {
        this.emailSettingsForm.patchValue({
          smtpHost: data.smtpHost ?? '',
          smtpPort: data.smtpPort,
          smtpUserName: data.smtpUserName ?? '',
          smtpPassword: '',
          smtpDomain: data.smtpDomain ?? '',
          smtpEnableSsl: data.smtpEnableSsl,
          smtpUseDefaultCredentials: data.smtpUseDefaultCredentials,
          defaultFromAddress: data.defaultFromAddress ?? '',
          defaultFromDisplayName: data.defaultFromDisplayName ?? '',
        });

        this.testEmailForm.patchValue({
          senderEmailAddress: data.defaultFromAddress ?? '',
          targetEmailAddress: data.defaultFromAddress ?? '',
          subject: 'Test Email',
        });
      },
      error: error => {
        console.error('Failed to load email settings', error);
      },
    });

    this.timeZoneSettingsService.get().subscribe({
      next: value => {
        this.timeZoneForm.patchValue({ timeZone: value || '' });
      },
      error: error => {
        console.error('Failed to load current timezone', error);
      },
      complete: () => {
        this.loading = false;
      },
    });

    this.timeZoneSettingsService.getTimezones().subscribe({
      next: items => {
        this.timezones = (items as unknown as TimeZoneOption[]) ?? [];
      },
      error: error => {
        console.error('Failed to load timezones', error);
      },
    });
  }

  saveEmailSettings(): void {
    if (this.emailSettingsForm.invalid) {
      this.emailSettingsForm.markAllAsTouched();
      return;
    }

    const value = this.emailSettingsForm.value;

    const payload: UpdateEmailSettingsDto = {
      smtpHost: value.smtpHost ?? '',
      smtpPort: value.smtpPort ?? 587,
      smtpUserName: value.smtpUserName ?? '',
      smtpPassword: value.smtpPassword ?? '',
      smtpDomain: value.smtpDomain ?? '',
      smtpEnableSsl: value.smtpEnableSsl ?? true,
      smtpUseDefaultCredentials: value.smtpUseDefaultCredentials ?? false,
      defaultFromAddress: value.defaultFromAddress ?? '',
      defaultFromDisplayName: value.defaultFromDisplayName ?? '',
    };

    this.savingEmail = true;
    this.emailSettingsService.update(payload).subscribe({
      next: () => {
        this.testEmailForm.patchValue({
          senderEmailAddress: payload.defaultFromAddress,
        });
      },
      error: error => {
        console.error('Failed to update email settings', error);
      },
      complete: () => {
        this.savingEmail = false;
      },
    });
  }

  saveTimeZone(): void {
    if (this.timeZoneForm.invalid) {
      this.timeZoneForm.markAllAsTouched();
      return;
    }

    const timeZone = this.timeZoneForm.value.timeZone ?? '';

    this.savingTimeZone = true;
    this.timeZoneSettingsService.update(timeZone).subscribe({
      error: error => {
        console.error('Failed to update timezone', error);
      },
      complete: () => {
        this.savingTimeZone = false;
      },
    });
  }

  sendTestEmail(): void {
    if (this.testEmailForm.invalid) {
      this.testEmailForm.markAllAsTouched();
      return;
    }

    const payload = {
      senderEmailAddress: this.testEmailForm.value.senderEmailAddress ?? '',
      targetEmailAddress: this.testEmailForm.value.targetEmailAddress ?? '',
      subject: this.testEmailForm.value.subject ?? '',
      body: this.testEmailForm.value.body ?? '',
    };

    this.sendingTest = true;
    this.emailSettingsService.sendTestEmail(payload).subscribe({
      error: error => {
        console.error('Failed to send test email', error);
      },
      complete: () => {
        this.sendingTest = false;
      },
    });
  }
}
