import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
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

  constructor(
    private fb: FormBuilder,
    private workflowService: WorkflowService,
    private requestService: RequestService
  ) {
    this.requestForm = this.fb.group({
      requestService: ['', Validators.required],
      requestDescription: ['', Validators.required],
      requestType: [''],
      requestStatus: [''] ,
      documentSetUrl: ['']
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

  onSubmit(): void {
    if (this.requestForm.valid) {
      const formValue = this.requestForm.value;
      const requestData = {
        serviceId: formValue.requestService,
        description: formValue.requestDescription,
        documentSetUrl: "", 
        requestType: formValue.requestType, 
        requestStatus: formValue.requestStatus, 
        documents: []
      };
      console.log('Request Data to be submitted:', requestData);
      this.requestService.create(requestData).subscribe({
        next: (response) => {
          console.log('Request created:', response);
          // Handle success, e.g., navigate or show message
        },
        error: (error) => {
          console.error('Error creating request:', error);
          // Handle error
        }
      });
    }
  }
   onServiceChange(): void {
    const selectedServiceId = this.requestForm.get('requestService')?.value;
    const selectedService = this.services.find(s => s.id === selectedServiceId);
         this.selectedServiceCode = selectedService?.code || '';

    if (selectedService && selectedService.code) {
     
    let reqType: RequestType | null = null;
      switch (selectedService.code) {
        case 'PID':
          reqType = RequestType.InternalMemorandum;
          this.requestForm.controls['requestType'].setValue(RequestType.InternalMemorandum);
          break;      
       case 'RLNS':
          reqType = RequestType.ReceptionLodgingOfNewStaff;
          this.requestForm.controls['requestType'].setValue(RequestType.ReceptionLodgingOfNewStaff);
          break;
       case 'RICR':
          reqType = RequestType.IncommingCorrespondence;
          break;
       case 'TS':
          reqType = RequestType.Translation;
          break;
       case 'CDW':
          reqType = RequestType.CustomsDutiesWaivers;
          break;
       case 'AHM':
          reqType = RequestType.AccreditationOfHeadsOfMission;
          break;
       case 'EOM':
          reqType = RequestType.ElectionObservationMission;
          break;
       case 'DC':
          reqType = RequestType.DiplomaticCocktail;
          break;
       case 'IS':
          reqType = RequestType.Interpretion;
          break;
       case 'PCNTR':
          reqType = RequestType.ConceptNoteTermOfReference;
          break;
       case 'GTS':
          reqType = RequestType.GroundTransportation;
          break;
       case 'VP':
          reqType = RequestType.VisaProcurement;
          break;
       case 'MEET':
          reqType = RequestType.Meeting;
          break;       
        default:
          reqType = null; // Or set a default if applicable
          break;
      }

      this.requestForm.patchValue({
        requestType: reqType,
      requestStatus: RequestStatus.Pending
      });
    }
  }
}