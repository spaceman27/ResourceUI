
import React from 'react';
import SearchControl from './SearchControl.jsx';
import SelectListRender from './SelectListRender.jsx';
import PermissionPanel from './PermissionPanel.jsx';
import Tooltip from '../Common/Tooltip.jsx';

var UserTab = React.createClass({
    getInitialState: function () {
        return {
            permissionDetail: null,
            userList: null,
            searchValue: null
        }
    },
    componentDidMount: function () {
        var self = this;
        $.ajax({ url: "/UserGroup/GetUserList" }).done((response) => self.setState({ userList: response }));
    },
    onSelect: function (e) {
        var username = e.target.value;
        var user = this.state.userList.filter((value) => value.Username === username);
        this.setState({ permissionDetail: user });
    },
    UpdateUserList: function (user) {
        var userLength = this.state.userList.length;
        var updateList = this.state.userList;
        for (let i = 0; i < userLength; i++) {
            if (updateList[i].Username === user.Username) {
                updateList[i] = user;
                break;
            }
        }        
        this.setState({ userList: updateList });
    },
    onCheckboxClick: function (value, obj, e) {
        var self = this;
        
        var selected = e.target.checked;
        var v = value.modulePermission;
        if (obj === "r") {
            v.UserRight.Read = selected;
        } else if (obj === "w") {
            v.UserRight.Write = selected;
        }
        let input = {
                userName: value.model.Username,
                moduleName: v.Name,
                userRight: v.UserRight
            };
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: "/UserGroup/UpdateUserPermission",
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(input),
            success: function (response) {
                self.showTooltip.bind(self)("Update success");
                self.setState({ permissionDetail: response });
                self.UpdateUserList(response[0]);
            },
            error: function () {
                // will show a tool tip here
                self.showTooltip.bind(self)("Update Failed");
            }
        });
    },
    updateUserList: function (textChange) {
        this.setState({ searchValue: textChange });
    },
    showTooltip: function (message) {
        this.setState({
            openTooltip: true,
            content: message
        });
        this.setState({
            openTooltip: false
        });
    },
    render: function () {
        return (
            <div id="users_content">
                <div className="col-md-12">
                    <div className="col-md-4" style={{"marginBottom": 5 + "px"}}>
                        <SearchControl onChange={this.updateUserList} />
                    </div>
                    <div className="col-md-6" style={{"textAlign": "center"}}>
                        <Tooltip idx="userTooltip" content={this.state.content} isShow={this.state.openTooltip} />
                    </div>
                </div>
                <div className="col-md-12">
                    <div className="col-md-4">
                        <SelectListRender list={this.state.userList} filterBy={this.state.searchValue} onSelect={this.onSelect} />
                    </div>
                    <div className="col-md-6">
                        <PermissionPanel idx="userGrid" list={this.state.permissionDetail} onCheckboxClick={this.onCheckboxClick} />
                    </div>
                </div>
            </div>
        );
    }
});

export default  UserTab;