import * as React from 'react';
import { Provider } from 'react-redux';
import ReactDOM from 'react-dom';
import { renderToString } from 'react-dom/server';
import {BrowserRouter as Router, Route, Link, Switch, match } from 'react-router-dom';

import Schedule from "./Schedule/Index.jsx";
import UserGroup from "./UserGroup/Index.jsx";
import Resource from "./Resource/Index.jsx";
import Project from "./Project/Index.jsx";
import Developer from "./Developer/Index.jsx";

export default <Router>
                <div>
                    <nav className="navbar navbar-inverse navbar-fixed-top">
                        <div className="container">
                            <div className="navbar-header">
                                <button type="button" className="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                                    <span className="sr-only">Toggle navigation</span>
                                    <span className="icon-bar"></span>
                                    <span className="icon-bar"></span>
                                    <span className="icon-bar"></span>
                                </button>
                                <a className="navbar-brand"><img src="images/MenuI11.png" /></a>

                            </div>
                            <div className="navbar-collapse collapse">
                                <ul className="nav navbar-nav arrows">
                                    <li><Link to={{ pathname: '/project' }}>Project</Link></li>
                                    <li><Link to={{ pathname: '/schedule' }}>Schedule</Link></li>
                                    <li><Link to={{ pathname: '/resource' }}>Resource Schedule</Link></li>
                                    <li><Link to={{ pathname: '/developer' }}>Developer</Link></li>
                                    <li><Link to={{ pathname: '/usergroup' }}>User and Group</Link></li>                                    
                                </ul>
                                <p className="nav navbar-text navbar-right">Hello, {window.currentLoggedUser}!</p>
                            </div>
                        </div>
                    </nav>                    
                    
                    <div className="container" style={{marginTop: 110+"px"}}>
                        <Route exact path="/" component={Project}/>
                        <Route path="/schedule" title="Dao Nguyen" component={Schedule} />
                        <Route path="/usergroup" component={UserGroup} />
                        <Route path="/resource" component={Resource} />
                        <Route path="/project" component={Project} />
                        <Route path="/developer" component={Developer} />

                        <hr />
                        <footer>
                            <p>&copy; 2017 - RMAUI</p>
                        </footer>
                    </div>
                    
                </div>
            </Router>            
;

// Enable Hot Module Replacement (HMR)
// if (module.hot) {
//     module.hot.accept();
// }
