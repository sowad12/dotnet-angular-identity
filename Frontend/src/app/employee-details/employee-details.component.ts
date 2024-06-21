import { Component, OnInit } from '@angular/core';
import { EmployeeService } from '../shared/employee.service';
import { ToastrService } from 'ngx-toastr';
import { Employee } from '../shared/employee.model';

@Component({
  selector: 'app-employee-details',
  templateUrl: './employee-details.component.html',
  styleUrls: ['./employee-details.component.css'],
})
export class EmployeeDetailsComponent implements OnInit {
  constructor(
    private empService: EmployeeService,
    public toast: ToastrService
  ) {}

  EmployeeDataList: Array<Employee> = [];

  ngOnInit() {
    this.empService.GetEmployees().subscribe((x) => {
      this.EmployeeDataList = x;
    });
  }
  tableHeadingList: Array<string> = [
    'Name',
    'Last Name',
    'Email',
    'Date of join',
    'Designation',
    'Age',
    'Actions',
  ];
  
  updateEmployee(data:Employee){
    this.empService.UpdateEmployee(data);
  }
  removeEmployee(id:number){
    console.log("id",id);
  }

}
