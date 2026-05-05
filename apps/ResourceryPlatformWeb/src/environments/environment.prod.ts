import { Environment } from '@abp/ng.core';

const baseUrl = 'https://localhost:4200';

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
    scope: 'offline_access profile email phone roles ResourceryPlatformWorkflowWorkflow ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: false,
  },
  apis: {
    default: {
      url: 'https://localhost:7500',
      rootNamespace: 'ResourceryPlatformWorkflow',
    },
  },
  localization: {
    defaultResourceName: 'Workflow',
  },
} as Environment;
