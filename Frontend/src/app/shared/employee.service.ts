import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Designation, Employee } from './employee.model';
import { Observable } from 'rxjs';
@Injectable({
  providedIn: 'root',
})
export class EmployeeService {
  constructor(private _Httpclient: HttpClient) {}

  EmployeeBaseUrl: string = 'https://localhost:7100/api/v1/Employee';
  DesignationBaseUrl: string = 'https://localhost:7100/api/v1/Designation';
  EmployeeData: Employee = new Employee();
  EmployeeList: Array<Employee> = [];
  DesinationData: Designation;

  AddEmployee() {
    return this._Httpclient.post(
      `${this.EmployeeBaseUrl}/add-employee`,
      this.EmployeeData
    );
  }
  UpdateEmployee(data:Employee) {
    return this._Httpclient.put(
      `${this.EmployeeBaseUrl}/update-employee`,
       data
    );
  }
  GetEmployees(): Observable<Array<Employee>> {
    return this._Httpclient.get<Array<Employee>>(
      `${this.EmployeeBaseUrl}/get-employees`
    );
  }

  deleteEmployee(id: number) {
    return this._Httpclient.delete(`${this.EmployeeBaseUrl}/${id}`);
  }

  GetDesignations(): Observable<Array<Designation>> {
    return this._Httpclient.get<Array<Designation>>(
      `${this.DesignationBaseUrl}/get-designations`
    );
  }
}
