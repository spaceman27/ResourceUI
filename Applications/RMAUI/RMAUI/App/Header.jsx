import React from 'react';
import {Link} from 'react-router-dom';

const Header = React.createClass({
    onSideBarClick: function(e){
        
    },
    render: function(){
        return (<div>
                    <div id="sidebar-wrapper">
                        <ul className="sidebar-nav">
                            <li className="sidebar-brand">
                                <a href="#">
                                    More Features
                                </a>
                            </li>
                        </ul>
                    </div>
                    <nav className="navbar navbar-inverse navbar-fixed-top">
                        <div className="container">
                            <div className="navbar-header">
                                <button type="button" className="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                                    <span className="sr-only">Toggle navigation</span>
                                    <span className="icon-bar"></span>
                                    <span className="icon-bar"></span>
                                    <span className="icon-bar"></span>
                                </button>
                                <a className="navbar-brand"><img src="images/MenuI11.png" onClick={this.onSideBarClick} /></a>

                            </div>
                            <div className="navbar-collapse collapse">
                                <ul className="nav navbar-nav arrows">
                                    {window.schedulePermission.Read || window.schedulePermission.Write?<li><Link to={{ pathname: '/project' }}>Project</Link></li>:""}
                                    {window.projectSchedulePermission.Read || window.projectSchedulePermission.Write?<li><Link to={{ pathname: '/schedule' }}>Schedule</Link></li>:""}
                                    {window.assignmentPermission.Read || window.assignmentPermission.Write?<li><Link to={{ pathname: '/resource' }}>Resource Schedule</Link></li>:""}
                                    {window.developerPermission.Read || window.developerPermission.Write?<li><Link to={{ pathname: '/developer' }}>Developer</Link></li>:""}
                                    {window.UserAndGroupPermission.Read || window.UserAndGroupPermission.Write?<li><Link to={{ pathname: '/usergroup' }}>User and Group</Link></li>:""}
                                </ul>;
                                <p className="nav navbar-text navbar-right">Hello, {window.currentLoggedUser}!</p>
                            </div>
                        </div>
                    </nav>
                </div>);
    }
});

export default Header;