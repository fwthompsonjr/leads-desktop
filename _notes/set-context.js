/*
set context script
*/
let set_context = {
    'names': [
        'Court',
        'Case Status'
    ],
    'controls': [
        'ctl00_ContentPlaceHolder1_ddlCourt',
        'ctl00_ContentPlaceHolder1_DropDownListStatus'
    ],
    'control_values': [
        'All',
        'Open'
    ],
    'set_value': function (id) {
        let ui_name = set_context.names[id];
        let name = set_context.controls[id];
        let opt_value = set_context.control_values[id];
        let cbo = document.getElementById(name);
        let options = Array.prototype.slice.call(cbo.getElementsByTagName('option'), 0);
        let option_index = options.findIndex(x => x.getAttribute('value') == opt_value);
        if (option_index < 0) {
            let msg = ''.concat("Error: unable to set control ", ui_name, " to ", opt_value);
            throw msg;
        }
        cbo.selectedIndex = option_index;
    },
    'populate': function () {
        let status = {
            'name': 'Populate search context',
            'result': true
        }
        try {
            for (let i = 0; i < set_context.controls.length; i++) {
                set_context.set_value(i);
            }
            return JSON.stringify(status);
        } catch {
            status.result = false;
            return JSON.stringify(status);
        }
    }
}
set_context.populate();