import { Environment } from '@abp/ng.core';

const baseUrl = 'https://smartserve.ecowas.int';

export const environment = {
  production: true,
  application: {
    baseUrl,
    name: 'ResourceryPlatformWorkflow',
    logoUrl: 'https://auth.smartserve.ecowas.int/Account/Login',
  },
  oAuthConfig: {
    issuer: 'https://auth.smartserve.ecowas.int/',
    redirectUri: baseUrl,
    clientId: 'ResourceryPlatformWorkflow_Web',
    responseType: 'code',
    scope: 'offline_access profile email phone roles ResourceryPlatformWorkflowWorkflow ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: true,
  },
  apis: {
    default: {
      url: 'https://api.smartserve.ecowas.int',
      rootNamespace: 'ResourceryPlatformWorkflow',
    },
  },
  localization: {
    defaultResourceName: 'Workflow',
  },
} as Environment;
