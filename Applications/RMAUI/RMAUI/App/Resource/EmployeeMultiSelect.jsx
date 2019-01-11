
import React from 'react';

var EmployeeMultiSelect = React.createClass({
    getDefaultProps: function() {
        return {
            maxItem: 10
        }  
    },
    getInitialState: function () {
        return {
            list: null,
            selectedEmployee: []
        }
    },    
    componentDidMount: function () {
        console.log('multiselect loaded');
        var self = this;
        this.getUser();
        $("#" + this.props.idx).kendoMultiSelect({
            maxSelectedItems: this.props.maxItem,
            dataSource: this.state.list,
            change: function (e) {
                self.props.onSelect(this.value());
                self.setState({selectedEmployee: this.value()});
            },
            dataTextField: "Name",
            dataValueField: "Name",
            itemTemplate: function(dataItem){ 
                dataItem.employee = self.state.selectedEmployee;
                return kendo.template($("#resourceTemplate").html())(dataItem);
            } ,
            autoClose: false,
            select: function(e) {
                console.log(e);
                var item = e.item;
                var text = item.text();
                item.find("input[type=checkbox]").prop('checked', true)
            },
            deselect: function(e) {
                var dataItem = e.dataItem;
                var item = e.item;
                // Use the deselected data item or jQuery item

            }
        });
    },
    componentDidUpdate: function(){
        var self =this;
        var dataSource = new kendo.data.DataSource({
            data: self.state.list
        });
        var grid = $("#" + this.props.idx).data("kendoMultiSelect");
        grid.setDataSource(dataSource); 
    },
    getUser: function() {
        var self = this;
        $.ajax({
            type: "GET",
            dataType: "JSON",
            url: this.props.url,
            success: function (response) {   
                self.setState({ list: response });                
            }
        });
    },
    render: function () {
        return (
            <select id={this.props.idx} multiple="multiple" data-placeholder={this.props.placeholder}>  
            </select>
        );
}
});

export default EmployeeMultiSelect;