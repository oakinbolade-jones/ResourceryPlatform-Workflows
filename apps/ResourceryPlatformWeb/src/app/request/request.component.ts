import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { WorkflowService } from '../proxy/workflow/services/workflow.service';
import { ServiceDto } from '../proxy/workflow/services/models';
import { RequestService } from '../proxy/workflow/requests/request.service';
import { RequestStatus, RequestType } from '@proxy/workflow/requests';

@Component({
  selector: 'app-request',
  templateUrl: './request.component.html',
  styleUrls: ['./request.component.scss'],
})
export class RequestComponent implements OnInit {
  requestForm: FormGroup;
  services: ServiceDto[] = [];
  selectedServiceCode: string = '';
  openAccordionItems = new Set<number>([1, 2, 3]);

  constructor(
    private fb: FormBuilder,
    private workflowService: WorkflowService,
    private requestService: RequestService
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
  }

  loadServices(): void {
    this.workflowService.getList().subscribe((services) => {
      this.services = services;
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
        this.requestForm.addControl('MeetingForm', this.buildMeetingFormGroup());
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

    const value = this.requestForm.value;

    const requestData = {
      serviceId: value.ServiceId,
      description: value.Description,
      documentSetUrl: value.DocumentSetUrl || '',
      requestType: value.RequestType,
      requestStatus: value.RequestStatus,
      meetingForm: value.MeetingForm ?? undefined,
      documents: [],
    };

    this.requestService.create(requestData).subscribe({
      next: (response) => {
        console.log('Request created:', response);
      },
      error: (error) => {
        console.error('Error creating request:', error);
      },
    });
  }

  private removeDynamicGroups(): void {
    if (this.requestForm.contains('MeetingForm')) {
      this.requestForm.removeControl('MeetingForm');
    }
    if (this.requestForm.contains('PidForm')) {
      this.requestForm.removeControl('PidForm');
    }
  }

  private buildMeetingFormGroup(): FormGroup {
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
      { validators: [this.dateRangeValidator()] }
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

  get meetingGroup(): FormGroup | null {
    return this.requestForm.get('MeetingForm') as FormGroup | null;
  }

  get pidGroup(): FormGroup | null {
    return this.requestForm.get('PidForm') as FormGroup | null;
  }


}