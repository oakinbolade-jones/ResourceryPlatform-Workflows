import { Environment } from '@abp/ng.core';

<<<<<<< HEAD
const baseUrl = 'https://smartserve.ecowas.int';
=======
const baseUrl = 'http://smartserve.ecowas.int';
>>>>>>> staging

export const environment = {
  production: true,
  application: {
    baseUrl,
<<<<<<< HEAD
    name: 'ResourceryPlatformWorkflow',
    logoUrl: 'https://auth.smartserve.ecowas.int/Account/Login',
  },
  oAuthConfig: {
    issuer: 'https://auth.smartserve.ecowas.int',
=======
    name: 'SmartServe Platform',
    logoUrl: 'https://auth.smartserve.ecowas.int//Account/Login',
  },
  oAuthConfig: {
    issuer: 'https://auth.smartserve.ecowas.int/',
>>>>>>> staging
    redirectUri: baseUrl,
    clientId: 'ResourceryPlatformWorkflow_Web',
    responseType: 'code',
    scope: 'offline_access profile email phone roles ResourceryPlatformWorkflowWorkflow ResourceryPlatformWorkflowIdentityService ResourceryPlatformWorkflowAdministration ResourceryPlatformWorkflowSaaS',
    requireHttps: true,
  },
  apis: {
    default: {
<<<<<<< HEAD
      url: 'https://api.smartserve.ecowas.int',
=======
      url: 'http://api.smartserve.ecowas.int',
>>>>>>> staging
      rootNamespace: 'ResourceryPlatformWorkflow',
    },
  },
  localization: {
    defaultResourceName: 'Administration',
  },
} as Environment;
