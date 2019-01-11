import * as React from 'react';
import { Provider } from 'react-redux';
import ReactDOM from 'react-dom';
import { renderToString } from 'react-dom/server';
import {BrowserRouter as Router, Route} from 'react-router-dom';

import Schedule from "./Schedule/Index.jsx";
import UserGroup from "./UserGroup/Index.jsx";
import Resource from "./Resource/Index.jsx";
import Project from "./Project/Index.jsx";
import Developer from "./Developer/Index.jsx";
import Header from "./Header.jsx";

const MainView = React.createClass({    
    render: function () {
        return (
            <Router>
                <div>
                    <Header />                    
                    <div className="container" style={{marginTop: 33 + "px"}}>
                        <Route exact path="/" render={() => (
                            <div>
                                <h1>Project</h1>
                                <Project />
                            </div>
                         )} />
                        <Route path="/schedule" render={() => (
                            <div>
                                <h1>Schedule</h1>
                                <Schedule />
                            </div>
                         )} />
                        <Route path="/usergroup" render={() => (
                            <div>
                                <h1>User and Group</h1>
                                <UserGroup />
                            </div>
                         )} />
                        <Route path="/resource" render={() => (
                            <div>
                                <h1>Resource</h1>
                                <Resource />
                            </div>
                         )} />
                        <Route path="/project" render={() => (
                            <div>
                                <h1>Project</h1>
                                <Project />
                            </div>
                         )} />
                        <Route path="/developer" render={() => (
                            <div>
                                <h1>Developer</h1>
                                <Developer />
                            </div>
                         )} />

                        <hr />
                        <footer>
                            <p>&copy; 2017 - RMAUI</p>
                        </footer>
                    </div>                    
                </div>
            </Router>            
        );
    }
});
export default MainView;

// Enable Hot Module Replacement (HMR)
// if (module.hot) {
//     module.hot.accept();
// }

 ReactDOM.render(
    <MainView />, document.getElementById("hereSPA"));
