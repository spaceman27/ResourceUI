import React from 'react';
var PermissionPanel = React.createClass({
    componentDidMount: function () {
        $("#" + this.props.idx).kendoGrid({});
    },
    render: function () {
        var self = this;
        return (
            <table id={this.props.idx}>
                <colgroup>
                    <col style={{ width: 150 + "px" }} />
                    <col style={{ width: 100 + "px" }} />
                    <col style={{ width: 100 + "px" }} />
                </colgroup>
                <thead>
                    <tr>
                        <th>Component</th>
                        <th>Read</th>
                        <th>Write</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.props.list &&
                        this.props.list[0].ModulePermissions.map(function (value, i) {
                            var obj = {
                                model: self.props.list[0],
                                modulePermission: value
                            };
                            return (
                                <tr key={i}>
                                    <td>{value.Name}</td>
                                    <td>
                                        <div onClick={self.props.onCheckboxClick.bind(this, obj, "r")}>
                                            <input disabled={!window.UserAndGroupPermission.Write} type="checkbox" checked={value.UserRight.Read} />
                                        </div>
                                    </td>
                                    <td>
                                        <div onClick={self.props.onCheckboxClick.bind(this, obj, "w")}>
                                            <input disabled={!window.UserAndGroupPermission.Write} type="checkbox" checked={value.UserRight.Write} />
                                        </div>
                                    </td>
                                </tr>
                            );
                        })
                    }
                </tbody>
            </table>
        );
    }
});

export default PermissionPanel;