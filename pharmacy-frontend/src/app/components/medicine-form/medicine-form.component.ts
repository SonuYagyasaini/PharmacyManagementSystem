import { Component, OnInit, inject } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { PharmacyService } from '../../services/pharmacy.service';
import { CreateMedicineRequest, Medicine, UpdateMedicineRequest } from '../../models/medicine.model';

@Component({
  selector: 'medicine-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './medicine-form.component.html',
  styleUrls: ['./medicine-form.component.scss'],
})
export class MedicineFormComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly pharmacy = inject(PharmacyService);

  form = inject(FormBuilder).nonNullable.group({
    fullName: ['', Validators.required],
    brand: ['', Validators.required],
    quantity: [0, [Validators.required, Validators.min(0)]],
    price: [0, [Validators.required, Validators.min(0)]],
    expiryDate: ['', Validators.required],
    notes: [''],
  });

  mode: 'create' | 'edit' | 'view' = 'create';
  medicineId?: string;
  loading = false;
  error?: string;
  successMessage?: string;

  ngOnInit(): void {
    const mode = this.route.snapshot.data['mode'] as 'create' | 'edit' | 'view';
    this.mode = mode ?? 'create';
    this.medicineId = this.route.snapshot.paramMap.get('id') ?? undefined;

    if (this.mode !== 'create' && this.medicineId) {
      this.loading = true;
      this.pharmacy.getMedicine(this.medicineId).subscribe({
        next: (medicine) => this.populateForm(medicine),
        error: (err) => {
          this.error = err?.message || 'Unable to load medicine';
          this.loading = false;
        },
      });
    }
  }

  private populateForm(medicine: Medicine): void {
    this.form.setValue({
      fullName: medicine.fullName,
      brand: medicine.brand,
      quantity: medicine.quantity,
      price: medicine.price,
      expiryDate: medicine.expiryDate,
      notes: medicine.notes ?? '',
    });
    if (this.mode === 'view') {
      this.form.disable();
    }
    this.loading = false;
  }

  submit(): void {
    if (this.form.invalid) {
      this.error = 'Please complete the form fields correctly.';
      return;
    }

    this.loading = true;
    this.error = undefined;
    const payload: CreateMedicineRequest | UpdateMedicineRequest = this.form.getRawValue();

    const request$ =
      this.mode === 'edit' && this.medicineId
        ? this.pharmacy.updateMedicine(this.medicineId, payload)
        : this.pharmacy.createMedicine(payload);

    request$.subscribe({
      next: () => {
        this.successMessage = this.mode === 'edit' ? 'Medicine updated successfully.' : 'Medicine added successfully.';
        this.loading = false;
        // Navigate immediately without delay for instant list refresh
        this.router.navigate(['/']);
      },
      error: (err) => {
        this.error = err?.message || 'Unable to save medicine';
        this.loading = false;
      },
    });
  }

  get title(): string {
    return this.mode === 'create' ? 'Add Medicine' : this.mode === 'edit' ? 'Edit Medicine' : 'View Medicine';
  }

  get isViewMode(): boolean {
    return this.mode === 'view';
  }

  cancel(): void {
    this.router.navigate(['/']);
  }
}
