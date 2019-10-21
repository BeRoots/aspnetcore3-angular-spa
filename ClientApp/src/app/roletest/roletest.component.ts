import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
    selector: 'app-roletest',
    templateUrl: './roletest.component.html'
})
export class RoletestComponent {
    public dates: MyDates[];

    constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
        http.get<MyDates[]>(baseUrl + 'role').subscribe(result => {
            this.dates = result;
        }, error => console.error(error));
    }
}

interface MyDates {
    date: string;
}
