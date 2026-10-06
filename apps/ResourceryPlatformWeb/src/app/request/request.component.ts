import { Component, OnDestroy, OnInit } from '@angular/core';
import { LocalizationService } from '@abp/ng.core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { WorkflowService } from '../proxy/workflow/services/workflow.service';
import { ServiceDto } from '../proxy/workflow/services/models';
import { RequestService } from '../proxy/workflow/requests/request.service';
import { RequestStatus, RequestType } from '@proxy/workflow/requests';
import { AppPopupService } from '../shared/services/app-popup.service';
import { Router } from '@angular/router';

export interface MeetingRequirementOption {
  key: string;
  label: string;
}

export interface MeetingRequirementGroup {
  key: string;
  label: string;
  options: MeetingRequirementOption[];
  checked: boolean;
}

export interface MeetingRequirementEntry {
  selected: boolean;
  days: number | null;
  quantity: number | null;
  budget: number | null;
  startDate: string;
  endDate: string;
  remarks: string;
}

@Component({
  selector: 'app-request',
  templateUrl: './request.component.html',
  styleUrls: ['./request.component.scss'],
})
export class RequestComponent implements OnInit, OnDestroy {
  requestForm: FormGroup;
  services: ServiceDto[] = [];
  selectedServiceCode: string = '';
  openAccordionItems = new Set<number>([1, 2, 3]);
  meetingRequirementGroups: MeetingRequirementGroup[] = [];
  meetingRequirementEntries: Record<string, Record<string, MeetingRequirementEntry>> = {};
  showMeetingPreviewModal = false;
  showPostSubmitModal = false;
  private postSubmitModalTimer: ReturnType<typeof setTimeout> | null = null;

  constructor(
    private fb: FormBuilder,
    private workflowService: WorkflowService,
    private requestService: RequestService,
    private appPopupService: AppPopupService,
    private localizationService: LocalizationService,
    private router: Router
  ) {
    this.requestForm = this.fb.group({
      ServiceId: ['', Validators.required],
      Description: ['', Validators.required],
      RequestType: [''],
      RequestStatus: [''],
      DocumentSetUrl: [''],
    });
  }

  ngOnInit(): void {
    this.loadServices();
    this.buildMeetingRequirementGroups();
  }

  ngOnDestroy(): void {
    if (this.postSubmitModalTimer) {
      clearTimeout(this.postSubmitModalTimer);
      this.postSubmitModalTimer = null;
    }
  }

  loadServices(): void {
    this.workflowService.getList().subscribe((services) => {
      this.services = [...services].sort((left, right) =>
        (left.displayName ?? '').localeCompare(right.displayName ?? '', undefined, {
          sensitivity: 'base',
        })
      );
    });
  }

  toggleAccordion(itemIndex: number): void {
    if (this.openAccordionItems.has(itemIndex)) {
      this.openAccordionItems.delete(itemIndex);
      return;
    }
    this.openAccordionItems.add(itemIndex);
  }

