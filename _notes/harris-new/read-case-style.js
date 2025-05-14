/*
read case style
*/
let read_case_style = {
	'read_row': function(lnk) {
		if (null == lnk) return null;
		try {
		let row = lnk.closest('tr');
		return {
			'court': row.cells[2].innerText,
			'fileDate': row.cells[3].innerText,
			'status': row.cells[4].innerText,
			'caseType': row.cells[5].innerText,
			'caseStyle': row.cells[7].innerText,
			'caseNumber': lnk.innerText
		}
		} catch {
			return null;
		}
		
	}
    'execute': function () {
        let status = {
            'name': 'Read case styles',
			'data': [],
            'result': true
        }        
        try {
			var links = Array.prototype.slice.call( document.getElementsByTagName('a'), 0 )
			links = links.filter(a => a.classList.contains('doclinks'));
			links.forEach(lnk => {
				let dta = read_case_style.read_row(lnk);
				if (null != dta) { status.data.push( dta ); }
			});
            return JSON.stringify(status);
        } catch {
            status.result = false;
            return JSON.stringify(status);
        }
    }
}
read_case_style.execute();