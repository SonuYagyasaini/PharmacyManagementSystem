import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { Subscription } from 'rxjs';
import { PharmacyService } from '../../services/pharmacy.service';
import { Medicine } from '../../models/medicine.model';

type SortColumn = 'fullName' | 'brand' | 'quantity' | 'price' | 'expiryDate' | 'createdAtUtc' | 'status';
type SortDirection = 'asc' | 'desc';

@Component({
  selector: 'medicine-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './medicine-list.component.html',
  styleUrls: ['./medicine-list.component.scss'],
})
export class MedicineListComponent implements OnInit, OnDestroy {
  medicines: Medicine[] = [];
  loading = false;
  search = '';
  error?: string;
  saleQuantities: Record<string, number> = {};
  private refreshSub?: Subscription;
  sortBy: SortColumn = 'createdAtUtc';
  sortDirection: SortDirection = 'desc';

  constructor(private pharmacy: PharmacyService, private router: Router) {}

  ngOnInit(): void {
    this.loadMedicines();
    // Auto-reload list when navigating back to this component
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd && event.url === '/') {
        this.loadMedicines();
      }
    });
    // Reload when service emits refresh events (create/update/delete/sale)
    this.refreshSub = this.pharmacy.refresh$.subscribe(() => this.loadMedicines());
  }

  ngOnDestroy(): void {
    this.refreshSub?.unsubscribe();
  }

  loadMedicines(): void {
    this.loading = true;
    this.error = undefined;
    // Always load without search parameter unless search is explicitly set
    const searchQuery = this.search?.trim() || undefined;
    this.pharmacy.getMedicines(searchQuery, this.sortBy, this.sortDirection).subscribe({
      next: (medicines) => {
        this.medicines = Array.isArray(medicines) ? medicines : [];
        this.loading = false;
      },
      error: (err) => {
        console.error('Medicine load error:', err);
        this.error = err?.error?.message || err?.message || 'Unable to load medicines';
        this.medicines = [];
        this.loading = false;
      },
    });
  }

  searchMedicines(): void {
    if (this.search?.trim()) {
      this.loadMedicines();
    }
  }

  resetSearch(): void {
    this.search = '';
    this.loadMedicines();
  }

  sortByColumn(column: SortColumn): void {
    if (this.sortBy === column) {
      this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortBy = column;
      this.sortDirection = 'asc';
    }
    this.loadMedicines();
  }

  getSortIndicator(column: SortColumn): string {
    if (this.sortBy !== column) {
      return '↕';
    }
    return this.sortDirection === 'asc' ? '↑' : '↓';
  }

  deleteMedicine(id: string): void {
    if (!confirm('Delete this medicine permanently?')) {
      return;
    }
    this.pharmacy.deleteMedicine(id).subscribe({
      next: () => this.loadMedicines(),
      error: (err) => (this.error = err?.message || 'Unable to delete medicine'),
    });
  }

  recordSale(medicine: Medicine): void {
    const quantity = this.saleQuantities[medicine.id] ?? 1;
    if (quantity < 1) {
      this.error = 'Sale quantity must be at least 1';
      return;
    }
    if (quantity > medicine.quantity) {
      this.error = 'Sale quantity cannot exceed available stock';
      return;
    }

    this.pharmacy
      .recordSale({ medicineId: medicine.id, quantity })
      .subscribe({
        next: () => {
          this.saleQuantities[medicine.id] = 1;
          this.loadMedicines();
        },
        error: (err) => (this.error = err?.message || 'Unable to record sale'),
      });
  }

  getRowClasses(medicine: Medicine): string {
    if (medicine.highlightColor === 'red' || medicine.isExpiringWithin30Days) {
      return 'row-expiring';
    }
    if (medicine.highlightColor === 'yellow' || medicine.isLowStock) {
      return 'row-low-stock';
    }
    return '';
  }

  viewMedicine(id: string): void {
    this.router.navigate(['/medicine/view', id]);
  }

  editMedicine(id: string): void {
    this.router.navigate(['/medicine/edit', id]);
  }
}
