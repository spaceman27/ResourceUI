import React from "react";
import ReactDOM from "react-dom";
import dateNames from "date-names/en";
import moment from "moment";
import Dialog from "../Common/Dialog.jsx";
import ProjectGrid from "./ProjectGrid.jsx";
import "./project.css";

const ProjectView = React.createClass({
    getInitialState: function () {
        return {
            gridData: []
        };
    },
    componentWillMount: function () {
        this.onsearch();
    },    
    componentDidMount: function () {
        $('#startDatePicker').kendoDatePicker({
            //value: new Date()
        });
        $('#endDatePicker').kendoDatePicker({
            //value: new Date()
        });
    },
    componentDidUpdate: function () {

    },
    onsearch: function () {
        var self= this;
        var dict={};
        
        var input = {
            StartDate: $("#startDatePicker").val(),
            EndDate: $("#endDatePicker").val()
        };
        
        $.ajax({
            type: "POST",
            dataType: 'JSON',
            data: JSON.stringify(input),            
            url: "/Schedule/SearchProject",
            contentType: 'application/json;charset=utf-8',
            success: function (response) {
                let firstDate = null;
                let numsOfWeek = 7;
                response.forEach(function(row){
                    let arr = Object.keys(row.HourPerWeeks);
                    arr.forEach(function(val){
                        if(!firstDate){
                            firstDate = moment(val, "MM/DD/YYYY");
                        } else {
                            if(moment(val, "MM/DD/YYYY") < firstDate){ 
                                firstDate = moment(val, "MM/DD/YYYY");
                            }
                        }
                    });
                });
                                                                    
                // do array transformation
                let totalDays;
                if(!input.EndDate && !input.StartDate){
                    totalDays = 49; // init 7 weeks
                }else{
                    totalDays = moment(input.EndDate, "MM/DD/YYYY").diff(moment(input.StartDate, "MM/DD/YYYY"), 'days');
                }                                
                numsOfWeek = (totalDays / 7) + 1;                
                let newResult = response.map(function(val){
                    let newItem = {
                        ProjectType: val.ProjectType,    
                        Title: val.Title                                      
                    };
                  
                    for (let i = 0; i < numsOfWeek; i++){
                        let hour = "";
                        let dayLoop = firstDate.clone().add(i*7, 'days');
                        if (dayLoop.format("MM/DD/YYYY") in val.HourPerWeeks)
                        {
                            hour = val.HourPerWeeks[dayLoop.format("MM/DD/YYYY")];
                        }
                        newItem["Date"+dayLoop.format("x")] = hour;
                    }
                    return newItem;
                });
                self.setState({ gridData: newResult });                
            }
            
        });
    },
    render: function () {
        return (
            <div className="k-content">
                    <div style={{ marginBottom: 10 + "px"}}>
                        <span className="span-mgn">
                            <label className="label-mgn">Start Date</label>
                            <input id="startDatePicker" className="form-control"  />                            
                        </span>
                        <span className="span-mgn">
                            <label className="label-mgn">End Date</label>
                            <input id="endDatePicker" className="form-control" />
                        </span>
                        <span className="span-mgn">
                            <button className="k-button" onClick={this.onsearch}>Search</button>
                        </span>                        
                    </div>
                    <ProjectGrid data={this.state.gridData}/>
            </div>
        );
    }
});
        
export default ProjectView;