
import React from 'react';
import ReactDOM from 'react-dom';
import "./UserGroup.css";
import UserTab from './UserTab.jsx';
import GroupTab from './GroupTab.jsx';

var UserGroup = React.createClass({
    componentDidMount: () => $("#tabstrip").kendoTabStrip().data("kendoTabStrip").activateTab($("#userTab")),
    render: function() {
        return (
            <div id="tabstrip">
                <ul>
                    <li id="userTab">Users</li>
                    <li>Groups</li>
                </ul>
                <div><UserTab/></div>
                <div><GroupTab/></div>
            </div>
        );
    }
});

export default UserGroup;