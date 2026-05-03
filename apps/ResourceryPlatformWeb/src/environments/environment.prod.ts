import { Environment } from '@abp/ng.core';

<<<<<<< HEAD
const baseUrl = 'http://localhost:4200';
=======
const baseUrl = 'https://localhost:4200';
>>>>>>> refs/heads/development

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ResourceryPlatformWorkflow',
    logoUrl: '',
  },
  oAuthConfig: {
    issuer: 'https://localhost:7600/',
    redirectUri: baseUrl,
    clientId: 'ResourceryPlatformWorkflow_Web',
    clientSecret: '1q2w3e*',
    responseType: 'code',
<<<<<<< HEAD
    scope: 'offline_access ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: true,
=======
    scope: 'offline_access profile email phone roles ResourceryPlatformWorkflowWorkflow ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: false,
>>>>>>> refs/heads/development
  },
  apis: {
    default: {
      url: 'https://localhost:7500',
      rootNamespace: 'ResourceryPlatformWorkflow',
    },
  },
  localization: {
<<<<<<< HEAD
    defaultResourceName: 'AbpUi',
=======
    defaultResourceName: 'Workflow',
>>>>>>> refs/heads/development
  },
} as Environment;
