import { Component } from '@angular/core';
import { Chart, registerables } from 'chart.js';
import { HttpErrorResponse, provideHttpClient } from '@angular/common/http';
import { HttpClient } from '@angular/common/http'; // Import HttpClient
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // Import FormsModule
Chart.register(...registerables);

let accountChartInstance: Chart | null = null;
let numberChartInstance: Chart | null = null;

@Component({
  selector: 'app-dashboard',
  standalone: true,
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css'],
  imports: [CommonModule, FormsModule],
})


export class DashboardComponent {
  constructor(private http: HttpClient) {} // Inject HttpClient here
  smsList: { accountId: string; phoneNumber: string }[] = [
    { accountId: '', phoneNumber: '' }
  ]; // Initial row for SMS details

  async fetchData(): Promise<void> {
    try {
      console.log("Fetching data...");

      const phoneNumber = (document.getElementById('phoneNumber') as HTMLInputElement)?.value || '';
      const startDateInput = document.getElementById('startDate') as HTMLInputElement | null;
      const endDateInput = document.getElementById('endDate') as HTMLInputElement | null;

      // Validate input elements
      if (!startDateInput || !endDateInput) {
        alert('Start date and end date input elements are required.');
      return;
      }

      const startDate = startDateInput.value ? `${startDateInput.value}T00:00:00Z` : null;
      const endDate = endDateInput.value ? `${endDateInput.value}T23:59:59Z` : null;

      if (!startDate || !endDate) {
        alert('Start date and end date input elements are required.');
      return;
      }

      // Fetch data concurrently
      const [accountResponse, numberResponse] = await Promise.all([
        fetch(`http://localhost:5115/monitor/account?startDate=${startDate}&endDate=${endDate}`),
        fetch(`http://localhost:5115/monitor/number?phoneNumber=${phoneNumber}&startDate=${startDate}&endDate=${endDate}`)
      ]);

      if (accountResponse.ok && accountResponse.headers.get('Content-Type')?.includes('application/json')) {
        const accountData = await accountResponse.json() as AccountData[];
        this.updateAccountChart(accountData);
      } else {
        console.error("Error fetching account data:", await accountResponse.text());
      }

      if (numberResponse.ok && numberResponse.headers.get('Content-Type')?.includes('application/json')) {
        const numberData = await numberResponse.json() as ChartData[];
        this.updateNumberChart(numberData);
      } else {
        console.error("Error fetching number data:", await numberResponse.text());
      }
    } catch (error) {
      console.error("An error occurred:", error);
    }
  } 

  addSmsRow(): void {
    this.smsList.push({ accountId: '', phoneNumber: '' });
  }

  removeSmsRow(index: number): void {
    if (this.smsList.length > 1) {
      this.smsList.splice(index, 1);
    }
  }
  sendSms(): void {
    // Get the values from the input fields
    const accountIdInput = document.getElementById('accountId') as HTMLInputElement;
    const phoneInput = document.getElementById('phone') as HTMLInputElement;
  
    const accountId = accountIdInput?.value.trim() || '';
    const phoneNumber = phoneInput?.value.trim() || '';
  
    // Validate inputs
    for (const sms of this.smsList) {
      if (!sms.accountId || !sms.phoneNumber) {
        alert('All Account IDs and Phone Numbers must be filled.');
        return;
      }
    }
  
     // Send requests for each pair
     const requests = this.smsList.map((sms) =>
      this.http
        .post('http://localhost:5115/api/sms/can-send', sms, { responseType: 'text' })
        .toPromise()
    );

  
      // Wait for all requests to complete
      Promise.allSettled(requests)
  .then((results) => {
    results.forEach((result, index) => {
      if (result.status === 'fulfilled') {
        alert(`for ${this.smsList[index].phoneNumber}: ${result.value}`);
      } else {
        // Check if the result.reason is an HttpErrorResponse
        const errorResponse = result.reason as HttpErrorResponse;
        const errorMessage =
          errorResponse.error?.message || errorResponse.message || "Unknown error";

        alert(`for ${this.smsList[index].phoneNumber}: ${errorMessage}`);
      }
    });
  });

  
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
   // Sort data by date (ascending)
   const sortedData = data.sort((a, b) => new Date(a.date).getTime() - new Date(b.date).getTime());
  // Combine date and accountId for labels
   const labels = data.map((item) => `${item.accountId} - ${item.date}`);
    
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
  accountId: string; // Unique identifier for the account
  date: string;
  messagesSent: number;
}
interface ChartData {
  date: string; // Adjust the type based on your actual data structure
  messagesSent: number; // Adjust the type based on your actual data structure
}