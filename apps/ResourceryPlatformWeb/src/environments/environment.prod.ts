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
    name: 'SmartServe Platform',
    logoUrl: '',
  },
  oAuthConfig: {
<<<<<<< HEAD
    issuer: 'https://auth.smartserve.ecowas.int',
=======
    issuer: 'https://auth.smartserve.ecowas.int/',
>>>>>>> staging
    redirectUri: baseUrl,
    clientId: 'ResourceryPlatformWorkflow_Web',
    // clientSecret: '1q2w3e*',
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

