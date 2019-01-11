import React from 'react';
import SearchControl from './SearchControl.jsx';
import SelectListRender from './SelectListRender.jsx';
import PermissionPanel from './PermissionPanel.jsx';
import Tooltip from '../Common/Tooltip.jsx';

var GroupTab = React.createClass({
    getInitialState: function() {
        return {
            permissionDetail: null,
            groupList: null,
            searchValue: null
        }
    },
    componentDidMount: function() {
        var self = this;
        $.ajax({url: "/UserGroup/GetGroupList"}).done((response) => self.setState({ groupList: response }));
    },
    onSelect: function(e) {
        var name = e.target.value;
        var group = this.state.groupList.filter((value) => value.Name === name);
        this.setState({ permissionDetail: group });
    },
    UpdateGroupList: function (group) {
        var groupLength = this.state.groupList.length;
        var updateList = this.state.groupList;
        for (let i = 0; i < groupLength; i++) {
            if (updateList[i].Name === group.Name) {
                updateList[i] = group;
                break;
            }
        }

        this.setState({ groupList: updateList });
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
                groupName: value.model.Name,
                moduleName: v.Name,
                userRight: v.UserRight
            };
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: "/UserGroup/UpdateGroupPermission",
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(input),
            success: function (response) {
                self.showTooltip.bind(self)("Update success");
                self.setState({ permissionDetail: response });
                self.UpdateGroupList(response[0]);
            },
            error: function () {
                // will show a tool tip here
                self.showTooltip.bind(self)("Update Failed");
            }
        });
    },
    updateGroupList: function(textChange) {
        this.setState({ searchValue : textChange});
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
            <div id="groups_content">
                <div className="col-md-12">
                    <div className="col-md-4" style={{"marginBottom": 5 + "px"}}>
                        <SearchControl onChange={this.updateGroupList} />
                    </div>
                    <div className="col-md-6" style={{"textAlign": "center"}}>
                        <Tooltip idx="groupTooltip" content={this.state.content} isShow={this.state.openTooltip}/>
                    </div>
                </div>
                <div className="col-md-12">
                    <div className="col-md-4">
                        <SelectListRender list={this.state.groupList} filterBy={this.state.searchValue} onSelect={this.onSelect} />
                    </div>
                    <div className="col-md-6">
                        <PermissionPanel idx="groupGrid" list={this.state.permissionDetail} onCheckboxClick={this.onCheckboxClick} />
                    </div>
                </div>
            </div>
        );
    }
});

export default  GroupTab;