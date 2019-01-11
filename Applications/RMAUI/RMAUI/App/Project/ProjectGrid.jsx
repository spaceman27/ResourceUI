import React from 'react';
import moment from 'moment';
import dateNames from 'date-names/en';
import Dialog from "../Common/Dialog.jsx";

const ProjectGrid = React.createClass({
    getInitialState: function () {
        return {};
    },
    componentWillMount: function () {},
    componentDidMount: function () {
        $("#project").kendoGrid({
            pageable: true,
            height: 525,
            sortable: true,
            scrollable: true
        });
    },
    componentWillUpdate: function () {},
    componentDidUpdate: function () {
        var self = this;        
        var grid = $("#project").data("kendoGrid");
        let dynamicColumns = [];
        if (self.props.data.length>0) {
           var keys = Object.keys(self.props.data[0]);           
           let addColumns = keys.filter(val => val.startsWith("Date")).map(function (val) {
               let title = moment(new Date(parseInt(val.substr(4))), "MM/DD/YYYY").format("MM/DD/YYYY");
               return {field: val, title: title, className: "text-center", width: "105px", template: "<div class='text-center'>#: "+val+" # </div>"};
           });
           dynamicColumns = addColumns.concat([
               { field: "ProjectType", title: "Project Type", width: "107px", locked: true, lockable: false },
               { field: "Title", title: "Project Name", width: "200px", locked: true, lockable: false }
           ]);
        }else{
            dynamicColumns = [
                { field: "ProjectType", title: "Project Type", width: "107px" },
                { field: "Title", title: "Project Name", width: "200px" }
            ]
        }
        $("#project").empty();
        $("#project").kendoGrid({
            pageable: true,
            height: 525,
            sortable: true,
            scrollable: true,
            columns: dynamicColumns,
            dataSource: {
                data: self.props.data,
                pageSize: 8
            }
        });
    },
    render: function () {
        return (
            <div id="project"></div>
        );
    }
});
export default ProjectGrid;
