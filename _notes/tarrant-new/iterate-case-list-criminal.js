/*
Uri
CaseNumber
CaseStyle
DateFiled
Court
CaseType
*/
let criminal_case_reader = {
	get_rows: function () {
		let headers = Array.prototype.slice.call(document.getElementsByTagName('th'), 0);
		let firstheader = headers.find(h => h.getAttribute('class') == 'ssSearchResultHeader' && h.innerText.trim() == 'Case Number');
		if (null == firstheader) return null;
		let headingrow = firstheader.parentElement;
		while (null != headingrow && headingrow.tagName != 'TBODY') { headingrow = headingrow.parentElement; }
		if (headingrow == null) return null;
		let rows = Array.prototype.slice.call(headingrow.getElementsByTagName('tr'), 0);
		rows = rows.filter(rw => { let cells = Array.prototype.slice.call(rw.getElementsByTagName('td'), 0); return cells.length > 4; });
		return rows;
	},
	get_uri: function (cell) {
		let links = cell.getElementsByTagName('a');
		if (null == links || links.length == 0) return '';
		let lnk = links[0].getAttribute('href').toString();
		let origin = document.location.origin;
		if (lnk.indexOf(origin) >= 0) return lnk;
		let pg = document.location.href;
		let qry = document.location.search;
		if (qry.length > 0) {
			pg = pg.substring(0, pg.length - qry.length);
		}
		let landing = document.location.pathname
		if (landing.length > 0) {
			let items = landing.split('/');
			let lastitem = items[items.length - 1];
			pg = pg.substring(0, pg.length - lastitem.length);
		}
		lnk = ''.concat(pg, lnk);
		return lnk;
	},
	get_data: function (rw) {
        try {
            let cells = Array.prototype.slice.call(rw.getElementsByTagName('td'), 0);
            let caseNo = String(cells[0].innerText);
            let citationNbr = String(cells[1].innerText);
            let chargeType = String(cells[5].innerText);
            let caseStyle = String(cells[2].innerText);
            let dateFiled = String(cells[3].firstChild.innerText)
            let court = String(cells[3].lastChild.innerText);
            let caseType = String(cells[4].firstChild.innerText);
            let uri = String(criminal_case_reader.get_uri(cells[0]));
            return {
                'uri': uri,
                'caseNumber': caseNo,
                'caseStyle': caseStyle,
                'caseType': caseType,
                'court': court,
                'dateFiled': dateFiled,
                'charges': chargeType,
                'citationNumber': citationNbr
            }
        } catch {
			return null;
        }
	},
	get_cases: function () {
		let cases = [];
		let rws = criminal_case_reader.get_rows();
		if (null == rws) return cases;
		rws.forEach(rw => {
			let dt = criminal_case_reader.get_data(rw);
			if (null != dt) { cases.push(dt); }
		})
		return cases;
	}
}
let data = criminal_case_reader.get_cases();
return JSON.stringify(data);