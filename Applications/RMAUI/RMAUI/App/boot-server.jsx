import * as React from 'react';
import { renderToString } from 'react-dom/server';
import { createServerRenderer, RenderResult } from 'aspnet-prerendering';
import App from "./App.jsx";

export default createServerRenderer(params => {
    return new Promise((resolve, reject) => {
        const app = (
            <App username={params.data.UserName}/>
        );
        console.log(params);
        resolve({
            html: renderToString(app),            
            globals: {  }
        });
    });
});