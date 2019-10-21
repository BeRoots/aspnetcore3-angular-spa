import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoletestComponent } from './roletest.component';

describe('RoletestComponent', () => {
  let component: RoletestComponent;
  let fixture: ComponentFixture<RoletestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoletestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoletestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
