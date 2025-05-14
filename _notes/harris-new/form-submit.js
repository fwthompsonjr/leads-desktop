/*
send form values and post form
*/
let send_form = {
    'controls': [
        'ctl00_ContentPlaceHolder1_btnSearchCase'
    ],
    'set_value': function (id) {
        let name = send_form.controls[id];
        let button = document.getElementById(name);
        button.click()
    },
    'populate': function () {
        let status = {
            'name': 'Submit form values',
            'result': true
        }
        try {
            for (let i = 0; i < send_form.controls.length; i++) {
                send_form.set_value(i);
            }
            return JSON.stringify(status);
        } catch {
            status.result = false;
            return JSON.stringify(status);
        }
    }
}
send_form.populate();