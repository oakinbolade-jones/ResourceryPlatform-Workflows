import { RoutesService, eLayoutType } from '@abp/ng.core';
import { APP_INITIALIZER } from '@angular/core';

export const APP_ROUTE_PROVIDER = [
  { provide: APP_INITIALIZER, useFactory: configureRoutes, deps: [RoutesService], multi: true },
];

function configureRoutes(routesService: RoutesService) {
  return () => {
    routesService.add([
      {
        path: '/',
        name: 'Administration::Home',
        iconClass: '',
        order: 0,
        layout: eLayoutType.application,
      },
      // {
      //   path: '/get-started',
      //   name: 'Administration::GetStarted',
      //   iconClass: '',
      //   order: 1,
      //   layout: eLayoutType.application,
      // },
      // {
      //   path: '/webcast',
      //   name: 'Administration::Webcasts',
      //   iconClass: '',
      //   order: 2,
      //   layout: eLayoutType.application,
      // },
      {
        path: '/dashboard',
        name: 'Administration::ServiceCenters',
        iconClass: '',
        order: 3,
        layout: eLayoutType.application,
      }, {
        path: '/documentation',
        name: 'Administration::Documentation',
        iconClass: '',
        order: 4,
        layout: eLayoutType.application,
      },
      {
        path: '/transcribe',
        name: 'Administration::SpeechToText',
        iconClass: '',
        order: 5,
        layout: eLayoutType.application,
      },

      {
        path: '/directorate',
        name: 'Administration::Directorate',
        parentName: 'Administration::ServiceCenters',
        iconClass: '',
        order: 0,
        layout: eLayoutType.application,
      },

      {
        path: '/translation',
        name: 'Administration::Translation',
        parentName: 'Administration::ServiceCenters',
        iconClass: '',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/interpretation',
        name: 'Administration::Interpretation',
        parentName: 'Administration::ServiceCenters',
        iconClass: '',
        order: 2,
        layout: eLayoutType.application,
      },
      {
        path: '/conference',
        name: 'Administration::Conference',
        parentName: 'Administration::ServiceCenters',
        iconClass: '',
        order: 3,
        layout: eLayoutType.application,
      },
      {
        path: '/protocol',
        name: 'Administration::Protocol',
        parentName: 'Administration::ServiceCenters',
        iconClass: '',
        order: 4,
        layout: eLayoutType.application,
      },
      // {
      //   path: '/transcription',
      //   name: 'Administration::Transcription',
      //   parentName: 'Administration::ServiceCenters',
      //   iconClass: '',
      //   order: 5,
      //   layout: eLayoutType.application,
      // },


    ]);
  };
}
