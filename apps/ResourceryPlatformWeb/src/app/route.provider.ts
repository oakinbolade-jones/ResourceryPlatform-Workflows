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
        name: 'Workflow::Home',
        iconClass: '',
        order: 0,
        layout: eLayoutType.application,
      },
      {
        path: '/get-started',
        name: 'Workflow::GetStarted',
        iconClass: '',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/webcast',
        name: 'Workflow::Webcasts',
        iconClass: '',
        order: 2,
        layout: eLayoutType.application,
      },
      {
        path: '/dashboard',
        name: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 3,
        layout: eLayoutType.application,
      }, {
        path: '/documentation',
        name: 'Workflow::Documentation',
        iconClass: '',
        order: 4,
        layout: eLayoutType.application,
      },
      {
        path: '/support',
        name: 'Workflow::Support',
        iconClass: '',
        order: 5,
        layout: eLayoutType.application,
      },

      {
        path: '/directorate',
        name: 'Workflow::Directorate',
        parentName: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 0,
        layout: eLayoutType.application,
      },

      {
        path: '/translation',
        name: 'Workflow::Translation',
        parentName: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 1,
        layout: eLayoutType.application,
      },
      {
        path: '/interpretation',
        name: 'Workflow::Interpretation',
        parentName: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 2,
        layout: eLayoutType.application,
      },
      {
        path: '/conference',
        name: 'Workflow::Conference',
        parentName: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 3,
        layout: eLayoutType.application,
      },
      {
        path: '/protocol',
        name: 'Workflow::Protocol',
        parentName: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 4,
        layout: eLayoutType.application,
      },
      {
        path: '/transcription',
        name: 'Workflow::Transcription',
        parentName: 'Workflow::ServiceCenters',
        iconClass: '',
        order: 5,
        layout: eLayoutType.application,
      },


    ]);
  };
}
