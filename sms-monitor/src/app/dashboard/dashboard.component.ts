import { Component } from '@angular/core';

@Component({
  selector: 'app-dashboard',
   standalone: true,
   //template: '<p>Dashboard works!</p>'
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {

  async fetchData() {
    console.log("Fetching data...");
    const startDate = (document.getElementById('startDate') as HTMLInputElement).value;
    const endDate = (document.getElementById('endDate') as HTMLInputElement).value;
    const phoneNumber = (document.getElementById('phoneNumber') as HTMLInputElement).value;

    
      //const accountResponse = await fetch(`/monitor/account?startDate=${startDate}&endDate=${endDate}`);
      const accountResponse = await fetch(`http://localhost:5115/monitor/account?startDate=${startDate}&endDate=${endDate}`);



      //const numberResponse = await fetch(`/monitor/number?phoneNumber=${phoneNumber}&startDate=${startDate}&endDate=${endDate}`);
      const numberResponse = await fetch(`http://localhost:5115/monitor/number?phoneNumber=${phoneNumber}&startDate=${startDate}&endDate=${endDate}`);

      if (accountResponse.ok && accountResponse.headers.get('Content-Type')?.includes('application/json')) {
        const accountData = await accountResponse.json(); 
      console.log(accountData);
    } else {
      const error = await accountResponse.text();
        
      console.error("Error1 fetching data:", error);
    }
    if (numberResponse.ok && numberResponse.headers.get('Content-Type')?.includes('application/json')) {
      const numberData = await numberResponse.json();
    console.log(numberData);
  } else {
    const error = await numberResponse.text();
      
    console.error("Error2 fetching data:", error);
  }}
 
 }
