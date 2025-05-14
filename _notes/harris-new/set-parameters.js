/*
set parameters script
*/
let set_parameters = {
    'names': [
        'Starting Date',
        'Ending Date'
    ],
    'controls': [
        'ctl00_ContentPlaceHolder1_txtFrom',
        'ctl00_ContentPlaceHolder1_txtTo'
    ],
    'control_values': [
        '04/28/2025',
        '05/05/2025'
    ],
    'set_value': function (id) {
        let name = set_parameters.controls[id];
        let opt_value = set_parameters.control_values[id];
        let tbx = document.getElementById(name);
        tbx.value = opt_value;
    },
    'populate': function () {
        let status = {
            'name': 'Populate search date range',
            'result': true
        }
        try {
            for (let i = 0; i < set_parameters.controls.length; i++) {
                set_parameters.set_value(i);
            }
            return JSON.stringify(status);
        } catch {
            status.result = false;
            return JSON.stringify(status);
        }
    }
}
set_parameters.populate();