import { AuthConfig } from 'angular-oauth2-oidc';

export const authCodeFlowConfig: AuthConfig = {
  clientId: 'tavern-trashers-public-client',
  redirectUri: window.location.origin,
  postLogoutRedirectUri: window.location.origin,

  scope: 'openid profile email offline_access',

  issuer: 'http://localhost:3000/realms/tavern-trashers',

  tokenEndpoint:
    'http://localhost:3000/realms/tavern-trashers/protocol/openid-connect/token',

  responseType: 'code',

  showDebugInformation: true,
};
