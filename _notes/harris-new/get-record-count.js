/*
read status label to get record count
*/
let read_status_label = {
    'controls': [
        'ctl00_ContentPlaceHolder1_lblCount'
    ],
    'execute': function () {		
        let name = read_status_label.controls[0];
        let spn = document.getElementById(name);
        let status = {
            'name': 'Read record count',
			'rowCount': 0,
            'result': true
        }        
        try {
            let txt = spn.innerText.trim();
			let arr = txt.split(' ');
			let itemCount = parseInt(arr[0]);
			if (!isNaN(itemCount)) { status.rowCount = itemCount; }
            return JSON.stringify(status);
        } catch {
            status.result = false;
            return JSON.stringify(status);
        }
    }
}
return read_status_label.execute();