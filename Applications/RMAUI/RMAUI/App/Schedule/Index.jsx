 //import $ from "jquery";
import 'bootstrap-loader';
import React from "react";
import ReactDOM from "react-dom";
import dateNames from "date-names/en";
import moment from "moment";
import "./schedule.css";
import Dialog from "../Common/Dialog.jsx";
import ScheduleGrid from "./ScheduleGrid.jsx";
import FilterByCondition from "./FilterByCondition.jsx";

const ScheduleView = React.createClass({
    getInitialState: function () {
        return {
            gridData: [],
            gridDataFilter: [],
            listSkills: [],
            listStatus: []
        };
    },
    componentWillMount: function() {
        var self = this;
        $.getJSON("/ProjectSchedule/GetListSchedule", (resp) => self.setState({ gridData: resp.Schedule, gridDataFilter: resp.Schedule }));
        $.getJSON("/ProjectSchedule/GetProjectStatus", (resp) => self.setState({ listStatus: resp }));
        $.getJSON("/ProjectSchedule/ShowSkills", (resp) => self.setState({ listSkills: resp }));
    },    
    componentDidMount: function () {
        
    },
    componentDidUpdate: function() {
            
    },
    search: function(list){
        this.setState({ gridData: list });
    },
    render: function() {
        return (
            <div>
                <FilterByCondition data={this.state.gridDataFilter} status={this.state.listStatus} onSearch={this.search} skills={this.state.listSkills}/>
                <ScheduleGrid data={this.state.gridData} status={this.state.listStatus} skills={this.state.listSkills}/>
            </div>
        );
    }
});
export default ScheduleView;