import { Component, CUSTOM_ELEMENTS_SCHEMA  } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DashboardComponent } from './dashboard/dashboard.component';;


@Component({
  selector: 'app-root',
  standalone: true, 
  templateUrl: './app.component.html',
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  imports: [DashboardComponent], 
  styleUrls: ['./app.component.css']
  //imports: [RouterOutlet],
 // templateUrl: './app.component.html',
 // styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'sms-monitor';
}