  isAccordionOpen(itemIndex: number): boolean {
    return this.openAccordionItems.has(itemIndex);
  }
  onServiceChange(): void {
    const selectedServiceId = this.requestForm.get('ServiceId')?.value;
    const selectedService = this.services.find((s) => s.id === selectedServiceId);

    this.selectedServiceCode = selectedService?.code || '';

    this.removeDynamicGroups();

    switch (this.selectedServiceCode) {
      case 'PID':

        this.requestForm.controls['RequestType'].setValue(RequestType.InternalMemorandum);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'RLNS':
        this.requestForm.controls['RequestType'].setValue(RequestType.ReceptionLodgingOfNewStaff);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'RICR':
        this.requestForm.controls['RequestType'].setValue(RequestType.IncommingCorrespondence);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'TS':
        this.requestForm.controls['RequestType'].setValue(RequestType.Translation);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'CDW':
        this.requestForm.controls['RequestType'].setValue(RequestType.CustomsDutiesWaivers);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'AHM':
        this.requestForm.controls['RequestType'].setValue(RequestType.AccreditationOfHeadsOfMission);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'EOM':
        this.requestForm.controls['RequestType'].setValue(RequestType.ElectionObservationMission);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'EOM':
        this.requestForm.controls['RequestType'].setValue(RequestType.ElectionObservationMission);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'EOM':
        this.requestForm.controls['RequestType'].setValue(RequestType.ElectionObservationMission);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'DC':
        this.requestForm.controls['RequestType'].setValue(RequestType.DiplomaticCocktail);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'IS':
        this.requestForm.controls['RequestType'].setValue(RequestType.Interpretion);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'IS':
        this.requestForm.controls['RequestType'].setValue(RequestType.Interpretion);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'IS':
        this.requestForm.controls['RequestType'].setValue(RequestType.Interpretion);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'PCNTR':
        this.requestForm.controls['RequestType'].setValue(RequestType.ConceptNoteTermOfReference);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'GTS':
        this.requestForm.controls['RequestType'].setValue(RequestType.GroundTransportation);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'GTS':
        this.requestForm.controls['RequestType'].setValue(RequestType.GroundTransportation);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'VP':
        this.requestForm.controls['RequestType'].setValue(RequestType.VisaProcurement);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'VP':
        this.requestForm.controls['RequestType'].setValue(RequestType.VisaProcurement);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        break;
      case 'MEET':
        this.requestForm.controls['RequestType'].setValue(RequestType.Meeting);
        this.requestForm.controls['RequestStatus'].setValue(RequestStatus.Pending);
        this.requestForm.addControl('Meeting', this.buildMeetingGroup());
        break;
      default:
        break;
    }

    this.requestForm.updateValueAndValidity();

  }


  onSubmit(): void {
    this.requestForm.markAllAsTouched();

    if (this.requestForm.invalid) {
      return;
    }

    if (!this.validateMeetingRequirements()) {
      this.openAccordionItems.add(2);
      this.requestForm.get('Meeting')?.updateValueAndValidity();
      return;
    }

    if (this.selectedServiceCode === 'MEET') {
      this.showMeetingPreviewModal = true;
      return;
    }

    this.submitRequest();
  }

  confirmMeetingSubmission(): void {
    this.showMeetingPreviewModal = false;
    this.submitRequest();
  }

  cancelMeetingSubmission(): void {
    this.showMeetingPreviewModal = false;
  }

  private submitRequest(): void {
    const value = this.requestForm.value;
    const meetingPayload = value.Meeting ? { ...value.Meeting, meetingItems: this.buildMeetingItems() } : undefined;

    const requestData = {
      serviceId: value.ServiceId,
      description: value.Description,
      documentSetUrl: value.DocumentSetUrl || '',
      requestType: value.RequestType,
      requestStatus: value.RequestStatus,
      meeting: meetingPayload,
      documents: [],
    };

    this.requestService.create(requestData).subscribe({
      next: (response) => {
        console.log('Request created:', response);
        this.appPopupService.show({
          title: this.t('Workflow::Meeting:SubmitSuccessTitle', 'Request submitted'),
          message: this.t(
            'Workflow::Meeting:SubmitSuccessMessage',
            'Your request was submitted successfully.'
          ),
          tone: 'success',
          durationMs: 2500,
        });

        if (this.postSubmitModalTimer) {
          clearTimeout(this.postSubmitModalTimer);
        }

        this.postSubmitModalTimer = setTimeout(() => {
          this.showPostSubmitModal = true;
          this.postSubmitModalTimer = null;
        }, 2600);
      },
      error: (error) => {
        console.error('Error creating request:', error);
      },
    });
  }

  private removeDynamicGroups(): void {
    if (this.requestForm.contains('Meeting')) {
      this.requestForm.removeControl('Meeting');
    }
    if (this.requestForm.contains('PidForm')) {
      this.requestForm.removeControl('PidForm');
    }
  }

