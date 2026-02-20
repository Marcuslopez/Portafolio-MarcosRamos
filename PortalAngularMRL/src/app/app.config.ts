//import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
//import { provideRouter } from '@angular/router';
//import { provideClientHydration, withEventReplay } from '@angular/platform-browser';

import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { routes } from './app.routes';
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http';
import { jwtInterceptor } from './interceptors/jwt.interceptor';


/*
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes), provideClientHydration(withEventReplay())
  ]
};
*/

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor]), withFetch())
  ]
};
