
import React from "react";

const Dialog = React.createClass({
    getDefaultProps: function() {
        return {
            width: "400px",
            title: "Error",
            closable: false,
            modal: false,
            content: "The server has some error at this time. Pleae try again later",
            actions: [
                { text: "OK", primary: true }
            ],
            close: null,
            open: null
        };
    },
    componentDidMount: function() {
    },
    componentDidUpdate: function() {        
        if (this.props.isShow) {
            $("#RmsDialog").kendoDialog({
                width: this.props.width,
                title: this.props.title,
                closable: this.props.closable,
                modal: this.props.modal,
                content: this.props.content,
                actions: this.props.actions,
                close: this.props.close,
                open: this.props.open});
            $("#RmsDialog").data("kendoDialog").open();
        }
    },
    render: function() {
        return (
            <div id="RmsDialog" />
        );
    },
});

export default Dialog;
