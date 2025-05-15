/*
read case address
*/
let read_case_address = {
    'decode_text': function (str) {
        let element = document.createElement('div');
        element.innerHTML = str;
        return element.innerText.trim().replace('  ',' ');
    },
    'expand_row': function (rw) {
        try {
            rw.cells[8].getElementsByTagName('a')[0].click();
            return true;
        } catch {
            return false;
        }
    },
    'read_row': function (lnk) {
        if (null == lnk) return null;
        try {
            let row = lnk.closest('tr');
            let obj = {
                'caseNumber': lnk.innerText,
                'defendant': '',
                'address': ''
            }
            let canExpand = read_case_address.expand_row(row);
            if (!canExpand) { return obj; }
            let rowId = row.rowIndex + 1;
            let tbl = row.closest('table');
            let target = tbl.rows[rowId];
            let children = Array.prototype.slice.call(target.getElementsByTagName('table'), 0);
            let data = children.find(x => x.id == 'gridView');
            if (null == data) { return obj; }
            let datarows = Array.prototype.slice.call(data.getElementsByTagName('tr'), 0)
            let defendant_rows = datarows.filter(x => {
                if (x.cells.length < 3) { return false; }
                return (x.cells[1].innerText.trim().toLowerCase() == 'defendant');
            });
            if (defendant_rows.length == 0) { return obj; }
            let defendant_row = defendant_rows[0];
            if (defendant_rows.length > 1) {
                defendant_row = defendant_rows.filter(x => x.cells[2].innerText.length > 0);
                if (defendant_row == null) { defendant_row = defendant_rows[0]; }
            }
            if (null == defendant_row) { return obj; }
            let arr_address = [];
            let address_info = defendant_row.cells[2].innerHTML.split('<br>');
            for (let a = 0; a < address_info.length; a++) {
                let txt = read_case_address.decode_text(address_info[a]);
                if (a == 0) { obj.defendant = txt }
                else {
                    arr_address.push(txt);
                }
            }
            obj.address = arr_address.join('|');
            return obj;
        } catch {
            return null;
        }

    },
    'execute': function () {
        let status = {
            'name': 'Read case styles',
            'data': [],
            'result': true
        }
        try {
            let links = Array.prototype.slice.call(document.getElementsByTagName('a'), 0)
            links = links.filter(a => a.classList.contains('doclinks'));
            links.forEach(lnk => {
                let dta = read_case_address.read_row(lnk);
                if (null != dta) { status.data.push(dta); }
            });
            return JSON.stringify(status);
        } catch {
            status.result = false;
            return JSON.stringify(status);
        }
    }
}
return read_case_address.execute();