import React from 'react';
import moment from 'moment';
import dateNames from 'date-names/en';
import Dialog from "../Common/Dialog.jsx";

const ScheduleGrid = React.createClass({
    getInitialState: function () {
        return {
            isShowDialog: false,
            activeSkillDict: {}
        };
    },
    componentWillMount: function () { },
    componentDidMount: function () {
        var self = this;
        if (window.schedulePermission.Write) {
            $("#schedule")
                .kendoGrid({
                    dataSource: [],
                    pageable: true,
                    editable: true,
                    detailInit: self.detailInit,
                    columns: [
                        { field: "Title", width: "200px", editable: function (item) { return false; } },
                        { field: "PlannedStartDate", editable: function (item) { return false; }, template: function (dataItem) { return self.formatDate(dataItem.PlannedStartDate); } },
                        { field: "ExpectedCompletionDate", editable: function (item) { return false; }, template: function (dataItem) { return self.formatDate(dataItem.ExpectedCompletionDate); } },
                        { field: "ProjectDueDate", editor: self.dueDateDatePickerEditor, template: function (dataItem) { dataItem.ProjectDueDate = self.formatDate(dataItem.ProjectDueDate); return dataItem.ProjectDueDate; } },
                        { field: "TotalPlannedHours", editable: function (item) { return false; } },
                        { field: "RequiredSkills", editable: function (item) { return false; }, template: function (dataItem) { return "<button class='requireSkill k-button' type='button'>View</button>"; } },
                        { field: "ReferenceLink", editable: function (item) { return false; }, template: function (dataItem) { return "<button  class='referenceLink k-button' type='button'>View</button>"; } },
                        { field: "ProjectStatus", editor: self.statusDropDownEditor, template: "#=ProjectStatus#" }
                    ],
                    height: 525,
                    sortable: true
                });
        }
        else {
            $("#schedule")
                .kendoGrid({
                    dataSource: [],
                    pageable: true,
                    detailInit: self.detailInit,
                    columns: [
                        { field: "Title", width: "200px", },
                        { field: "PlannedStartDate", template: function (dataItem) { return self.formatDate(dataItem.PlannedStartDate); } },
                        { field: "ExpectedCompletionDate", template: function (dataItem) { return self.formatDate(dataItem.ExpectedCompletionDate); } },
                        { field: "ProjectDueDate", template: function (dataItem) { dataItem.ProjectDueDate = self.formatDate(dataItem.ProjectDueDate); return dataItem.ProjectDueDate; } },
                        { field: "TotalPlannedHours", },
                        { field: "RequiredSkills", template: function (dataItem) { return "<button class='requireSkill k-button' type='button'>View</button>"; } },
                        { field: "ReferenceLink", template: function (dataItem) { return "<button  class='referenceLink k-button' type='button'>View</button>"; } },
                        { field: "ProjectStatus", template: "#=ProjectStatus#" }
                    ],
                    height: 525,
                    sortable: true
                });
        }
        $("#skills").kendoGrid({
            height: 400,
            scrollable: true,
            groupable: true,
            columns: [
                {
                    template: function (dataItem) {
                        return '<input class="chkSkill" ' + (window.schedulePermission.Write ? "" : "disabled") + ' type="checkbox"' + (dataItem.SkillName in self.state.activeSkillDict ? "checked='checked'" : "") + '/>';
                    }, width: 50
                },
                { field: "Category", title: "Category" },
                { field: "SkillName", title: "SkillName", groupable: false }
            ]
        });
        if (window.schedulePermission.Write) {
            $("#refGrid").kendoGrid({
                height: 400,
                editable: true,
                scrollable: true,
                toolbar: ["create", "save"],
                columns: [{ field: "ReferenceLink", title: "Link" }, { command: "destroy", width: 145 }],
                saveChanges: self.RefonChange,
                remove: function (e) {
                    console.log("Removing", e.model.ReferenceLink);
                }
            });
        }
        else {
            $("#refGrid").kendoGrid({
                height: 400,
                scrollable: true,
                columns: [{ field: "ReferenceLink", title: "Link" }],

            });
        }
        $("#wnd").kendoWindow({
            height: 500,
            width: 600,
            visible: false,
            draggable: false,
            modal: true,
            title: "SkillSets"
        });
        $("#wnd2").kendoWindow({
            height: 500,
            width: 600,
            visible: false,
            draggable: false,
            modal: true,
            title: "Reference Links"
        });
        $("#schedule").on("click", ".requireSkill", this.onRequireSkillClick);
        $("#schedule").on("click", ".referenceLink", this.onRefClick);
        if (window.schedulePermission.Write) {
            $("#skills").on("click", ".chkSkill", this.onSkillsCheckboxClick);

        }
    },
    componentWillUpdate: function () { },
    componentDidUpdate: function () {
        var self = this;
        var dataSource = new kendo.data.DataSource({
            batch: true,
            pageSize: 6,
            autoSync: true,
            schema: {
                model: {
                    id: "Title"
                },
                fields: {
                    Title: {},
                    PlannedStartDate: {},
                    ExpectedCompletionDate: {},
                    TotalPlannedHours: {},
                    ProjectDueDate: {},
                    ProjectStatus: {},
                    RequiredSkills: {},
                    ReferenceLink: {}
                }
            },
            transport: {
                read: function (options) {
                    options.success(self.props.data);
                },
                update: function (options) {
                    console.log("synced");
                    $.ajax({
                        type: "POST",
                        dataType: "JSON",
                        url: "/ProjectSchedule/UpdateData",
                        data: JSON.stringify(options.data.models),
                        contentType: 'application/json;charset=utf-8',
                        success: function (resp) {
                            options.success(resp.Schedule);
                        },
                        error: function (result) {
                            options.error(result);
                        }
                    });
                }
            }
        });

        $("#schedule").data("kendoGrid").setDataSource(dataSource);
    },
    RefonChange: function (e) {
        let refLinks = [];
        $("#refGrid").find("tr").each(function (i, val) {
            let refText = $($(val).find("td")[0]).text().trim();
            if (refText !== "") {
                refLinks.push(refText);
            }
        });
        // Update dsSchedule with Ref Links
        let dsSchedule = $("#schedule").data("kendoGrid").dataSource;
        dsSchedule.fetch(function () {
            let editedIndex = function () {
                let count = dsSchedule._data.length;
                for (let i = 0; i < count; i++) {
                    if (dsSchedule._data[i].Title === self.state.editedRow.Title) {
                        return i;
                    }
                }
                return null;
            }();
            dsSchedule.at(editedIndex).ReferenceLink = refLinks;
            dsSchedule.at(editedIndex).dirty = true;
            dsSchedule.sync();
        });
        self.setState({ message: "Update Succeed!", dialogTitle: "Information!", isShowDialog: true });
    },
    onRequireSkillClick: function (e) {
        let row = $(e.target).closest("tr");
        let grid = $("#schedule").data("kendoGrid");
        let item = grid.dataItem(row);
        let skillDict = {};
        for (var value of item.RequiredSkills) {
            skillDict[value] = value;
        }
        this.setState({ activeSkillDict: skillDict, editedRow: item });

        $("#skillCount").html("<strong>" + item.RequiredSkills.length + "</strong> selected");
        // update grid data source before open modal dialog
        if (this.props.skills) {
            var dsSkill = new kendo.data.DataSource({
                data: this.props.skills,
                group: { field: "Category" }
            });
            $("#skills").data("kendoGrid").setDataSource(dsSkill);
        }
        //$("#skills .k-grouping-header").remove();   => should remove group header ???
        let wnd = $("#wnd").data("kendoWindow");
        wnd.center().open();
    },
    onRefClick: function (e) {
        let row = $(e.target).closest("tr");
        let grid = $("#schedule").data("kendoGrid");
        let item = grid.dataItem(row);

        let refs = item.ReferenceLink.map(function (item) {
            return { ReferenceLink: item };
        });
        this.setState({ editedRow: item });
        let dsRef = new kendo.data.DataSource({
            data: refs
        });
        $("#refGrid").data("kendoGrid").setDataSource(dsRef);

        let wnd = $("#wnd2").data("kendoWindow");
        wnd.center().open();
    },
    onSkillsCheckboxClick: function (e) {
        var checked = this.checked;
        let skillsGrid = $("#skills").data("kendoGrid");
        let requireSkills = [];
        $(".chkSkill:checkbox:checked").each(function (i, val) {
            let row = $(this).closest("tr");
            let dataItem = skillsGrid.dataItem(row);
            requireSkills.push(dataItem.SkillName);
        });
        $("#skillCount").html("<strong>" + $(".chkSkill:checkbox:checked").length + "</strong> selected");

        // update dsSchedule with requireSkill
        let dsSchedule = $("#schedule").data("kendoGrid").dataSource;
        dsSchedule.fetch(function () {
            let editedIndex = function () {
                let count = dsSchedule._data.length;
                for (let i = 0; i < count; i++) {
                    if (dsSchedule._data[i].Title === self.state.editedRow.Title) {
                        return i;
                    }
                }
                return null;
            }();
            dsSchedule.at(editedIndex).RequiredSkills = requireSkills;
            dsSchedule.at(editedIndex).dirty = true;
            dsSchedule.sync();
        });
    },
    dueDateDatePickerEditor: function (container, options) {
        let self = this;
        let date = options.model.ProjectDueDate;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDatePicker({
                value: new Date(parseInt(date.replace(/\D/g, '')))
            });
    },
    statusDropDownEditor: function (container, options) {
        let self = this;
        $('<input name="' + options.field + '"/>')
            .appendTo(container)
            .kendoDropDownList({
                dataTextField: "Status",
                dataValueField: "Status",
                dataSource: self.props.status
            });
    },
    formatDate: function (unixTimeStamp) {
        if (unixTimeStamp instanceof Date || moment(unixTimeStamp, "MM/DD/YYYY").isValid()) {
            return moment(unixTimeStamp, "MM/DD/YYYY").format("MM/DD/YYYY");
        }
        return moment(new Date(parseInt(unixTimeStamp.replace(/\D/g, '')))).format("MM/DD/YYYY");
    },
    detailInit: function (e) {
        var self = this;
        $("<div/>").appendTo(e.detailCell).kendoGrid({
            dataSource: {
                transport: {
                    read: function (res) {
                        res.success(e.data.projects);
                    }
                }
            },
            columns: [
                { field: "ProjectID" },
                { field: "Hours" },
                { field: "spPlannedDate", title: "Planned Date", template: function (dataItem) { return self.formatDate(dataItem.spPlannedDate); } }
            ]
        });
    },
    onCloseDialog: function (e) {
        this.setState({ isShowDialog: false });
    },
    render: function () {
        return (
            <div>
                <div id="wnd">
                    <div id="skills" />
                    <br />
                    <span id="skillCount" />
                </div>
                <div id="wnd2">
                    <div id="refGrid" />
                </div>
                <div id="schedule" />
                <Dialog title={this.state.dialogTitle} content={this.state.message} isShow={this.state.isShowDialog} close={this.onCloseDialog} />
            </div>

        );
    }
});

export default ScheduleGrid;
