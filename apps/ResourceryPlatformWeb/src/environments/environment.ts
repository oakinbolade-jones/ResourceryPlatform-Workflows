import { Environment } from '@abp/ng.core';

const baseUrl = 'http://smartserve.ecowas.int:4200';

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'ResourceryPlatformWorkflow',
    logoUrl: 'https://smartserve.ecowas.int:7600/Account/Login',
  },
  oAuthConfig: {
    issuer: 'https://smartserve.ecowas.int:7600/',
    redirectUri: baseUrl,
    clientId: 'ResourceryPlatformWorkflow_Web',
    responseType: 'code',
    scope: 'offline_access profile email phone roles ResourceryPlatformWorkflowWorkflow ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: false,
  },
  apis: {
    default: {
      url: 'https://smartserve.ecowas.int:7500',
      rootNamespace: 'ResourceryPlatformWorkflow',
    },
  },
  localization: {
    defaultResourceName: 'Workflow',
  },
} as Environment;
