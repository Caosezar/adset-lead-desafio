import { Component, OnInit } from '@angular/core';
import { ApiConfigService } from './services/api-config.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'AdsetManagementWeb';

  constructor(private apiConfigService: ApiConfigService) {}

  ngOnInit(): void {

  }
}
