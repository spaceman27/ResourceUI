
import React from 'react';
import ReactDOM from 'react-dom';
import MultiSelectBox from './MultiSelectBox.jsx';
import "./developer.css";

const DeveloperView = React.createClass({
    getInitialState: function () {
        return {assignments: []}
    },
    componentDidMount: function () {
        this.getAssignment();
        this.initScheduler();
        this.myTimeout();
    },
    componentDidUpdate: function () {
        var self = this;
        var scheduler = $("#scheduler").data("kendoScheduler");
        var dataSource = new kendo
            .data
            .SchedulerDataSource({data: self.state.assignments});
        if (scheduler) {
            scheduler.setDataSource(dataSource);
        }
    },
    getAssignment: function (listEmployees = []) {
        var self = this;        
        $.ajax({
            type: "POST",
            dataType: "JSON",
            url: "/Developer/GetAssignment",
            contentType: 'application/json;charset=utf-8',
            data: JSON.stringify(listEmployees),            
            success: function (response) {
                self.setState({assignments: response});
            }
        });
    },
    initScheduler: function () {
        var self = this;
        $("#scheduler").kendoScheduler({
            editable: false,
            messages: {
                allDay: "Task"
            },
            showWorkHours: true,
            majorTick: 1440,
            majorTimeHeaderTemplate: "",
            navigate: function () {
                // update data

                
                setTimeout(function () {
                    if ($("#scheduler").data("kendoScheduler").viewName() === "week" || $("#scheduler").data("kendoScheduler").viewName() === "day") {
                        $(".k-scheduler-content")
                            .closest("tr")
                            .remove();
                        $(".k-header.k-scheduler-footer").remove();
                        $("#scheduler")
                            .data("kendoScheduler")
                            .refresh();
                    }
                }, 0);
            },
            date: new Date(),
            startTime: new Date(),
            height: 600,
            views: [
                "day", {
                    type: "week",
                    selected: true
                },
                "month",
                "agenda", {
                    type: "timelineMonth",
                    startTime: new Date()
                }
            ],
            timezone: "Etc/UTC",
            dataSource: {
                batch: true,
                transport: {
                    read: function (e) {
                        e.success(self.state.assignments);
                    }
                },
                schema: {
                    model: {
                        id: "id",
                        fields: {
                            //combined fields from the two schemas:
                            id: {
                                type: "number"
                            },
                            title: {
                                defaultValue: "No title",
                                validation: {
                                    required: true
                                }
                            },
                            start: {
                                type: "date"
                            },
                            end: {
                                type: "date"
                            },
                            isAllDay: {
                                type: "boolean"
                            },
                            roomId: {
                                nullable: true
                            },
                            assignedBy: {
                                type: "string"
                            }
                        }
                    }
                }
            },
            resources: [
                {
                    field: "roomId",
                    dataSource: [
                        {
                            value: 1,
                            color: "#6eb3fa"
                        }, {
                            value: 2,
                            color: "#f58a8a"
                        }, {
                            value: 3,
                            color: "#6FBF72"
                        }, {
                            value: 4,
                            color: "#FFAC32"
                        }, {
                            value: 5,
                            color: "#6573C4"
                        }, {
                            value: 6,
                            color: "#FAD849"
                        }, {
                            value: 7,
                            color: "#800000"
                        }, {
                            value: 8,
                            color: "#FF00FF"
                        }, {
                            value: 9,
                            color: "#EF5D7F"
                        }, {
                            value: 10,
                            color: "#000000"
                        }
                    ],
                    title: "Room"
                }
            ]
        });
        $("#scheduler").kendoTooltip({
            filter: ".k-event:not(.k-event-drag-hint) > div, .k-task",
            position: "top",
            width: 200,
            content: kendo.template($('#developerTemplate').html())
        });

    },
    onSelectEmployee: function (listEmployees) {
        this.getAssignment(listEmployees);
    },
    myTimeout: function () {
        var self = this;
        var timer = setTimeout(function () {
            if (($("#scheduler").data("kendoScheduler")._selectedViewName === "week" || $("#scheduler").data("kendoScheduler")._selectedViewName === "day") && $(".k-scheduler-content").closest("tr").length > 0) {
                $(".k-scheduler-content")
                    .closest("tr")
                    .remove();
                $(".k-header.k-scheduler-footer").remove();
                $("#scheduler")
                    .data("kendoScheduler")
                    .refresh();
                clearTimeout(timer);
                return;
            }
            self.myTimeout();
        }, 0);
    },
    
    render: function () {
        let multibox = window.developerPermission.Write
            ? <div>
                    <MultiSelectBox
                        idx="selDeveloper"
                        placeholder="Select Developer.."
                        onSelect={this.onSelectEmployee}
                        url="/Developer/GetUserList"/>
                </div>
            : "";
        return (
            <div id="example" className="k-content">
                {multibox}
                <div id="scheduler"></div>
            </div>
        );
    }
});

export default DeveloperView;