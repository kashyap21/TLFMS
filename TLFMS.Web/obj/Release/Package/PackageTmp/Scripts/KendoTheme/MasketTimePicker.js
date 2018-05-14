(function ($) {
    var kendo = window.kendo,
        ui = kendo.ui,
        Widget = ui.Widget,
        proxy = $.proxy,
        CHANGE = "change",
        PROGRESS = "progress",
        ERROR = "error",
        NS = ".generalInfo";

    var MaskedTimePicker = Widget.extend({
        init: function (element, options) {
            var that = this;
            Widget.fn.init.call(this, element, options);

            $(element).kendoMaskedTextBox({ mask: that.options.timeOptions.mask || "00:00" })
            .kendoTimePicker({
                format: that.options.timeOptions.format || "HH:mm",
                parseFormats: that.options.timeOptions.parseFormats || ["HH:mm", "H:m"],
                change: OnChange
            })
            .closest(".k-timepicker")
            .add(element)
            .removeClass("k-textbox");
        },
        options: {
            name: "MaskedTimePicker",
            timeOptions: {}
        },
        destroy: function () {
            var that = this;
            Widget.fn.destroy.call(that);

            kendo.destroy(that.element);
        },
        value: function (value) {
            var timepicker = this.element.data("kendoTimePicker");

            if (value === undefined) {
                return timepicker.value();
            }

            timepicker.value(value);
        }
    });

    ui.plugin(MaskedTimePicker);

})(window.kendo.jQuery);

function OnChange() {
    if (this.element.attr("id") == "MaximumResponseTime") {
        var thisRow = $("#fmsslaGrid tbody").find(".k-grid-edit-row");
        var data = $("#fmsslaGrid").data("kendoGrid").dataItem(thisRow);
        data.set("MaximumResponseTime", this._value);
    }
}