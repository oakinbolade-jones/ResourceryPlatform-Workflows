import { Environment } from '@abp/ng.core';

const baseUrl = 'http://smartserve.ecowas.int';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ResourceryPlatformWorkflow',
    logoUrl: '/assets/logo.png',
  },
  oAuthConfig: {
    issuer: 'http://auth.smartserve.ecowas.int/',
    redirectUri: baseUrl,
    clientId: 'ResourceryPlatformWorkflow_Web',
    responseType: 'code',
    scope: 'offline_access profile email phone roles ResourceryPlatformWorkflowWorkflow ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: false,
  },
  apis: {
    default: {
      url: 'http://api.smartserve.ecowas.int',
      rootNamespace: 'ResourceryPlatformWorkflow',
    },
  },
  localization: {
    defaultResourceName: 'Workflow',
  },
} as Environment;
