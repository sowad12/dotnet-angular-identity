export class Employee {
  id: number = 0;
  name: string;
  lastName: string = '';
  email: string = '';
  age: number;
  dob: any;
  gender: string = 'male';
  isMarried: number;
  isActive: number;
  designationID: number = 0;
  designation: Designation;
}
export class Designation {
  id: number = 0;
  designationTitle: string = '';
}