  private buildMeetingGroup(): FormGroup {
    return this.fb.group(
      {
        Title: ['', Validators.required],
        DepartureDate: ['', Validators.required],
        Location: ['', Validators.required],
        StartDate: ['', Validators.required],
        EndDate: ['', Validators.required],
        Type: [null, Validators.required],
        ReferenceNumber: [''],
        NumberOfParticipants: [null, [Validators.required, Validators.min(1)]],
        ContactPhone: ['', Validators.required],
        ContactEmail: ['', [Validators.required, Validators.email]],
        ContactName: ['', Validators.required],
        HostName: ['', Validators.required],
        HostDesignation: [''],
        HostPhoneNumber: ['', Validators.required],
        HostEmail: ['', [Validators.required, Validators.email]],
        CoHost1Name: [''],
        CoHost1Designation: [''],
        CoHost1PhoneNumber: [''],
        CoHost1Email: ['', Validators.email],
        CoHost2Name: [''],
        CoHost2Designation: [''],
        CoHost2PhoneNumber: [''],
        CoHost2Email: ['', Validators.email],
        GLNumberRefreshments: [''],
        GLNumberHotel: [''],
        GLNumberCarHire: [''],
        GLNumberEquipment: [''],
        GLNumberLanguageServices: [''],
        CostCenterNumberRefreshments: [''],
        CostCenterNumberHotel: [''],
        CostCenterNumberCarHire: [''],
        CostCenterNumberEquipment: [''],
        CostCenterNumberLanguageServices: [''],
      },
      {
        validators: [this.dateRangeValidator(), this.meetingRequirementsValidator()],
      }
    );
  }


  private buildPidFormGroup(): FormGroup {
    return this.fb.group({
      MemoNumber: ['', Validators.required],
      RequiresApproval: [false, Validators.requiredTrue],
    });
  }

