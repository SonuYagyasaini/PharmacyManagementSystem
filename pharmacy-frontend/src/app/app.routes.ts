import { Routes } from '@angular/router';
import { MedicineListComponent } from './components/medicine-list/medicine-list.component';
import { MedicineFormComponent } from './components/medicine-form/medicine-form.component';

export const routes: Routes = [
  { path: '', component: MedicineListComponent },
  { path: 'medicine/add', component: MedicineFormComponent, data: { mode: 'create' } },
  { path: 'medicine/edit/:id', component: MedicineFormComponent, data: { mode: 'edit' } },
  { path: 'medicine/view/:id', component: MedicineFormComponent, data: { mode: 'view' } },
  { path: '**', redirectTo: '' },
];
