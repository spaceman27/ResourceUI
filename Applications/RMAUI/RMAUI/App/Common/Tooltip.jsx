
import React from 'react';
var Tooltip = React.createClass({    
    getDefaultProps: function () {
        return {
            width: "200px",
            height: "20px",
            position: "center",
            content: "Server error. Please try again!",
            isShow: false,
            callout: false,
            autoHide: true,
            animation: {
                close: { effects: "fade:out", duration: 500 },
                open: { effects: "fade:in", duration: 200 }
            }
        };
    },
    getInitialState: function () {
        return {
            tooltip: null
        }
    },
    componentWillUpdate: function () {
        var tooltip = $("#"+this.props.idx).kendoTooltip({
            content: this.props.content,
            width: this.props.width,
            height: this.props.height,
            position: this.props.position,
            autoHide: this.props.autoHide,
            callout: this.props.callout,
            animation: this.props.animation
        }).data("kendoTooltip");
        if (this.props.isShow) {
            tooltip.show($("#"+this.props.idx));
        }
    },
    render: function () {
        return (
            <span id={this.props.idx} className="ReToolTip"></span>
        );
    }
});

export default  Tooltip;