
import React from 'react';
import ReactDOM from 'react-dom';
import EmployeeMultiSelect from './EmployeeMultiSelect.jsx';
import "./resource.css";
import dateNames from 'date-names/en';
import AssignmentGrid from "./AssignmentGrid.jsx";
import ProjectSideTable from "./ProjectSideTable.jsx";
import Dialog from "../Common/Dialog.jsx";
import moment from "moment";
var ResourceView = React.createClass({
    getInitialState: function () {
        return {
            assignments: [],
            selectedYear: null,
            selectedMonth: null,
            selectedEmployee: null,
            projectList: [],
            isShowDialog: false,
            isShowGrid: false,
            reset: false
        };
    },
    componentWillMount: function () {

    },
    componentDidMount: function () {
        this.getProjectListRequest();
    },

    componentDidUpdate: function () {

    },
    getProjectListRequest: function () {
        var self = this;
        var input = self.prepareLoadGridData();
        $.ajax({
            type: "POST",
            dataType: 'JSON',
            data: JSON.stringify(input),
            contentType: 'application/json;charset=utf-8',
            url: "/Resource/showAssignmentstatus",
            success: function (response) {
                self.setState({ assignmentStatus: response });
            }
        });
    },
    onSelectEmployee: function (listEmployees) {
        var shouldShowGrid = $('#selectedYear').val() !== "0" && $('#selectedMonth').val() !== "-1" && listEmployees !== null;
        if (!shouldShowGrid) {
            this.setState({
                projectList: []
            });
        }

        this.setState({
            selectedEmployee: listEmployees,
            isShowGrid: shouldShowGrid
        });
    },
    onChangeMonYear: function () {
        let year = parseInt($('#selectedYear').val());
        let month = parseInt($('#selectedMonth').val());
        var shouldShowGrid = year !== 0 && month !== -1 && this.state.selectedEmployee !== null;
        if (!shouldShowGrid) {
            this.setState({
                projectList: []
            });
        }
        this.setState({
            isShowGrid: shouldShowGrid,
            selectedYear: year,
            selectedMonth: month
        });
    },
    onChangeYear: function (e) {
        this.onChangeMonYear();
    },
    onChangeMonth: function (e) {
        this.onChangeMonYear();
    },
    prepareLoadGridData: function () {
        let year = parseInt($('#selectedYear').val());
        let month = parseInt($('#selectedMonth').val());
        let input = {
            "Year": year,
            "Month": month,
            "EmployeeNames": this.state.selectedEmployee
        };
        return input;
    },
    loadTheGrid: function () {
        var self = this;
        let year = parseInt($('#selectedYear').val());
        let month = parseInt($('#selectedMonth').val());
        if (year !== 0 && month !== -1 && this.state.selectedEmployee !== null) {
            $.ajax({
                type: "POST",
                dataType: "JSON",
                url: "/Resource/showdata",
                contentType: 'application/json;charset=utf-8',
                data: JSON.stringify(self.prepareLoadGridData()),
                success: function (response) {
                    var dict = {};
                    response.forEach(function (val) {
                        if (!(val.EmployeeName in dict)) {
                            dict[val.EmployeeName] = {};
                        }
                        dict[val.EmployeeName][val.AssignmentDate] = val;
                    });
                    console.log(response, dict);
                    self.setState({ projectList: dict, isShowGrid: true });
                }
            });
            self.setState({ assignmentStatus: null });
            this.getProjectListRequest();
        } else {
            self.setState({ message: "Please select Employee/Year/Month", dialogTitle: "Information!", isShowDialog: true });
        }
    },
    deleteProject: function (el) {
        var self = this;
        var assignment = [];
        var employeeName = $(el).data("name");
        var date = moment(new Date(parseInt($(el).attr("data-date"))));
        var selectedProject = $(el).find("textarea").val();
        var selectedOption = $(el).find("select").val();
        if (selectedProject !== null && selectedProject !== "") {
            assignment.push({
                "AssignedBy": window.currentLoggedUser,
                "EmployeeName": employeeName,
                "ProjectTitle": selectedProject,
                "AssignmentOption": selectedOption,
                "DayOfWeek": date.format("ddd"),
                "Year": date.get("Y"),
                "Month": date.format("MMM"),
                "Day": date.get("D")
            });
        }

        var input = {
            "Assignments": assignment,
            "ShowData": self.prepareLoadGridData()
        };
        $.ajax({
            url: '/Resource/deletedata',
            type: 'POST',
            dataType: 'JSON',
            data: JSON.stringify(input),
            contentType: 'application/json;charset=utf-8',
            traditional: true,
            success: function (response) {
                var dict = {};
                response.forEach(function (val) {
                    if (!(val.EmployeeName in dict)) {
                        dict[val.EmployeeName] = {};
                    }
                    dict[val.EmployeeName][val.AssignmentDate] = val;
                });

                self.setState({ projectList: dict });
                // hide x icon
                $(el).find("a").addClass("hidden");
                self.setState({ assignmentStatus: null });
                self.getProjectListRequest();
            },
            error: function (ex) {
                self.setState({ message: "Update Failed, Try again", isShowDialog: true });
            }
        });
    },

    saveTheGrid: function () {
        var self = this;
        var assignment = [];
        $(".hourCell").each(function (i, val) {
            const employeeName = $(val).data("name");
            const date = moment(new Date(parseInt($(val).attr("data-date"))));
            const selectedProject = $(val).find("textarea").val();
            const selectedOption = $(val).find("select").val();
            if (selectedProject !== null && selectedProject !== "") {
                assignment.push({
                    "AssignedBy": window.currentLoggedUser,
                    "EmployeeName": employeeName,
                    "ProjectTitle": selectedProject,
                    "AssignmentOption": selectedOption,
                    "DayOfWeek": date.format("ddd"),
                    "Year": date.get("Y"),
                    "Month": date.format("MMM"),
                    "Day": date.get("D")
                });
            }
        });
        var input = {
            "Assignments": assignment,
            "ShowData": self.prepareLoadGridData()
        };
        $.ajax({
            url: '/Resource/postdata',
            type: 'POST',
            dataType: 'JSON',
            data: JSON.stringify(input),
            contentType: 'application/json;charset=utf-8',
            traditional: true,
            success: function (response) {
                var dict = {};
                response.forEach(function (val) {
                    if (!(val.EmployeeName in dict)) {
                        dict[val.EmployeeName] = {};
                    }
                    dict[val.EmployeeName][val.AssignmentDate] = val;
                });

                self.setState({ projectList: dict, assignmentStatus: null });
                self.getProjectListRequest();
            },
            error: function (ex) {
                self.setState({ message: "Update Failed, Try again", isShowDialog: true });
            }
        });
    },
    onCloseDialog: function (e) {
        this.setState({ isShowDialog: false });
    },
    render: function () {
        return (
            <div className="k-content">
                <div>
                    <label>Name</label>
                    <EmployeeMultiSelect idx="selPeople" placeholder="Select People.." onSelect={this.onSelectEmployee} url="/Resource/GetEmployeeList" maxItem="10" />
                </div>

                <div>
                    <div className="inlineBlock">
                        <label>Select Year</label>
                        <select id="selectedYear" className="form-control" onChange={this.onChangeYear}>
                            <option value={0}> Select Year </option>
                            {[2017, 2018, 2019, 2020].map(function (val) {
                                return (
                                    <option key={val} value={val}> {val}</option>
                                );
                            })}
                        </select>
                    </div>
                    <div className="inlineBlock">
                        <label>Select Month</label>
                        <select id="selectedMonth" className="form-control" onChange={this.onChangeMonth}>
                            <option value={-1}> Select Month </option>
                            {dateNames.months.map(function (val, i) {
                                return (<option key={i} value={i}> {val} </option>);
                            })}
                        </select>
                    </div>
                    <div className="inlineBlock">
                        <button className="k-button" onClick={this.loadTheGrid} disabled={!this.state.isShowGrid}>LOAD</button> &nbsp;

</div>

                </div>
                <Dialog title={this.state.dialogTitle} content={this.state.message} isShow={this.state.isShowDialog} close={this.onCloseDialog} />
                <div className="row" style={{ marginTop: 10 + "px" }}>
                    <AssignmentGrid updateHandler={this.saveTheGrid} updateDelete={this.deleteProject} projectList={this.state.projectList} year={this.state.selectedYear} month={this.state.selectedMonth} selectedEmployee={this.state.selectedEmployee} isShow={this.state.isShowGrid} />
                    <ProjectSideTable idx="projectSideTable" assignmentStatus={this.state.assignmentStatus} />
                </div>
            </div>
        );
    }
});

export default ResourceView;