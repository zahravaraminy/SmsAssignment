import { Component } from '@angular/core';
import { Chart, registerables } from 'chart.js';
Chart.register(...registerables);

let accountChartInstance: Chart | null = null;
let numberChartInstance: Chart | null = null;

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})


export class DashboardComponent {

  async fetchData() {
    console.log("Fetching data...");
    const phoneNumber = (document.getElementById('phoneNumber') as HTMLInputElement).value;
    const startDateInput = document.getElementById('startDate')  as HTMLInputElement | null;
    const endDateInput = document.getElementById('endDate')  as HTMLInputElement | null;
 
     // Validate inputs
     if (!startDateInput || !endDateInput) {
      console.error('Input elements not found');
      return;
    }

    const startDate = startDateInput.value ? `${startDateInput.value}T00:00:00Z` : null;
    const endDate = endDateInput.value ? `${endDateInput.value}T23:59:59Z` : null;

    if (!startDate || !endDate) {
      console.error('Start Date and End Date are required');
      return;
    }
     // Fetch account data
     const accountResponse = await fetch(
      `http://localhost:5115/monitor/account?startDate=${startDate}&endDate=${endDate}`
    );
    const numberResponse = await fetch(
      `http://localhost:5115/monitor/number?phoneNumber=${phoneNumber}&startDate=${startDate}&endDate=${endDate}`
      );
     

      if (accountResponse.ok && accountResponse.headers.get('Content-Type')?.includes('application/json')) {
        const accountData = await accountResponse.json(); 
        this.updateAccountChart(accountData);
    } else {
      const error = await accountResponse.text();
        
      console.error("Error fetching data:", error);
    }
       
    if (numberResponse.ok && numberResponse.headers.get('Content-Type')?.includes('application/json')) {
      const numberData = await numberResponse.json();
      this.updateNumberChart(numberData);
    console.log(numberData);
  } 
     
  } 


updateAccountChart(data: AccountData[]): void {
    const canvas = document.getElementById('accountChart') as HTMLCanvasElement | null;

    if (!canvas) {
        console.error("Canvas element with id 'accountChart' not found.");
        return;
    }

    const ctx = canvas.getContext('2d');

    if (!ctx) {
        console.error("Unable to get 2D context for the canvas.");
        return;
    }

    if (accountChartInstance) {
      accountChartInstance.destroy();
  }
    const labels = data.map((item) => item.date);
    const messagesSent = data.map((item) => item.messagesSent);

    accountChartInstance = new Chart(ctx, {
      type: 'line',
      data: {
          labels: labels,
          datasets: [
              {
                  label: 'Account Messages Sent',
                  data: messagesSent,
                  borderColor: 'rgba(75, 192, 192, 1)',
                  tension: 0.1,
              },
          ],
      },
  });
};


updateNumberChart (data: ChartData[]): void {
  const canvas = document.getElementById('numberChart') as HTMLCanvasElement;
  if (!canvas) {
      console.error("Canvas element with id 'numberChart' not found.");
      return;
  }

  const ctx = canvas.getContext('2d');
  if (!ctx) {
      console.error("Failed to get 2D context from canvas element.");
      return;
  }
  if (numberChartInstance) {
    numberChartInstance.destroy();
}
  const labels = data.map((item) => item.date);
  const messagesSent = data.map((item) => item.messagesSent);

  numberChartInstance = new Chart(ctx, {
    type: 'line',
    data: {
        labels: labels,
        datasets: [
            {
                label: 'Number Messages Sent',
                data: messagesSent,
                borderColor: 'rgba(255, 99, 132, 1)',
                tension: 0.1,
            },
        ],
    },
});
};

 }

interface AccountData {
  date: string;
  messagesSent: number;
}
interface ChartData {
  date: string; // Adjust the type based on your actual data structure
  messagesSent: number; // Adjust the type based on your actual data structure
}