import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, Subject } from 'rxjs';
import { tap } from 'rxjs/operators';
import {
  CreateMedicineRequest,
  CreateSaleRequest,
  Medicine,
  SaleResponse,
  UpdateMedicineRequest,
} from '../models/medicine.model';

const API_BASE_URL = 'http://127.0.0.1:5170/api';

@Injectable({ providedIn: 'root' })
export class PharmacyService {
  private readonly refreshSubject = new Subject<void>();
  readonly refresh$ = this.refreshSubject.asObservable();
  private readonly medicinesUrl = `${API_BASE_URL}/medicines`;
  private readonly salesUrl = `${API_BASE_URL}/sales`;

  constructor(private readonly http: HttpClient) {}

  getMedicines(
    search?: string,
    sortBy?: string,
    sortDirection?: 'asc' | 'desc'
  ): Observable<Medicine[]> {
    let params = new HttpParams();
    if (search) {
      params = params.set('search', search);
    }
    if (sortBy) {
      params = params.set('sortBy', sortBy);
    }
    if (sortDirection) {
      params = params.set('sortDirection', sortDirection);
    }
    return this.http.get<Medicine[]>(this.medicinesUrl, { params });
  }

  getMedicine(id: string): Observable<Medicine> {
    return this.http.get<Medicine>(`${this.medicinesUrl}/${id}`);
  }

  createMedicine(request: CreateMedicineRequest): Observable<Medicine> {
    return this.http.post<Medicine>(this.medicinesUrl, request).pipe(
      tap(() => this.refreshSubject.next())
    );
  }

  updateMedicine(id: string, request: UpdateMedicineRequest): Observable<Medicine> {
    return this.http.put<Medicine>(`${this.medicinesUrl}/${id}`, request).pipe(
      tap(() => this.refreshSubject.next())
    );
  }

  deleteMedicine(id: string): Observable<void> {
    return this.http.delete<void>(`${this.medicinesUrl}/${id}`).pipe(
      tap(() => this.refreshSubject.next())
    );
  }

  recordSale(request: CreateSaleRequest): Observable<SaleResponse> {
    return this.http.post<SaleResponse>(this.salesUrl, request).pipe(
      tap(() => this.refreshSubject.next())
    );
  }
}
