import React from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import App from './App';
import * as serviceWorker from './serviceWorker';
const authentication = require('@kdpw/msal-b2c-react');
//import authentication from '@kdpw/msal-b2c-react';

authentication.initialize({
    instance: 'https://login.microsoftonline.com/tfp/',
    tenant: 'HueppauffB2C.onmicrosoft.com',
    signInPolicy: 'B2C_1_b2s_1_susi',
    applicationId: '739f43a1-d8fe-401e-97bc-3f07525e1292',
    cacheLocation: 'sessionStorage',
    scopes: ['user_impersonation'],
    redirectUri: 'http://localhost:3000',
    postLogoutRedirectUri: window.location.origin,
});


authentication.run(() => {
    ReactDOM.render(<App />, document.getElementById('root'));
    serviceWorker.unregister();
});