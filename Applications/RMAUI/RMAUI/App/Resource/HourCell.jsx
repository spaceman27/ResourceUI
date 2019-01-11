import React from 'react';
import dateNames from 'date-names/en';
import style from "./resource.css";
import moment from "moment";

var HourCell = React.createClass({
    getInitialState: function () {
        return {};
    },
    componentDidMount: function () { },
    componentDidUpdate: function () { },
    setColor: function (value, textarea) {
        var dict = {
            "Not Selected": "#fff",
            "Good To Go": "#87bf6f",
            "Schedule W/C": "#fffa00",
            "Waiting Approval": "#ff6600",
            "Place Holder": "#1a75ff",
            "No Scope": "#FF0000"
        };
        if (textarea) {
            var temp = textarea.val(); // hack around, backgroundColor must be set before set value otherwise, value will gone
            $(textarea).val(temp);
            $(textarea).css("background-color", dict[value]);
        }
    },
    bindColor: function (e) {
        var value = e.target.value;
        var textarea = $(e.target).prev();
        this.setColor(value, textarea);
        this.props.updateHandler();
    },
    onClearProject: function (e) {
        if (window.assignmentPermission) {
            this.props.updateDelete($(e.target).parents("td"));
        }
        e.preventDefault();
    },
    onMouseHover: function (e) {
        if (window.assignmentPermission) {
            let a = $(e.target).parents("td").find("a");
            let text = $(e.target).parents("td").find("textarea").val();
            if (a.hasClass("hidden") && text !== "") {
                a.removeClass('hidden');
            }
        }
        e.preventDefault();
    },
    onMouseBlur: function (event) {
        var from = event.relatedTarget ? event.relatedTarget : event.fromElement;
        var to = event.target ? event.target : event.toElement;
        let e = to || from;
        if (e.parentNode === this || e === this) {
            return;
        }
        let a = $(event.target).parents("td").find("a");
        a.addClass('hidden');
        event.preventDefault();
    },
    render: function () {
        var self = this;
        return (
            <td className="hourCell" data-name={this.props.employee} data-date={this.props.date}>
                <div className="cell-container" onMouseOver={this.onMouseHover} onMouseOut={this.onMouseBlur}>
                    <div className="item-1" >
                        <textarea disabled={!window.assignmentPermission.Write} className="form-control projectModel" style={{ backgroundColor: this.props.selectedColor }} value={this.props.textValue} />
                        <select disabled={!window.assignmentPermission.Write} className="form-control projectOption select-model" onChange={this.bindColor}>
                            <option value="Not Selected">select one</option>
                            {this.props.options && this.props.options.map(function (val, i) {
                                return (
                                    <option value={val} selected={self.props.selectedOption === val}>{val}</option>
                                );
                            })}
                        </select>
                    </div>
                    <div className="item-2">
                        <a href="#" className="clear hidden" onClick={this.onClearProject} />
                    </div>
                </div>
            </td>
        );
    }
});

export default HourCell;