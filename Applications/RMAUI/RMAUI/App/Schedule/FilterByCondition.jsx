// import $ from "jquery";
//require();
import React from 'react';
import moment from 'moment';
import dateNames from 'date-names/en';

const FilterByCondition = React.createClass({
    getInitialState: function () {
        return {};
    },
    componentWillMount: function () {},
    componentDidMount: function () {           
        let self = this;
        $("#fProject").kendoAutoComplete({
            filter: "contains",
            placeholder: "Select Project...",
            change: function(e) {
                var value = this.value();                
                $("#fProject").val(value);
                self.filterSchedule();
            }
        });
        $(".date-input").kendoDatePicker({
            change: function(e) {
                    self.filterSchedule();
            }
        });
        $("#fStatus").kendoDropDownList({
                dataTextField: "Status",
                dataValueField: "Value",
                dataSource: self.props.status,
                change: function(e) {
                    self.filterSchedule();
                }
            });
        $("#fSkill").kendoDropDownList({
            dataTextField: "SkillName",
            dataValueField: "SkillName",
            dataSource: self.props.skills,
            change: function(e) {
                self.filterSchedule();
            }
        });        
    },
    shouldComponentUpdate: function(nextProps, nextState){
        return nextProps.status!==this.props.status || nextProps.data!==this.props.data || nextProps.skills!==this.props.skills;
    },
    componentWillUpdate: function () {},
    componentDidUpdate: function () {
        // refresh project autocomplete
        let self= this;
        
        let dataSource = new kendo.data.DataSource({data: self.props.data.map((val)=> val.Title)});        
        $("#fProject").data("kendoAutoComplete").setDataSource(dataSource);
        // refresh status
        let newStatus = Array.from(self.props.status);
        newStatus.unshift({Status: "Select Status"});
        let dataSource1 = new kendo.data.DataSource({data: newStatus});
        let statusDropdown = $("#fStatus").data("kendoDropDownList");
        statusDropdown.setDataSource(dataSource1);
        statusDropdown.select(0);
        // refresh skills
        let newSkills = Array.from(self.props.skills);
        newSkills.unshift({Category: "", SkillName: "Select Skill"});
        let dataSource2 = new kendo.data.DataSource({data: newSkills});
        let skillDropdown = $("#fSkill").data("kendoDropDownList");
        skillDropdown.setDataSource(dataSource2);
        skillDropdown.select(0);
        //  update css
        $(".k-picker-wrap.k-state-default").height(34);
    },
    filterSchedule: function(){
        let self = this;
        let input = {
            "Project": $("#fProject").val(),
            "Status": $("#fStatus").val(),
            "PlannedDateFrom": $("#fPlannedDateFrom").val(),
            "PlannedDateTo": $("#fPlannedDateTo").val(),
            "DueDateFrom": $("#fDueDateFrom").val(),
            "DueDateTo": $("#fDueDateTo").val(),
            "PlannedHourFrom": $("#fPlannedHourFrom").val(),
            "PlannedHourTo": $("#fPlannedHourTo").val(),
            "Skills": [$("#fSkill").val()]
        };
        
        $.ajax({
            url: '/ProjectSchedule/SearchSchedule',
            type: 'POST',
            dataType: 'JSON',
            data: JSON.stringify(input),
            contentType: 'application/json;charset=utf-8',            
            success: function (response) {
                self.props.onSearch(response);
            },
            error: function (ex) {
                
            }
        });
    },
    resetForm: function(){
        $("#fProject").val("");
        $("#fStatus").data("kendoDropDownList").select(0);
        $("#fPlannedDateFrom").val("");
        $("#fPlannedDateTo").val("");
        $("#fDueDateFrom").val("");
        $("#fDueDateTo").val("");
        $("#fPlannedHourFrom").val("");
        $("#fPlannedHourTo").val("");
        $("#fSkill").data("kendoDropDownList").select(0);
        this.filterSchedule();
    },
    render: function () {
        var self = this;
        return (
            <div>                
                <label>Filter By </label>
                <a className="reset" onClick={this.resetForm}>Reset</a>
                <table className="table filterTable">
                    <tbody>
                    <tr>
                        <td className="FilterCol">Project</td>
                        <td><input id="fProject" onChange={this.filterSchedule} style={{width: 240+"px"}}/></td>
                        <td className="FilterCol">Status</td>
                        <td>
                            <select id="fStatus"/>
                        </td>
                    </tr>
                    <tr>
                        <td className="FilterCol">Planned Date</td>
                        <td><span><input id="fPlannedDateFrom" className="date-input" onChange={this.filterSchedule} /></span> to <input id="fPlannedDateTo" className="date-input" type="text" onChange={this.filterSchedule}/></td>
                        <td className="FilterCol">Due Date</td>
                        <td><input id="fDueDateFrom" className="date-input" type="text" onChange={this.filterSchedule}/> to <input id="fDueDateTo" className="date-input" type="text" onChange={this.filterSchedule}/></td>
                    </tr>
                    <tr>
                        <td className="FilterCol">Planned Hour</td>
                        <td><input id="fPlannedHourFrom" className="hour-input k-textbox" type="text" onChange={this.filterSchedule}/> to <input id="fPlannedHourTo" className="hour-input k-textbox" type="text" onChange={this.filterSchedule}/></td>
                        <td>Skills</td>
                        <td><select id="fSkill" /></td>
                    </tr>
                    </tbody>
                </table>
            </div>
        );
    }
});

export default FilterByCondition;
