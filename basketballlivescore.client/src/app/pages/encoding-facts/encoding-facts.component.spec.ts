import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EncodingFactsComponent } from './encoding-facts.component';

describe('EncodingFactsComponent', () => {
  let component: EncodingFactsComponent;
  let fixture: ComponentFixture<EncodingFactsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [EncodingFactsComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(EncodingFactsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
