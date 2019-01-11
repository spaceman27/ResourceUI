
import React from 'react';
var SearchControl = React.createClass({
    getInitialState: function() {
        return {

        }
    },
    componentDidMount: function() {
        
    },
    changeHandler: function(e) {
        var text = e.target.value;
        this.props.onChange(text);
    },
    render: function () {
        return (
            <span className="k-textbox k-space-right" style={{ "width": 305 + "px"}}>
                <input type="text" id="icon-left" placeholder="Filter by Name" onChange={this.changeHandler}/>
                <a href="#" className="k-icon k-i-search">&nbsp;</a>
            </span>
        );
}
});

export default  SearchControl;