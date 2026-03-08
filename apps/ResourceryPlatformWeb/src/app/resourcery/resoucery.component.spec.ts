import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ResourceryComponent } from './resourcery.component';

describe('ResourceryComponent', () => {
  let component: ResourceryComponent;
  let fixture: ComponentFixture<ResourceryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ResourceryComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ResourceryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
