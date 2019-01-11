import React from 'react';


var ProjectSideTable = React.createClass({
    getInitialState: function () {
        return {

        };
    },
    componentDidMount: function () {
        var grid1 = $("#" + this.props.idx).kendoGrid({
            height: 600,
            width: 550
        }).data("kendoGrid");
        if (window.assignmentPermission.Write) {
            $(grid1.element).kendoDraggable({
                filter: ".draggable:not(.disabled)",
                hint: function (e) {
                    let RH = $(e.children()[3]).text().split(" / ");
                    var item;
                    if (RH[0] === "0") {
                        item = $('<div></div>');
                    } else {
                        item = $('<div class="k-grid k-widget" style="background-color: DarkOrange; color: black;"><table><tbody><tr>' + $(e).find("td")[1].outerHTML + '</tr></tbody></table></div>');
                    }

                    return item;
                },
                group: "gridGroup1"
            });
        }

    },
    componentDidUpdate: function () {

    },
    render: function () {
        var isShow = this.props.isShow;
        return (
            <div className="pull-left mytable">
                <table id={this.props.idx}>
                    <colgroup>
                        <col style={{ width: 50 }} />
                        <col style={{ width: 300 }} />
                    </colgroup>
                    <thead>
                        <tr>
                            <th data-field="Status">Status</th>
                            <th data-field="Title">Title</th>
                        </tr>
                    </thead>
                    <tbody>
                        {
                            this.props.assignmentStatus && this.props.assignmentStatus.map(function (data, i) {
                                return (
                                    <tr key={i} className={"draggable " + (data.IsCompletelyAssigned ? "disabled" : "")}>
                                        <td><input type="checkbox" name="projectstatus" defaultChecked={data.IsCompletelyAssigned} /> </td>
                                        <td>
                                            <div>{data.ProjectTitle}</div>
                                            <div>
                                                <span>{data.ProjectTotalAssignedHours} / {data.ProjectTotalAssignedDays}</span>
                                                <span className={"hours"}>{data.RemainingHours} / {data.RemainingDays} </span>
                                                <span className={"hours"}>{data.ProjectTotalPlannedHours} / {data.ProjectTotalPlannedDays}</span>
                                            </div>
                                        </td>
                                    </tr>
                                );
                            })
                        }
                    </tbody>
                </table>
            </div>
        );
    }
});

export default ProjectSideTable;