  private dateRangeValidator(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
      const start = group.get('StartDate')?.value;
      const end = group.get('EndDate')?.value;

      if (!start || !end) {
        return null;
      }

      return new Date(start) <= new Date(end) ? null : { invalidDateRange: true };
    };
  }

  private meetingRequirementsValidator(): ValidatorFn {
    return (group: AbstractControl): ValidationErrors | null => {
      if (!this.meetingRequirementGroups.length) {
        return null;
      }

      for (const requirementGroup of this.meetingRequirementGroups) {
        const entries = this.meetingRequirementEntries[requirementGroup.key] ?? {};
        const selectedOptions = requirementGroup.options.filter(
          (option) => entries[option.key]?.selected
        );

        if (selectedOptions.length === 0) {
          continue;
        }

        for (const option of selectedOptions) {
          const entry = entries[option.key];
          const isIncomplete =
            entry.days == null ||
            entry.days <= 0 ||
            entry.quantity == null ||
            entry.quantity <= 0 ||
            entry.budget == null ||
            entry.budget <= 0 ||
            !entry.startDate ||
            !entry.endDate ||
            new Date(entry.startDate) > new Date(entry.endDate);

          if (isIncomplete) {
            return {
              invalidMeetingRequirementOption: {
                group: requirementGroup.key,
                option: option.key,
              },
            };
          }
        }
      }

      return null;
    };
  }

  get meetingGroup(): FormGroup | null {
    return this.requestForm.get('Meeting') as FormGroup | null;
  }

  get pidGroup(): FormGroup | null {
    return this.requestForm.get('PidForm') as FormGroup | null;
  }

  private buildMeetingItems(): Array<{ 
    itemName: string;
    itemCode: string;
    category: string;
    serviceCenterCode: string;
    quantityNo: number | null;
    periodFrom: string;
    periodTo: string;
    budget: number | null;
    remarkObservation: string;
  }> {
    const items: Array<{ 
      itemName: string;
      itemCode: string;
      category: string;
      serviceCenterCode: string;
      quantityNo: number | null;
      periodFrom: string;
      periodTo: string;
      budget: number | null;
      remarkObservation: string;
    }> = [];

    this.meetingRequirementGroups.forEach((group) => {
      group.options.forEach((option) => {
        const entry = this.meetingRequirementEntries[group.key]?.[option.key];
        if (!entry?.selected) {
          return;
        }

        items.push({
          itemName: option.label,
          itemCode: option.key,
          category: group.key,
          serviceCenterCode: group.key,
          quantityNo: entry.quantity,
          periodFrom: entry.startDate,
          periodTo: entry.endDate,
          budget: entry.budget,
          remarkObservation: entry.remarks ?? '',
        });
      });
    });

    return items;
  }

  private buildMeetingRequirementGroups(): void {
    const groups: MeetingRequirementGroup[] = [
      {
        key: 'venue',
        label: 'Administration::Meeting:RequirementVenue',
        checked: false,
        options: [
          { key: 'meetingHall', label: 'Administration::Meeting:ItemHall' },
          { key: 'committeeRoom', label: 'Administration::Meeting:ItemCommitteeRoom' },
          { key: 'auditorium', label: 'Administration::Meeting:ItemAuditorium' },
          { key: 'secretariat', label: 'Administration::Meeting:ItemSecretariat' },
          { key: 'translationRoom', label: 'Administration::Meeting:ItemTranslationRoom' },
        ],
      },
      {
        key: 'hotel',
        label: 'Administration::Meeting:RequirementHotel',
        checked: false,
        options: [
          { key: 'singleRoom', label: 'Administration::Meeting:ItemHotelSingle' },
          { key: 'suite', label: 'Administration::Meeting:ItemHotelSuite' },
        ],
      },
      {
        key: 'languageServices',
        label: 'Administration::Meeting:RequirementLanguage',
        checked: false,
        options: [
          { key: 'simultaneousInterpretationEquipment', label: 'Administration::Meeting:ItemSimultaneousInterpretationEquipment' },
          { key: 'interpreterEnglish', label: 'Administration::Meeting:ItemInterpreterEnglish' },
          { key: 'interpreterFrench', label: 'Administration::Meeting:ItemInterpreterFrench' },
          { key: 'interpreterPortuguese', label: 'Administration::Meeting:ItemInterpreterPortuguese' },
          { key: 'translatorEnglish', label: 'Administration::Meeting:ItemTranslatorEnglish' },
          { key: 'translatorFrench', label: 'Administration::Meeting:ItemTranslatorFrench' },
          { key: 'translatorPortuguese', label: 'Administration::Meeting:ItemTranslatorPortuguese' },
        ],
      },
      {
        key: 'equipment',
        label: 'Administration::Meeting:RequirementEquipment',
        checked: false,
        options: [
          { key: 'computer', label: 'Administration::Meeting:ItemComputerPrinter' },
          { key: 'printer', label: 'Administration::Meeting:ItemComputerPrinter' },
          { key: 'photocopier', label: 'Administration::Meeting:ItemPhotocopierScanner' },
          { key: 'scanner', label: 'Administration::Meeting:ItemPhotocopierScanner' },
          { key: 'projector', label: 'Administration::Meeting:ItemProjector' },
          { key: 'stationery', label: 'Administration::Meeting:ItemStationery' },
        ],
      },
      {
        key: 'refreshments',
        label: 'Administration::Meeting:RequirementRefreshment',
        checked: false,
        options: [
          { key: 'teaCoffeeMorning', label: 'Administration::Meeting:ItemTeaCoffeeMorning' },
          { key: 'teaCoffeeAfternoon', label: 'Administration::Meeting:ItemTeaCoffeeAfternoon' },
          { key: 'lunch', label: 'Administration::Meeting:ItemLunch' },
          { key: 'cocktail', label: 'Administration::Meeting:ItemRefreshment' },
          { key: 'dinner', label: 'Administration::Meeting:ItemDinner' },
        ],
      },
      {
        key: 'transportation',
        label: 'Administration::Meeting:RequirementTransport',
        checked: false,
        options: [
          { key: 'saloonCar', label: 'Administration::Meeting:ItemSaloonCar' },
          { key: 'seater15', label: 'Administration::Meeting:Item15SeaterBus' },
          { key: 'seater30', label: 'Administration::Meeting:Item30SeaterBus' },
          { key: 'executiveCar', label: 'Administration::Meeting:ItemExecutiveCar' },
        ],
      },
      {
        key: 'localAssistant',
        label: 'Administration::Meeting:RequirementLocalAssistance',
        checked: false,
        options: [
          { key: 'hostess', label: 'Administration::Meeting:ItemHostess' },
          { key: 'airportAssistant', label: 'Administration::Meeting:ItemAirportAssistance' },
          { key: 'reproductionAssistant', label: 'Administration::Meeting:ItemReproductionAssistant' },
          { key: 'itAssistant', label: 'Administration::Meeting:ItemITAssistant' },
        ],
      },
    ];

    this.meetingRequirementGroups = groups;
    this.meetingRequirementGroups.forEach((group) => {
      this.meetingRequirementEntries[group.key] = {};
      group.options.forEach((option) => {
        this.meetingRequirementEntries[group.key][option.key] = {
          selected: false,
          days: null,
          quantity: null,
          budget: null,
          startDate: '',
          endDate: '',
          remarks: '',
        };
      });
    });
  }

  toggleMeetingRequirement(groupKey: string): void {
    const group = this.meetingRequirementGroups.find((item) => item.key === groupKey);
    if (!group) {
      return;
    }

    group.checked = !group.checked;
    if (!group.checked) {
      group.options.forEach((option) => {
        const entry = this.meetingRequirementEntries[group.key]?.[option.key];
        if (!entry) {
          return;
        }

        entry.selected = false;
        entry.days = null;
        entry.quantity = null;
        entry.budget = null;
        entry.startDate = '';
        entry.endDate = '';
        entry.remarks = '';
      });
    }

    this.requestForm.get('Meeting')?.updateValueAndValidity();
  }

  isMeetingRequirementChecked(groupKey: string): boolean {
    const group = this.meetingRequirementGroups.find((item) => item.key === groupKey);
    return !!group?.checked;
  }

  hasMeetingRequirementOption(groupKey: string, optionKey: string): boolean {
    return !!this.meetingRequirementEntries[groupKey]?.[optionKey]?.selected;
  }

  isMeetingOptionChecked(groupKey: string, optionKey: string): boolean {
    return this.hasMeetingRequirementOption(groupKey, optionKey);
  }

  toggleMeetingRequirementOption(groupKey: string, optionKey: string): void {
    const group = this.meetingRequirementGroups.find((item) => item.key === groupKey);
    const entry = this.meetingRequirementEntries[groupKey]?.[optionKey];
    if (!group || !entry) {
      return;
    }

    if (!group.checked) {
      group.checked = true;
    }

    entry.selected = !entry.selected;
    if (!entry.selected) {
      entry.days = null;
      entry.quantity = null;
      entry.budget = null;
      entry.startDate = '';
      entry.endDate = '';
      entry.remarks = '';
    }

    this.refreshMeetingRequirementValidation();
  }

  refreshMeetingRequirementValidation(): void {
    this.requestForm.get('Meeting')?.updateValueAndValidity();
  }

  isMeetingOptionSelected(groupKey: string, optionKey: string): boolean {
    return this.hasMeetingRequirementOption(groupKey, optionKey);
  }

  trackMeetingRequirementGroup(index: number, group: MeetingRequirementGroup): string {
    return group.key;
  }

  trackMeetingRequirementOption(index: number, option: MeetingRequirementOption): string {
    return option.key;
  }

  meetingRequirementGroupLabel(groupKey: string): string {
    const group = this.meetingRequirementGroups.find((item) => item.key === groupKey);
    return group?.label ?? '';
  }

  meetingRequirementOptionLabel(groupKey: string, optionKey: string): string {
    const group = this.meetingRequirementGroups.find((item) => item.key === groupKey);
    return group?.options.find((item) => item.key === optionKey)?.label ?? '';
  }

  validateMeetingRequirements(): boolean {
    let isValid = true;

    this.meetingRequirementGroups.forEach((group) => {
      const hasSelectedOption = group.options.some(
        (option) => !!this.meetingRequirementEntries[group.key]?.[option.key]?.selected
      );

      if (!group.checked && hasSelectedOption) {
        isValid = false;
        return;
      }

      if (!group.checked) {
        return;
      }

      group.options.forEach((option) => {
        const entry = this.meetingRequirementEntries[group.key]?.[option.key];
        if (!entry?.selected) {
          return;
        }

        const isChildIncomplete =
          entry.days == null ||
          entry.days <= 0 ||
          entry.quantity == null ||
          entry.quantity <= 0 ||
          entry.budget == null ||
          entry.budget <= 0 ||
          !entry.startDate ||
          !entry.endDate ||
          new Date(entry.startDate) > new Date(entry.endDate);

        if (isChildIncomplete) {
          isValid = false;
        }
      });
    });

    return isValid;
  }

  hasMeetingRequirementValidationError(groupKey: string, optionKey: string): boolean {
    const group = this.meetingRequirementGroups.find((item) => item.key === groupKey);
    const entry = this.meetingRequirementEntries[groupKey]?.[optionKey];
    if (!group || !entry?.selected || !group.checked) {
      return false;
    }

    return (
      entry.days == null ||
      entry.days <= 0 ||
      entry.quantity == null ||
      entry.quantity <= 0 ||
      entry.budget == null ||
      entry.budget <= 0 ||
      !entry.startDate ||
      !entry.endDate ||
      new Date(entry.startDate) > new Date(entry.endDate)
    );
  }

  hasMeetingControlError(controlName: string): boolean {
    const control = this.meetingGroup?.get(controlName);
    return !!control && control.invalid && (control.dirty || control.touched);
  }

  meetingControlErrorMessage(controlName: string): string {
    const control = this.meetingGroup?.get(controlName);
    if (!control || !control.errors) {
      return '';
    }

    if (control.errors['required']) {
      return this.t('Workflow::Meeting:ValidationRequired', 'This field is required.');
    }

    if (control.errors['email']) {
      return this.t(
        'Workflow::Meeting:ValidationEmail',
        'Please enter a valid email address.'
      );
    }

    if (control.errors['min']) {
      return this.t(
        'Workflow::Meeting:ValidationMinGreaterThanZero',
        'Please enter a value greater than 0.'
      );
    }

    return this.t('Workflow::Meeting:ValidationGeneric', 'Please check this field.');
  }

  shouldShowMeetingDateRangeError(): boolean {
    const group = this.meetingGroup;
    return !!group && group.hasError('invalidDateRange') && (group.touched || group.dirty);
  }

  startNewRequest(): void {
    this.showPostSubmitModal = false;
    this.showMeetingPreviewModal = false;
    this.requestForm.reset({
      ServiceId: '',
      Description: '',
      RequestType: '',
      RequestStatus: '',
      DocumentSetUrl: '',
    });
    this.selectedServiceCode = '';
    this.openAccordionItems = new Set<number>([1, 2, 3]);
    this.removeDynamicGroups();
    this.buildMeetingRequirementGroups();
  }

  goToDashboard(): void {
    this.showPostSubmitModal = false;
    this.router.navigate(['/dashboard']);
  }

  private t(key: string, fallback: string): string {
    const value = this.localizationService.instant(key);
    return value && value !== key ? value : fallback;
  }

  get meetingPreviewSummary(): { title: string; location: string; startDate: string; endDate: string; participants: number | null; requirements: string[] } {
    const meeting = this.requestForm.get('Meeting')?.value ?? {};
    const requirements = this.meetingRequirementGroups.reduce<string[]>((result, group) => {
      const selectedOptions = group.options.filter(
        (option) => this.meetingRequirementEntries[group.key]?.[option.key]?.selected
      );

      selectedOptions.forEach((option) => {
        result.push(this.meetingRequirementOptionLabel(group.key, option.key));
      });

      return result;
    }, []);

    return {
      title: meeting.Title ?? '',
      location: meeting.Location ?? '',
      startDate: meeting.StartDate ?? '',
      endDate: meeting.EndDate ?? '',
      participants: meeting.NumberOfParticipants ?? null,
      requirements,
    };
  }
}


