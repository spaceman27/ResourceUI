
import React from 'react';

var MultiSelectBox = React.createClass({
    getDefaultProps: function() {
        return {
            maxItem: 10
        }  
    },
    getInitialState: function () {
        return {
            list: null
        }
    },
    getUser: function() {
        var self = this;
        $.ajax({
            type: "GET",
            dataType: "JSON",
            url: this.props.url,
            success: function (response) {                
                self.setState({ list: response });
                // update kendo ds
                var dataSource = new kendo.data.DataSource({
                    data: response
                });
                var multiselect = $("#" + self.props.idx).data("kendoMultiSelect");
                multiselect.setDataSource(dataSource);
            }
        });
    },
    componentDidMount: function () {
        var self = this;
        this.getUser();
        $("#" + this.props.idx).kendoMultiSelect({
            maxSelectedItems: this.props.maxItem,
            change: function (e) {
                self.props.onSelect(this.value());
            }
        });
    },
    render: function () {
        return (
            <select id={this.props.idx} multiple="multiple" data-placeholder={this.props.placeholder}>
                {
                    this.state.list && this.state.list.map(function (value, i) {
                        return (
                            <option key={i} value={value}> {value} </option>
                        );
                    })
                }
            </select>
        );
}
});

export default MultiSelectBox;