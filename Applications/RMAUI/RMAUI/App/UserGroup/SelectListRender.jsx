import React from 'react';
var SelectListRender = React.createClass({
    render: function() {
        var filterList = this.props.filterBy
            ? this.props.list.filter((value) => value.Name.toLowerCase().search(this.props.filterBy.toLowerCase()) !== -1)
            : this.props.list;
        return (
             <select className="form-control" multiple="multiple" onChange={this.props.onSelect} style={{height: 335}}>
                 {
                     filterList &&
                         filterList.map(function(value, i) {
                             return <option key={i} value={value.Username}>{value.Name}</option>;
                         })
                 }
             </select>
        );
    }
});

export default  SelectListRender;