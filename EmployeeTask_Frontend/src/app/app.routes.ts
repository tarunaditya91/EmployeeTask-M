import { Routes } from '@angular/router';
import { EmployeeList } from './components/employee-list/employee-list';
import { TaskList } from './components/task-list/task-list';
import { Login } from './components/login/login';
import { Register } from './components/register/register';

export const routes: Routes = [
  { path: 'employees', component: EmployeeList },
  { path: 'tasks', component: TaskList },
  { path: 'auth/login', component: Login },
  { path: 'auth/register', component: Register },
  { path: '', redirectTo: 'auth/login', pathMatch: 'full' },
  { path: '**', redirectTo: 'auth/login' } // Wildcard for invalid routes
];