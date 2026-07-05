export interface Medicine {
  id: string;
  fullName: string;
  brand: string;
  notes?: string;
  expiryDate: string;
  quantity: number;
  price: number;
  isExpiringWithin30Days: boolean;
  isLowStock: boolean;
  highlightColor: string;
  createdAtUtc: string;
  updatedAtUtc?: string;
}

export interface CreateMedicineRequest {
  fullName: string;
  brand: string;
  notes?: string;
  expiryDate: string;
  quantity: number;
  price: number;
}

export interface UpdateMedicineRequest extends CreateMedicineRequest {}

export interface CreateSaleRequest {
  medicineId: string;
  quantity: number;
}

export interface SaleResponse {
  id: string;
  medicineId: string;
  medicineName: string;
  quantity: number;
  unitPrice: number;
  totalAmount: number;
  soldAtUtc: string;
}
