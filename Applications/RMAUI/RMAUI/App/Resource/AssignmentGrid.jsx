import React from 'react';
import dateNames from 'date-names/en';
import moment from "moment";
import HourCell from "./HourCell.jsx";
var AssignmentGrid = React.createClass({
    getInitialState: function () {
        return {options: []}
    },
    componentWillMount: function () {},
    componentDidMount: function () {
        var self = this;
        $.ajax({
            type: "GET",
            dataType: "JSON",
            url: "/Resource/getEnums",
            success: function (response) {
                self.setState({options: response});
            }
        });
        $("#logHours")
            .kendoGrid({width: 806, height: 600})
            .data("kendoGrid");

    },  
    componentWillUpdate: function () {
        $("textarea")
            .each(function (i, val) {
                if ($(val).val() == "") {
                    $(val).css("font-size", "12");
                    $(val)
                        .next()
                        .val("Not Selected");
                }
            });
    },
    componentDidUpdate: function () {
        var self = this;
        $("textarea").kendoDropTarget({
            drop: function (e) {
                var dragItem = $(e.draggable.currentTarget[0]).find("div")[0];
                e.dropTarget.val($(dragItem).text());
                self.props.updateHandler();
            },
            group: "gridGroup1"
        });
    },    
    getColor: function (value) {
        var dict = {
            "select one": "#FF0000",
            "Good To Go": "#87bf6f",
            "Schedule W/C": "#fffa00",
            "Waiting Approval": "#ff6600",
            "Place Holder": "#1a75ff",
            "No Scope": "#FF0000"

        };
        return dict[value];
    },    
    render: function () {
        var self = this;
        let firstMonday = moment(new Date(this.props.year, this.props.month, 1))
            .startOf('month')
            .day("Monday");
        let rows = [];
        var projectList = this.props.projectList;
        if (this.props.isShow && projectList != []) {
            this
                .props
                .selectedEmployee
                .forEach(function (emp) {
                    var employeeAssignment = projectList != null && (emp in projectList)
                        ? projectList[emp]
                        : null;
                    dateNames
                        .abbreviated_days
                        .slice(1, 6)
                        .map(function (weekday, i) {
                            rows.push(
                                <tr>
                                    <td>
                                        <strong>{i == 0? emp: ""}</strong>
                                    </td>
                                    <td>
                                        {weekday}
                                    </td>
                                    {[0, 1, 2, 3, 4]
                                        .map(function (idx) {
                                            let iDate = firstMonday.clone().day(i + idx * 7 + 1);
                                            let timestamp = "/Date(" + iDate.unix() + "000)/"; // hack around
                                            let assignedTask = (employeeAssignment && (timestamp in employeeAssignment))
                                                ? employeeAssignment[timestamp]
                                                : null;
                                            let selectedColor = assignedTask
                                                ? self.getColor(assignedTask.AssignmentOption)
                                                : "#fff";
                                            let textValue = assignedTask? assignedTask.ProjectTitle: "";
                                            let selectedOption = assignedTask? assignedTask.AssignmentOption: "Not Selected";
                                            return (<HourCell
                                                updateDelete={self.props.updateDelete}
                                                updateHandler={self.props.updateHandler}
                                                key={idx}
                                                employee={emp}
                                                project={employeeAssignment}
                                                date={iDate}
                                                options={self.state.options}
                                                selectedOption={selectedOption}
                                                selectedColor={selectedColor}
                                                textValue={textValue}
                                                isFocused={false} />);
                                        })}
                                </tr>
                            )
                        })

                })
        }

        return (
            <div className="pull-left mygrid">
                <table id="logHours" className="table table-striped table-bordered">
                    <colgroup>
                        <col style={{width: 80}}/>
                        <col style={{width: 60}}/>
                        <col style={{width: 150}}/>
                        <col style={{width: 150}}/>
                        <col style={{width: 150}}/>
                        <col style={{width: 150}}/>
                        <col style={{width: 150}}/>
                    </colgroup>
                    <thead role="rowgroup">
                        <tr role="row">
                            <th className="k-header" data-field="employee">Employee</th>
                            <th className="k-header" data-field="day">Day</th>
                            {[0, 1, 2, 3, 4]
                                .map(function (i) {
                                    const iMonday = firstMonday.clone().day(i * 7 + 1),
                                        iFriday = iMonday.clone().day(5),
                                        dayy = iMonday.format("MMM") + iMonday.get("D") + "-" + iFriday.format("MMM") + iFriday.get("D");
                                    return (
                                        <th className="k-header" data-field={dayy}>
                                            {iMonday.format("MMM")}{iMonday.get("D")}-{iFriday.format("MMM")}{iFriday.get("D")}</th>
                                    );
                                })}
                        </tr>
                    </thead>
                    <tbody id="workhourtbody">
                        {this.props.isShow? rows: ""}
                    </tbody>
                </table>
            </div>
        );
    }
});

export default AssignmentGrid;